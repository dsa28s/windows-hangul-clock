using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Shapes;

using HangulClockDataKit;
using HangulClockDataKit.Model;

namespace HangulClock
{
    /// <summary>
    /// DashboardTab.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ClockSettingTab : UserControl
    {
        private static String CLOCK_SIZE = "시계 크기 ({0}%)";
        private MultiMonitorSelectPage mmPage;

        private string MonitorDeviceName;

        public ClockSettingTab()
        {
            InitializeComponent();
            mmPage = new MultiMonitorSelectPage();
        }

        public void loadInitData()
        {
            clockSizeSlider.Value = 50;
            clockSizeValueText.Content = String.Format(CLOCK_SIZE, clockSizeSlider.Value);

            MonitorDeviceName = System.Windows.Forms.Screen.AllScreens[0].DeviceName;

            currentMonitorSettingText.Text = String.Format("현재 모니터 설정 : {0}", MonitorDeviceName);

            var monitorSetting = DataKit.getInstance().getSharedRealms().All<ClockSettingsByMonitor>().Where(c => c.MonitorDeviceName == MonitorDeviceName);

            if (monitorSetting.Count() > 0)
            {
                var monitor1Config = monitorSetting.First();

                clockColorToggle.IsChecked = !monitor1Config.IsWhiteClock;
                clockSizeSlider.Value = monitor1Config.ClockSize;
                // MessageBox.Show("OK");
            }
            else
            {
                DataKit.getInstance().getSharedRealms().Write(() =>
                {
                    var monitor1Config = new ClockSettingsByMonitor();
                    monitor1Config.IsWhiteClock = true;
                    monitor1Config.MonitorDeviceName = MonitorDeviceName;
                    monitor1Config.ClockSize = 100;
                    monitor1Config.YoutubeURL = "";

                    DataKit.getInstance().getSharedRealms().Add(monitor1Config);
                });
            }
        }

        private void clockSizeSlider_ValueChanged(object sender, EventArgs e)
        {
            clockSizeValueText.Content = String.Format(CLOCK_SIZE, Convert.ToInt32(clockSizeSlider.Value));
        }

        private void externalDisplaySettingButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.pager.ShowPage(mmPage);
        }
    }
}
