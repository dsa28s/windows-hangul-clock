using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace HangulClockUninstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string UNINSTALLER_URL = "https://storage.googleapis.com/hangulclock-bucket/uninstaller/HangulClockUninstaller.exe";
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            string tempPath = Path.GetTempPath();

            using (WebClient wc = new WebClient())
            {
                wc.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36";
                wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
                wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                wc.DownloadFileAsync(new Uri(UNINSTALLER_URL), $"{tempPath}/HangulClockUninstaller.exe");
            }
        }

        private void Wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            string tempPath = Path.GetTempPath();

            if (File.Exists($"{tempPath}/HangulClockUninstaller.exe"))
            {
                try
                {
                    Process installerProcess = new Process();
                    installerProcess.StartInfo.FileName = $"{tempPath}/HangulClockUninstaller.exe";
                    installerProcess.Start();

                    Application.Current.Shutdown();
                }
                catch (Exception ee)
                {
                    if (MessageBox.Show("한글시계 제거 프로그램을 실행할 수 없습니다.", "한글시계 제거 프로그램", MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK)
                    {
                        Environment.Exit(0);
                    }
                }
            }
            else
            {
                if (MessageBox.Show("한글시계 제거 프로그램을 실행할 수 없습니다. 알 수 없는 오류입니다.", "한글시계 제거 프로그램", MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    Environment.Exit(0);
                }
            }
        }

        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            
        }
    }
}
