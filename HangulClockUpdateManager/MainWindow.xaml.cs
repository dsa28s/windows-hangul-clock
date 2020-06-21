using HangulClockDataKit;
using HangulClockDataKit.Model;
using HangulClockKit;
using HangulClockUIKit;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Path = System.IO.Path;

namespace HangulClockUpdateManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const String UPDATE_CHECK_URL = "https://us-central1-hangul-clock.cloudfunctions.net/version/";
        private string downloadURL = "";

        public MainWindow()
        {
            InitializeComponent();

            hangulClockIcon.Source = UIKit.GetLogoImage();
            hangulClockCurrentVersion.Content = $"현재 설치된 버전 : {VersionKit.HANGULCLOCK_VERSION}";

            // checkUpdate();
        }

        private void checkUpdate()
        {
            new Thread(() =>
            {
                var DataKit = new DataKit();
                var hangulClockCommonSetting = new DataKit().Realm.All<HangulClockCommonSetting>();

                if (hangulClockCommonSetting.Count() <= 0)
                {
                    if (MessageBox.Show("업데이트를 확인할 수 없습니다.\n유효성 검사를 실패하였습니다.", "업데이트 확인 오류", MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK)
                    {
                        Environment.Exit(0);
                    }
                }
                else
                {
                    var hu = hangulClockCommonSetting.First().hu;

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UPDATE_CHECK_URL);
                    request.Headers["hu"] = hu;
                    request.Headers["platform"] = "windows";
                    request.Headers["version"] = VersionKit.HANGULCLOCK_VERSION;

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream);

                    string result = reader.ReadToEnd();

                    stream.Close();
                    response.Close();

                    JObject obj = JObject.Parse(result);

                    bool isUpdateAvailable = (bool)obj["isUpdateAvailable"];
                    string latestVersion = obj["version"].ToString();
                    downloadURL = obj["downloadURL"].ToString();
                    JArray changeLogs = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(obj["changeLog"]));

                    this.Dispatcher.Invoke(() =>
                    {
                        if (isUpdateAvailable)
                        {
                            newVersionAvailable.Content = $"새로운 버전 {latestVersion}으로 업데이트 할 수 있습니다.";

                            for (var i = 0; i < changeLogs.Count; i++)
                            {
                                updateComment.Text += $" - {changeLogs[i].ToString()}\n";
                            }
                        }
                        else
                        {
                            newVersionAvailable.Content = "현재 버전이 최신버전입니다.";
                            updateButton.IsEnabled = false;
                            updateComment.IsEnabled = false;
                        }

                        loadingIndicator.Visibility = Visibility.Hidden;
                    });
                }
            }).Start();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            checkUpdate();
        }

        private void updateButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            loadingIndicator.Visibility = Visibility.Visible;

            string tempPath = Path.GetTempPath();

            using (WebClient wc = new WebClient())
            {
                wc.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36";
                wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
                wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                wc.DownloadFileAsync(new Uri(downloadURL), $"{tempPath}/HangulClockInstaller.exe");
            }
        }

        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {

        }

        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Debug.WriteLine(e.Error);
            string tempPath = Path.GetTempPath();

            if (File.Exists($"{tempPath}/HangulClockInstaller.exe"))
            {
                try
                {
                    Process installerProcess = new Process();
                    installerProcess.StartInfo.FileName = $"{tempPath}/HangulClockInstaller.exe";
                    installerProcess.Start();

                    Application.Current.Shutdown();
                }
                catch (Exception ee)
                {
                    if (MessageBox.Show("업데이트를 다운로드할 수 없습니다.\n업데이트 파일이 손상되었습니다.", "업데이트 다운로드 오류", MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK)
                    {
                        Environment.Exit(0);
                    }
                }
            }
            else
            {
                if (MessageBox.Show("업데이트를 다운로드할 수 없습니다.\n업데이트 파일이 존재하지 않습니다.", "업데이트 다운로드 오류", MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    Environment.Exit(0);
                }
            }
        }
    }
}
