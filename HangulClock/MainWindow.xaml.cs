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

using HangulClockUIKit;
using HangulClockUIKit.PageTransitions;
using System.Windows.Media.Animation;
using System.IO;
using HangulClockDataKit.Model;
using HangulClockDataKit;

namespace HangulClock
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private UIKit.HangulClockTab activeTab = UIKit.HangulClockTab.DASHBOARD;

        public static PageTransition pager;

        private DashboardTab dashboardTab = new DashboardTab();
        private ClockSettingTab clockSettingTab = new ClockSettingTab();
        private WallpaperSettingTab wallpaperSettingTab = new WallpaperSettingTab();
        private ThemeSettingTab themeSettingTab = new ThemeSettingTab();
        private CommentSettingTab commentSettingTab = new CommentSettingTab();
        private InformationTab informationTab = new InformationTab();
        private MultiMonitorSelectPage monitorTab = new MultiMonitorSelectPage();

        private static Label tabMonitor = null;
        private static string MonitorDeviceName;

        public MainWindow()
        {
            InitializeComponent();

            pager = pageController;
            tabMonitor = tab_monitor;

            Directory.CreateDirectory("C:\\Hangul Clock Configuration Files");

            DirectoryInfo di = new DirectoryInfo("C:\\Hangul Clock Configuration Files");
            if ((di.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
            {
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
        }

        public static ClockSettingsByMonitor loadMonitorPreferences(string monitorDeviceName)
        {
            MonitorDeviceName = monitorDeviceName;
            tabMonitor.Content = String.Format("현재 모니터 설정 : {0}", monitorDeviceName);

            var monitorSettingQuery = DataKit.getInstance().getSharedRealms().All<ClockSettingsByMonitor>().Where(c => c.MonitorDeviceName == monitorDeviceName);

            if (monitorSettingQuery.Count() > 0)
            {
                return monitorSettingQuery.First();
            }
            else
            {
                var monitor1Config = new ClockSettingsByMonitor();

                DataKit.getInstance().getSharedRealms().Write(() =>
                {
                    monitor1Config.IsWhiteClock = true;
                    monitor1Config.MonitorDeviceName = monitorDeviceName;
                    monitor1Config.ClockSize = 100;
                    monitor1Config.YoutubeURL = "";

                    DataKit.getInstance().getSharedRealms().Add(monitor1Config);
                });

                return monitor1Config;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowTitlebar.AttachTitlebar(this, mainContent, CloseButton_MouseDown, MinimizeButton_MouseDown);
            this.MouseDown += MainWindow_MouseDown;

            logo.Source = UIKit.GetLogoImage();

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
            Application.Current.Shutdown();
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

            dashboardTab.loadInitData();
            pageController.ShowPage(dashboardTab);
        }

        private void tab_clocksetting_MouseDown(object sender, MouseButtonEventArgs e)
        {
            blurBackground(true);
            activeTab = UIKit.HangulClockTab.CLOCK_SETTINGS;
            updateTabStatus();

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

        private void updateTabStatus()
        {
            switch(activeTab)
            {
                case UIKit.HangulClockTab.DASHBOARD:
                    tab_dashboard.Opacity = 1;
                    tab_clocksetting.Opacity = 0.3;
                    tab_wallpaper.Opacity = 0.3;
                    // tab_theme.Opacity = 0.3;
                    tab_comment.Opacity = 0.3;
                    tab_information.Opacity = 0.3;
                    tab_monitor.Opacity = 0.3;
                    break;
                case UIKit.HangulClockTab.CLOCK_SETTINGS:
                    tab_dashboard.Opacity = 0.3;
                    tab_clocksetting.Opacity = 1;
                    tab_wallpaper.Opacity = 0.3;
                    // tab_theme.Opacity = 0.3;
                    tab_comment.Opacity = 0.3;
                    tab_information.Opacity = 0.3;
                    tab_monitor.Opacity = 0.3;
                    break;
                case UIKit.HangulClockTab.WALLPAPER_SETTINGS:
                    tab_dashboard.Opacity = 0.3;
                    tab_clocksetting.Opacity = 0.3;
                    tab_wallpaper.Opacity = 1;
                    // tab_theme.Opacity = 0.3;
                    tab_comment.Opacity = 0.3;
                    tab_information.Opacity = 0.3;
                    tab_monitor.Opacity = 0.3;
                    break;
                case UIKit.HangulClockTab.THEME_SETTINGS:
                    tab_dashboard.Opacity = 0.3;
                    tab_clocksetting.Opacity = 0.3;
                    tab_wallpaper.Opacity = 0.3;
                    // tab_theme.Opacity = 1;
                    tab_comment.Opacity = 0.3;
                    tab_information.Opacity = 0.3;
                    tab_monitor.Opacity = 0.3;
                    break;
                case UIKit.HangulClockTab.COMMENT_SETTINGS:
                    tab_dashboard.Opacity = 0.3;
                    tab_clocksetting.Opacity = 0.3;
                    tab_wallpaper.Opacity = 0.3;
                    // tab_theme.Opacity = 0.3;
                    tab_comment.Opacity = 1;
                    tab_information.Opacity = 0.3;
                    tab_monitor.Opacity = 0.3;
                    break;
                case UIKit.HangulClockTab.INFORMATION:
                    tab_dashboard.Opacity = 0.3;
                    tab_clocksetting.Opacity = 0.3;
                    tab_wallpaper.Opacity = 0.3;
                    // tab_theme.Opacity = 0.3;
                    tab_comment.Opacity = 0.3;
                    tab_information.Opacity = 1;
                    tab_monitor.Opacity = 0.3;
                    break;
                case UIKit.HangulClockTab.MONITOR:
                    tab_dashboard.Opacity = 0.3;
                    tab_clocksetting.Opacity = 0.3;
                    tab_wallpaper.Opacity = 0.3;
                    // tab_theme.Opacity = 0.3;
                    tab_comment.Opacity = 0.3;
                    tab_information.Opacity = 0.3;
                    tab_monitor.Opacity = 1;
                    break;
            }
        }

        private void mainContent_MouseMove(object sender, MouseEventArgs e)
        {
            pageController.Margin = new Thickness(e.GetPosition(pageController).X / 200, e.GetPosition(pageController).Y / 200, 0, 0);
        }
    }
}
