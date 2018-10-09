using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using Microsoft.Win32;

namespace HangulClock
{
    /// <summary>
    /// DashboardTab.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WallpaperSettingTab : UserControl
    {
        public WallpaperSettingTab()
        {
            InitializeComponent();
        }

        public void loadInitData()
        {
            new Thread(() =>
            {
                var DataKit = new DataKit();
                var monitorSettingQuery = from c in DataKit.Realm.All<BackgroundSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c;
                BackgroundSettingsByMonitor monitorSetting = monitorSettingQuery.First();

                int backgroundType = monitorSetting.backgroundType;
                string imgPath = monitorSetting.imgPath;
                string solidColor = monitorSetting.SolidColor;
                string youtubeVideoCode = monitorSetting.YoutubeURL;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    backgroundToggle.IsChecked = backgroundType == BackgroundSettingsByMonitor.BackgroundType.STILL_IMAGE;
                    solidToggle.IsChecked = backgroundType == BackgroundSettingsByMonitor.BackgroundType.SOLID;
                    youtubeToggle.IsChecked = backgroundType == BackgroundSettingsByMonitor.BackgroundType.YOUTUBE_VIDEO;

                    if (imgPath != "" && imgPath != null)
                    {
                        if (File.Exists(imgPath))
                        {
                            stillImage.Source = new BitmapImage(new Uri(imgPath));
                            stillImage.Stretch = Stretch.UniformToFill;
                        }
                    }
                    else
                    {
                        stillImage.Source = null;
                    }

                    BrushConverter bc = new BrushConverter();
                    if (solidColor != null && solidColor != "")
                    {
                        solidColorContainer.Background = (Brush)bc.ConvertFrom("#" + solidColor);
                    }
                    else
                    {
                        solidColorContainer.Background = (Brush)bc.ConvertFrom("#10FFFFFF");
                    }

                    youtubeVideoCodeTextBox.Text = youtubeVideoCode;

                    if (!backgroundToggle.IsChecked ?? false)
                    {
                        stillImageBorder.Opacity = 0.3;
                    }
                    else
                    {
                        stillImageBorder.Opacity = 1;
                    }

                    if (!solidToggle.IsChecked ?? false)
                    {
                        solidImageBorder.Opacity = 0.3;
                    }
                    else
                    {
                        solidImageBorder.Opacity = 1;
                    }

                    if (!youtubeToggle.IsChecked ?? false)
                    {
                        youtubeVideoCodeTextBox.Opacity = 0.3;
                        youtubeVideoCodeTextBox.IsEnabled = false;
                    }
                    else
                    {
                        youtubeVideoCodeTextBox.Opacity = 1;
                        youtubeVideoCodeTextBox.IsEnabled = true;
                    }
                }));
            }).Start();
        }

        private void backgroundToggle_Checked(object sender, RoutedEventArgs e)
        {
            stillImageBorder.Opacity = 1;

            new Thread(() =>
            {
                var DataKit = new DataKit();
                var monitorSetting = (from c in DataKit.Realm.All<BackgroundSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();
                DataKit.Realm.Write(() =>
                {
                    monitorSetting.backgroundType = BackgroundSettingsByMonitor.BackgroundType.STILL_IMAGE;
                });
            }).Start();

            solidToggle.IsChecked = false;
            youtubeToggle.IsChecked = false;
        }

        private void backgroundToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            stillImageBorder.Opacity = 0.3;

            checkInitBackground();
        }

        private void solidToggle_Checked(object sender, RoutedEventArgs e)
        {
            solidImageBorder.Opacity = 1;

            new Thread(() =>
            {
                var DataKit = new DataKit();
                var monitorSetting = (from c in DataKit.Realm.All<BackgroundSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();
                DataKit.Realm.Write(() =>
                {
                    monitorSetting.backgroundType = BackgroundSettingsByMonitor.BackgroundType.SOLID;
                });
            }).Start();

            backgroundToggle.IsChecked = false;
            youtubeToggle.IsChecked = false;
        }

        private void solidToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            solidImageBorder.Opacity = 0.3;

            checkInitBackground();
        }

        private void youtubeToggle_Checked(object sender, RoutedEventArgs e)
        {
            youtubeVideoCodeTextBox.Opacity = 1;
            youtubeVideoCodeTextBox.IsEnabled = true;

            new Thread(() =>
            {
                var DataKit = new DataKit();
                var monitorSetting = (from c in DataKit.Realm.All<BackgroundSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();

                DataKit.Realm.Write(() =>
                {
                    monitorSetting.backgroundType = BackgroundSettingsByMonitor.BackgroundType.YOUTUBE_VIDEO;
                });
            }).Start();

            backgroundToggle.IsChecked = false;
            solidToggle.IsChecked = false;
        }

        private void youtubeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            youtubeVideoCodeTextBox.Opacity = 0.3;
            youtubeVideoCodeTextBox.IsEnabled = false;

            checkInitBackground();
        }

        private void stillImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (backgroundToggle.IsChecked ?? false)
            {
                if (e.ClickCount == 2)
                {
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.Filter = "이미지 파일 (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";

                    if (dialog.ShowDialog() ?? false)
                    {
                        string fileName = dialog.FileName;

                        stillImage.Source = new BitmapImage(new Uri(fileName));
                        stillImage.Stretch = Stretch.UniformToFill;

                        new Thread(() =>
                        {
                            var DataKit = new DataKit();
                            var monitorSetting = (from c in DataKit.Realm.All<BackgroundSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();

                            DataKit.Realm.Write(() =>
                            {
                                monitorSetting.imgPath = fileName;
                            });
                        }).Start();
                    }
                }
            }
        }

        private void stillImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("OK");
        }

        private void checkInitBackground()
        {
            if (!(backgroundToggle.IsChecked ?? false) && !(solidToggle.IsChecked ?? false) && !(youtubeToggle.IsChecked ?? false))
            {
                new Thread(() =>
                {
                    var DataKit = new DataKit();
                    var monitorSetting = (from c in DataKit.Realm.All<BackgroundSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();

                    DataKit.Realm.Write(() =>
                    {
                        monitorSetting.backgroundType = BackgroundSettingsByMonitor.BackgroundType.DEFAULT;
                    });
                }).Start();

                MainWindow.showToastMessage("배경화면 설정이 모두 해제되었습니다. '시스템에 설정된 배경화면' 으로 자동 설정됩니다.");
            }
        }

        private void solidColorContainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (solidToggle.IsChecked ?? false)
            {
                if (e.ClickCount == 2)
                {
                    System.Windows.Forms.ColorDialog dialog = new System.Windows.Forms.ColorDialog();

                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        int argb = dialog.Color.ToArgb();

                        new Thread(() =>
                        {
                            var DataKit = new DataKit();
                            var monitorSetting = (from c in DataKit.Realm.All<BackgroundSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();

                            DataKit.Realm.Write(() =>
                            {
                                monitorSetting.SolidColor = (argb & 0x00FFFFFF).ToString("X6");
                            });
                        }).Start();

                        BrushConverter bc = new BrushConverter();
                        solidColorContainer.Background = (Brush)bc.ConvertFrom("#" + (argb & 0x00FFFFFF).ToString("X6"));
                        Debug.WriteLine("OKOKOKOKOK");
                    }
                }
            }
        }

        private void youtubeVideoCodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string youtubeVideoCode = youtubeVideoCodeTextBox.Text;
            new Thread(() =>
            {
                var DataKit = new DataKit();
                var monitorSetting = (from c in DataKit.Realm.All<BackgroundSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();

                DataKit.Realm.Write(() =>
                {
                    monitorSetting.YoutubeURL = youtubeVideoCode;
                });
            }).Start();
        }
    }
}
