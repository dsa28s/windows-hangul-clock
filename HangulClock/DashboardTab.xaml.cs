using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

namespace HangulClock
{
    /// <summary>
    /// DashboardTab.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DashboardTab : UserControl
    {
        public DashboardTab()
        {
            InitializeComponent();
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

            await Task.Delay(3000);

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
