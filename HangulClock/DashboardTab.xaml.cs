using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using HangulClockDataKit;
using HangulClockDataKit.Model;
using HangulClockKit;
using Newtonsoft.Json.Linq;

namespace HangulClock
{
    /// <summary>
    /// DashboardTab.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DashboardTab : UserControl
    {
        private const String UPDATE_CHECK_URL = "https://us-central1-hangul-clock.cloudfunctions.net/version/";
        private System.Timers.Timer updateDelayTimer;

        public DashboardTab()
        {
            InitializeComponent();

            updateText.Content = "업데이트를 확인하는 중...";

            updateDelayTimer = new System.Timers.Timer(3000);
            updateDelayTimer.Elapsed += UpdateDelayTimer_Elapsed;
            updateDelayTimer.Start();

            versionText.Content = $"한글시계 v {VersionKit.HANGULCLOCK_VERSION} (for Windows)";
        }

        private void UpdateDelayTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            updateDelayTimer.Stop();

            new Thread(() =>
            {
                var hangulClockCommonSetting = new DataKit().Realm.All<HangulClockCommonSetting>();

                if (hangulClockCommonSetting.Count() <= 0)
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        updateText.Content = "업데이트 확인 실패.";
                    }));
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

                    this.Dispatcher.Invoke(() =>
                    {
                        if (isUpdateAvailable)
                        {
                            updateText.Content = "업데이트 있음.";

                            Process updateProcess = new Process();
                            updateProcess.StartInfo.FileName = "HangulClockUpdateManager.exe";
                            updateProcess.Start();
                        }
                        else
                        {
                            updateText.Content = "업데이트 없음. 최신버전.";
                        }
                    });
                }
            }).Start();
        }

        public async void loadInitData()
        {
            /* if (hangulClockONOFFToggle.IsChecked == true)
            {
                useText.Content = "사용중이야.";
            }
            else
            {
                useText.Content = "사용중이지 않아.";
            }

            Process[] hangulClockRendererProcesses = Process.GetProcessesByName("HangulClockRenderer");

            if (hangulClockRendererProcesses.Length > 0)
            {
                useText.Content = "사용중이야.";
                hangulClockONOFFToggle.IsChecked = true;
            }
            else
            {
                useText.Content = "사용중이지 않아.";
                hangulClockONOFFToggle.IsChecked = false;
            } */

            MainWindow.showToastMessage("해당 모니터에 해당하는 한글시계 설정값을 구성하고 있습니다...");

            await Task.Delay(1000);

            new Thread(() =>
            {
                var DataKit = new DataKit();

                var clockSetting = DataKit.Realm.All<ClockSettingsByMonitor>().Where(c => c.MonitorDeviceName == MainWindow.MonitorDeviceName).First();

                bool isEnabledClockCurrentMonitor = clockSetting.isUseHangulClock;

                this.Dispatcher.Invoke(() =>
                {
                    hangulClockONOFFToggle.IsChecked = isEnabledClockCurrentMonitor;

                    useText.Content = isEnabledClockCurrentMonitor ? "사용중이야." : "사용중이지 않아.";
                });
            }).Start();
        }

        private void hangulClockONOFFToggle_Checked(object sender, RoutedEventArgs e)
        {
            useText.Content = "사용중이야.";

            new Thread(() =>
            {
                var DataKit = new DataKit();
                var clockSetting = DataKit.Realm.All<ClockSettingsByMonitor>().Where(c => c.MonitorDeviceName == MainWindow.MonitorDeviceName).First();

                DataKit.Realm.Write(() =>
                {
                    clockSetting.isUseHangulClock = true;
                });

                this.Dispatcher.Invoke(() =>
                {
                    MainWindow.showToastMessage("해당 모니터에서 한글시계를 사용합니다.");
                });
            }).Start();
        }

        private void hangulClockONOFFToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            useText.Content = "사용중이지 않아.";

            new Thread(() =>
            {
                var DataKit = new DataKit();
                var clockSetting = DataKit.Realm.All<ClockSettingsByMonitor>().Where(c => c.MonitorDeviceName == MainWindow.MonitorDeviceName).First();

                DataKit.Realm.Write(() =>
                {
                    clockSetting.isUseHangulClock = false;

                    this.Dispatcher.Invoke(() =>
                    {
                        MainWindow.showToastMessage("해당 모니터에서 한글시계를 사용하지 않습니다.");
                    });
                });
            }).Start();
        }
    }
}
