using HangulClockDataKit;
using HangulClockDataKit.Model;
using HangulClockUIKit;
using HangulClockUIKit.PageTransitions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace HangulClock
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public static UIKit.HangulClockTab activeTab = UIKit.HangulClockTab.DASHBOARD;

        public static PageTransition pager;

        public static DashboardTab dashboardTab = new DashboardTab();
        private ClockSettingTab clockSettingTab = new ClockSettingTab();
        private WallpaperSettingTab wallpaperSettingTab = new WallpaperSettingTab();
        private ThemeSettingTab themeSettingTab = new ThemeSettingTab();
        private CommentSettingTab commentSettingTab = new CommentSettingTab();
        private InformationTab informationTab = new InformationTab();
        private MultiMonitorSelectPage monitorTab = new MultiMonitorSelectPage();

        private static Label dashboardLabel = null;
        private static Label clockSettingLabel = null;
        private static Label wallpaperSettingLabel = null;
        private static Label commentSettingLabel = null;
        private static Label informationLabel = null;
        private static Label monitorLabel = null;

        private static Label toast = null;

        public static string MonitorDeviceName;

        public MainWindow()
        {
            InitializeComponent();

            KillMonitoringProcess();

            pager = pageController;

            dashboardLabel = tab_dashboard;
            clockSettingLabel = tab_clocksetting;
            wallpaperSettingLabel = tab_wallpaper;
            commentSettingLabel = tab_comment;
            informationLabel = tab_information;
            monitorLabel = tab_monitor;

            toast = toastMessage;

            Directory.CreateDirectory("C:\\Hangul Clock Configuration Files");
            DirectoryInfo di = new DirectoryInfo("C:\\Hangul Clock Configuration Files");
            if ((di.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
            {
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }

            setCurrentMonitor(System.Windows.Forms.Screen.AllScreens[0].DeviceName);
        }

        public async static void showToastMessage(string message)
        {
            toast.Content = message;

            toast.Visibility = Visibility.Visible;

            await Task.Delay(3000);

            toast.Visibility = Visibility.Hidden;
        }

        public static void setCurrentMonitor(string monitorName)
        {
            MonitorDeviceName = monitorName;
            monitorLabel.Content = String.Format("현재 모니터 설정 : {0}", MonitorDeviceName);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowTitlebar.AttachTitlebar(this, mainContent, CloseButton_MouseDown, MinimizeButton_MouseDown);
            this.MouseDown += MainWindow_MouseDown;

            logo.Source = UIKit.GetLogoImage();

            await InitalizeData();

            StartMonitoringProcess();

            dashboardTab.loadInitData();
            clockSettingTab.loadInitData();
            wallpaperSettingTab.loadInitData();
            commentSettingTab.loadInitData();

            await Task.Delay(1000);
            background.Source = BackgroundWallpaper.GetRandomDefaultImage();

            pageController.ShowPage(dashboardTab);
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                try
                {
                    this.DragMove();
                }
                catch (InvalidOperationException error)
                {

                }
            }
        }

        private void CloseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Application.Current.Shutdown();
            Environment.Exit(0);
        }

        private void MinimizeButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void tab_dashboard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            blurBackground(false);
            activeTab = UIKit.HangulClockTab.DASHBOARD;
            updateTabStatus();

            pageController.ShowPage(dashboardTab);
            dashboardTab.loadInitData();
        }

        private void tab_clocksetting_MouseDown(object sender, MouseButtonEventArgs e)
        {
            blurBackground(true);
            activeTab = UIKit.HangulClockTab.CLOCK_SETTINGS;
            updateTabStatus();

            clockSettingTab.loadInitData();
            pageController.ShowPage(clockSettingTab);
            clockSettingTab.loadInitData();
        }

        private void tab_wallpaper_MouseDown(object sender, MouseButtonEventArgs e)
        {
            blurBackground(true);
            activeTab = UIKit.HangulClockTab.WALLPAPER_SETTINGS;
            updateTabStatus();

            pageController.ShowPage(wallpaperSettingTab);
            wallpaperSettingTab.loadInitData();
        }

        private void tab_theme_MouseDown(object sender, MouseButtonEventArgs e)
        {
            blurBackground(true);
            activeTab = UIKit.HangulClockTab.THEME_SETTINGS;
            updateTabStatus();

            pageController.ShowPage(themeSettingTab);
        }

        private void tab_comment_MouseDown(object sender, MouseButtonEventArgs e)
        {
            blurBackground(true);
            activeTab = UIKit.HangulClockTab.COMMENT_SETTINGS;
            updateTabStatus();

            pageController.ShowPage(commentSettingTab);
            commentSettingTab.loadInitData();
        }

        private void tab_information_MouseDown(object sender, MouseButtonEventArgs e)
        {
            blurBackground(true);
            activeTab = UIKit.HangulClockTab.INFORMATION;
            updateTabStatus();

            pageController.ShowPage(informationTab);
        }

        private void tab_monitor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            blurBackground(true);
            activeTab = UIKit.HangulClockTab.MONITOR;
            updateTabStatus();

            pageController.ShowPage(monitorTab);
        }

        private void blurBackground(bool isBlury)
        {
            var blurStoryboard = (Storyboard)this.FindResource("blurBackgroundStoryboard");
            var unblurStoryboard = (Storyboard)this.FindResource("unblurBackgroundStoryboard");

            if (isBlury)
            {
                if (backgroundBlur.Radius == 0)
                {
                    blurStoryboard.Begin(background, true);
                }
            }
            else
            {
                if (backgroundBlur.Radius == 20)
                {
                    unblurStoryboard.Begin(background, true);
                }
            }
        }

        public static void updateTabStatus()
        {
            switch (activeTab)
            {
                case UIKit.HangulClockTab.DASHBOARD:
                    dashboardLabel.Opacity = 1;
                    clockSettingLabel.Opacity = 0.3;
                    wallpaperSettingLabel.Opacity = 0.3;
                    // tab_theme.Opacity = 0.3;
                    commentSettingLabel.Opacity = 0.3;
                    informationLabel.Opacity = 0.3;
                    monitorLabel.Opacity = 0.3;
                    break;
                case UIKit.HangulClockTab.CLOCK_SETTINGS:
                    dashboardLabel.Opacity = 0.3;
                    clockSettingLabel.Opacity = 1;
                    wallpaperSettingLabel.Opacity = 0.3;
                    // tab_theme.Opacity = 0.3;
                    commentSettingLabel.Opacity = 0.3;
                    informationLabel.Opacity = 0.3;
                    monitorLabel.Opacity = 0.3;
                    break;
                case UIKit.HangulClockTab.WALLPAPER_SETTINGS:
                    dashboardLabel.Opacity = 0.3;
                    clockSettingLabel.Opacity = 0.3;
                    wallpaperSettingLabel.Opacity = 1;
                    // tab_theme.Opacity = 0.3;
                    commentSettingLabel.Opacity = 0.3;
                    informationLabel.Opacity = 0.3;
                    monitorLabel.Opacity = 0.3;
                    break;
                case UIKit.HangulClockTab.THEME_SETTINGS:

                    break;
                case UIKit.HangulClockTab.COMMENT_SETTINGS:
                    dashboardLabel.Opacity = 0.3;
                    clockSettingLabel.Opacity = 0.3;
                    wallpaperSettingLabel.Opacity = 0.3;
                    // tab_theme.Opacity = 0.3;
                    commentSettingLabel.Opacity = 1;
                    informationLabel.Opacity = 0.3;
                    monitorLabel.Opacity = 0.3;
                    break;
                case UIKit.HangulClockTab.INFORMATION:
                    dashboardLabel.Opacity = 0.3;
                    clockSettingLabel.Opacity = 0.3;
                    wallpaperSettingLabel.Opacity = 0.3;
                    // tab_theme.Opacity = 0.3;
                    commentSettingLabel.Opacity = 0.3;
                    informationLabel.Opacity = 1;
                    monitorLabel.Opacity = 0.3;
                    break;
                case UIKit.HangulClockTab.MONITOR:
                    dashboardLabel.Opacity = 0.3;
                    clockSettingLabel.Opacity = 0.3;
                    wallpaperSettingLabel.Opacity = 0.3;
                    // tab_theme.Opacity = 0.3;
                    commentSettingLabel.Opacity = 0.3;
                    informationLabel.Opacity = 0.3;
                    monitorLabel.Opacity = 1;
                    break;
            }
        }

        private void mainContent_MouseMove(object sender, MouseEventArgs e)
        {
            pageController.Margin = new Thickness(e.GetPosition(pageController).X / 200, e.GetPosition(pageController).Y / 200, 0, 0);
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private Task InitalizeData()
        {
            var task = new Task(() =>
            {
                var DataKit = new DataKit();

                var hangulClockCommonSetting = DataKit.Realm.All<HangulClockCommonSetting>();
                if (hangulClockCommonSetting.Count() <= 0)
                {
                    var setting = new HangulClockCommonSetting();
                    setting.hu = RandomString(16);

                    DataKit.Realm.Write(() =>
                    {
                        DataKit.Realm.Add(setting);
                    });
                }

                var screens = System.Windows.Forms.Screen.AllScreens;

                foreach (var screen in screens)
                {
                    var clockSetting = from c in DataKit.Realm.All<ClockSettingsByMonitor>() where c.MonitorDeviceName == screen.DeviceName select c;
                    if (clockSetting.Count() <= 0)
                    {
                        var setting = new ClockSettingsByMonitor();

                        DataKit.Realm.Write(() =>
                        {
                            setting.IsWhiteClock = true;
                            setting.MonitorDeviceName = screen.DeviceName;
                            setting.ClockSize = 100;
                            setting.YoutubeURL = "";

                            DataKit.Realm.Add(setting);
                        });
                    }

                    var commentSetting = from c in DataKit.Realm.All<CommentSettingsByMonitor>() where c.MonitorDeviceName == screen.DeviceName select c;
                    if (commentSetting.Count() <= 0)
                    {
                        var setting = new CommentSettingsByMonitor();

                        DataKit.Realm.Write(() =>
                        {
                            setting.MonitorDeviceName = screen.DeviceName;
                            setting.IsEnabled = false;
                            setting.IsEnabledLoadFromServer = false;

                            setting.Name = "";
                            setting.Comment = "";

                            DataKit.Realm.Add(setting);
                        });
                    }

                    var backgroundSetting = from c in DataKit.Realm.All<BackgroundSettingsByMonitor>() where c.MonitorDeviceName == screen.DeviceName select c;
                    if (backgroundSetting.Count() <= 0)
                    {
                        var setting = new BackgroundSettingsByMonitor();

                        DataKit.Realm.Write(() =>
                        {
                            setting.MonitorDeviceName = screen.DeviceName;
                            setting.backgroundType = BackgroundSettingsByMonitor.BackgroundType.DEFAULT;

                            DataKit.Realm.Add(setting);
                        });
                    }
                }
            });

            task.Start();
            return task;
        }

        private void StartMonitoringProcess()
        {
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo("HangulClockMonitoringProcess");
            // p.StartInfo.UseShellExecute = true;
            p.StartInfo.CreateNoWindow = true;

            p.Start();
        }

        private void KillMonitoringProcess()
        {
            Process[] hangulClockMonitoringProcesses = Process.GetProcessesByName("HangulClockMonitoringProcess");

            try
            {
                foreach (var hangulClockMonitoringProcess in hangulClockMonitoringProcesses)
                {
                    hangulClockMonitoringProcess.Kill();
                }
            }
            catch (Exception e) { }
        }
    }
}
