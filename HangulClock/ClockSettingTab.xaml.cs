using HangulClockDataKit;
using HangulClockDataKit.Model;
using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace HangulClock
{
    /// <summary>
    /// DashboardTab.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ClockSettingTab : UserControl
    {
        private static String CLOCK_SIZE = "시계 크기 ({0}%)";

        public ClockSettingTab()
        {
            InitializeComponent();
        }

        public void loadInitData()
        {
            // clockSizeSlider.Value = 50;
            // clockSizeValueText.Content = String.Format(CLOCK_SIZE, clockSizeSlider.Value);

            new Thread(() =>
            {
                var DataKit = new DataKit();
                var monitorSettingQuery = from c in DataKit.Realm.All<ClockSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c;
                ClockSettingsByMonitor monitorSetting = monitorSettingQuery.First();

                bool isWhiteClock = !monitorSetting.IsWhiteClock;
                int clockSize = monitorSetting.ClockSize;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    clockColorToggle.IsChecked = isWhiteClock;
                    clockSizeSlider.Value = clockSize;
                    clockSizeValueText.Content = String.Format(CLOCK_SIZE, clockSizeSlider.Value);
                }));
            }).Start();
        }

        private void clockSizeSlider_ValueChanged(object sender, EventArgs e)
        {
            int clockSize = Convert.ToInt32(clockSizeSlider.Value);

            clockSizeValueText.Content = String.Format(CLOCK_SIZE, clockSize);

            new Thread(() =>
            {
                var DataKit = new DataKit();
                var monitorSetting = (from c in DataKit.Realm.All<ClockSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();

                DataKit.Realm.Write(() =>
                {
                    monitorSetting.ClockSize = clockSize;
                });
            }).Start();
        }

        private void clockColorToggle_Checked(object sender, RoutedEventArgs e)
        {
            bool isToggleChecked = !clockColorToggle.IsChecked ?? false;

            new Thread(() =>
            {
                var DataKit = new DataKit();
                var monitorSetting = (from c in DataKit.Realm.All<ClockSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();

                DataKit.Realm.Write(() =>
                {
                    monitorSetting.IsWhiteClock = isToggleChecked;
                });
            }).Start();
        }

        private void clockColorToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            bool isToggleChecked = !clockColorToggle.IsChecked ?? false;

            new Thread(() =>
            {
                var DataKit = new DataKit();
                var monitorSetting = (from c in DataKit.Realm.All<ClockSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();

                DataKit.Realm.Write(() =>
                {
                    monitorSetting.IsWhiteClock = isToggleChecked;
                });
            }).Start();
        }
    }
}
