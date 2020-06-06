﻿using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace HangulClock
{
    /// <summary>
    /// DashboardTab.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MultiMonitorSelectPage : UserControl
    {
        public MultiMonitorSelectPage()
        {
            InitializeComponent();

            display1.Visibility = Visibility.Hidden;
            display1_Text.Visibility = Visibility.Hidden;
            display1_Border.Visibility = Visibility.Hidden;
            display2.Visibility = Visibility.Hidden;
            display2_Text.Visibility = Visibility.Hidden;
            display2_Border.Visibility = Visibility.Hidden;
            display3.Visibility = Visibility.Hidden;
            display3_Text.Visibility = Visibility.Hidden;
            display3_Border.Visibility = Visibility.Hidden;
            display4.Visibility = Visibility.Hidden;
            display4_Text.Visibility = Visibility.Hidden;
            display4_Border.Visibility = Visibility.Hidden;
            display5.Visibility = Visibility.Hidden;
            display5_Text.Visibility = Visibility.Hidden;
            display5_Border.Visibility = Visibility.Hidden;
            display6.Visibility = Visibility.Hidden;
            display6_Text.Visibility = Visibility.Hidden;
            display6_Border.Visibility = Visibility.Hidden;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var index = 0;
            foreach (System.Windows.Forms.Screen s in System.Windows.Forms.Screen.AllScreens)
            {
                switch (index)
                {
                    case 0:
                        display1.Visibility = Visibility.Visible;
                        display1_Text.Visibility = Visibility.Visible;
                        display1_Border.Visibility = Visibility.Visible;

                        display1_Text.Content = (index + 1) + s.DeviceName + ((s.Primary) ? " (주 모니터)" : "");

                        renderCapturedImage(display1, s);

                        break;
                    case 1:
                        display2.Visibility = Visibility.Visible;
                        display2_Text.Visibility = Visibility.Visible;
                        display2_Border.Visibility = Visibility.Visible;

                        display2_Text.Content = s.DeviceName + ((s.Primary) ? " (주 모니터)" : "");

                        renderCapturedImage(display2, s);
                        break;
                    case 2:
                        display3.Visibility = Visibility.Visible;
                        display3_Text.Visibility = Visibility.Visible;
                        display3_Border.Visibility = Visibility.Visible;

                        display3_Text.Content = s.DeviceName + ((s.Primary) ? " (주 모니터)" : "");

                        renderCapturedImage(display3, s);
                        break;
                    case 3:
                        display4.Visibility = Visibility.Visible;
                        display4_Text.Visibility = Visibility.Visible;
                        display4_Border.Visibility = Visibility.Visible;

                        display4_Text.Content = s.DeviceName + ((s.Primary) ? " (주 모니터)" : "");

                        renderCapturedImage(display4, s);
                        break;
                    case 4:
                        display5.Visibility = Visibility.Visible;
                        display5_Text.Visibility = Visibility.Visible;
                        display5_Border.Visibility = Visibility.Visible;

                        display5_Text.Content = s.DeviceName + ((s.Primary) ? " (주 모니터)" : "");

                        renderCapturedImage(display5, s);
                        break;
                    case 5:
                        display6.Visibility = Visibility.Visible;
                        display6_Text.Visibility = Visibility.Visible;
                        display6_Border.Visibility = Visibility.Visible;

                        display6_Text.Content = s.DeviceName + ((s.Primary) ? " (주 모니터)" : "");

                        renderCapturedImage(display6, s);
                        break;
                }

                index++;
            }
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void renderCapturedImage(System.Windows.Controls.Image i, System.Windows.Forms.Screen s)
        {
            System.Drawing.Size size = s.Bounds.Size;
            Bitmap b = new Bitmap(s.Bounds.Width, s.Bounds.Height);
            Graphics g = Graphics.FromImage(b as System.Drawing.Image);
            g.CopyFromScreen(s.Bounds.X, s.Bounds.Y, 0, 0, size);

            i.Source = BitmapToImageSource(b);
        }

        private void display1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (System.Windows.Forms.Screen.AllScreens.Length > 0)
            {
                MainWindow.setCurrentMonitor(System.Windows.Forms.Screen.AllScreens[0].DeviceName);
                MainWindow.activeTab = HangulClockUIKit.UIKit.HangulClockTab.DASHBOARD;
                MainWindow.updateTabStatus();
                MainWindow.pager.ShowPage(MainWindow.dashboardTab);
                MainWindow.dashboardTab.loadInitData();
            }
        }

        private void display2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (System.Windows.Forms.Screen.AllScreens.Length > 1)
            {
                MainWindow.setCurrentMonitor(System.Windows.Forms.Screen.AllScreens[1].DeviceName);
                MainWindow.activeTab = HangulClockUIKit.UIKit.HangulClockTab.DASHBOARD;
                MainWindow.updateTabStatus();
                MainWindow.pager.ShowPage(MainWindow.dashboardTab);
                MainWindow.dashboardTab.loadInitData();
            }
        }

        private void display3_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (System.Windows.Forms.Screen.AllScreens.Length > 2)
            {
                MainWindow.setCurrentMonitor(System.Windows.Forms.Screen.AllScreens[2].DeviceName);
                MainWindow.activeTab = HangulClockUIKit.UIKit.HangulClockTab.DASHBOARD;
                MainWindow.updateTabStatus();
                MainWindow.pager.ShowPage(MainWindow.dashboardTab);
                MainWindow.dashboardTab.loadInitData();
            }
        }

        private void display4_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (System.Windows.Forms.Screen.AllScreens.Length > 3)
            {
                MainWindow.setCurrentMonitor(System.Windows.Forms.Screen.AllScreens[3].DeviceName);
                MainWindow.activeTab = HangulClockUIKit.UIKit.HangulClockTab.DASHBOARD;
                MainWindow.updateTabStatus();
                MainWindow.pager.ShowPage(MainWindow.dashboardTab);
                MainWindow.dashboardTab.loadInitData();
            }
        }

        private void display5_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (System.Windows.Forms.Screen.AllScreens.Length > 4)
            {
                MainWindow.setCurrentMonitor(System.Windows.Forms.Screen.AllScreens[4].DeviceName);
                MainWindow.activeTab = HangulClockUIKit.UIKit.HangulClockTab.DASHBOARD;
                MainWindow.updateTabStatus();
                MainWindow.pager.ShowPage(MainWindow.dashboardTab);
                MainWindow.dashboardTab.loadInitData();
            }
        }

        private void display6_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (System.Windows.Forms.Screen.AllScreens.Length > 5)
            {
                MainWindow.setCurrentMonitor(System.Windows.Forms.Screen.AllScreens[5].DeviceName);
                MainWindow.activeTab = HangulClockUIKit.UIKit.HangulClockTab.DASHBOARD;
                MainWindow.updateTabStatus();
                MainWindow.pager.ShowPage(MainWindow.dashboardTab);
                MainWindow.dashboardTab.loadInitData();
            }
        }
    }
}
