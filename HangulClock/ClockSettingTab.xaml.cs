using HangulClockDataKit;
using HangulClockDataKit.Model;
using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Drawing;
using System.Windows.Threading;

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
                string font = monitorSetting.FontName;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    clockColorToggle.IsChecked = isWhiteClock;
                    clockSizeSlider.Value = clockSize;
                    clockSizeValueText.Content = String.Format(CLOCK_SIZE, clockSizeSlider.Value);

                    try
                    {
                        var cvt = new FontConverter();
                        Font f = cvt.ConvertFromString(font) as Font;
                        clockFontValueText.Content = f.Name;
                    }catch
                    {
                        clockFontValueText.Content = "Noto Sans";
                    }
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

        private void clockFontValueText_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                var fontDialog= new System.Windows.Forms.FontDialog();
                fontDialog.ShowColor = false;
                fontDialog.ShowEffects = false;

                if (fontDialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
                {
                    var changedFont = fontDialog.Font.Name.ToString();
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate { clockFontValueText.Content = changedFont; }));
                    var DataKit = new DataKit();
                    var monitorSetting = (from c in DataKit.Realm.All<ClockSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();
                    var cvt = new FontConverter();
                    var fontSerial= cvt.ConvertToString(fontDialog.Font);
                    DataKit.Realm.Write(() =>
                    {
                        monitorSetting.FontName = fontSerial;
                    });
                }
            }).Start();
        }
    }
}
