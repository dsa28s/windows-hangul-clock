using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        
        private ClockSettingsByMonitor monitorSetting;

        private bool isDataLoaded = false;

        public ClockSettingTab()
        {
            InitializeComponent();
        }

        public void loadInitData()
        {
            // clockSizeSlider.Value = 50;
            // clockSizeValueText.Content = String.Format(CLOCK_SIZE, clockSizeSlider.Value);

            monitorSetting = MainWindow.loadMonitorPreferences(System.Windows.Forms.Screen.AllScreens[0].DeviceName);

            clockColorToggle.IsChecked = !monitorSetting.IsWhiteClock;
            clockSizeSlider.Value = monitorSetting.ClockSize;
            clockSizeValueText.Content = String.Format(CLOCK_SIZE, clockSizeSlider.Value);

            HangulClockUIKit.UIKit.Delay(1000);

            isDataLoaded = true;

            Debug.WriteLine(monitorSetting.ClockSize);
        }

        private void clockSizeSlider_ValueChanged(object sender, EventArgs e)
        {
            clockSizeValueText.Content = String.Format(CLOCK_SIZE, Convert.ToInt32(clockSizeSlider.Value));

            if (isDataLoaded)
            {
                DataKit.getInstance().getSharedRealms().Write(() =>
                {
                    monitorSetting.ClockSize = Convert.ToInt32(clockSizeSlider.Value);
                    Debug.WriteLine("OK");
                });
            }
        }

        private void clockColorToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (isDataLoaded)
            {
                DataKit.getInstance().getSharedRealms().Write(() =>
                {
                    monitorSetting.IsWhiteClock = !clockColorToggle.IsChecked ?? false;
                });
            }
        }
    }
}
