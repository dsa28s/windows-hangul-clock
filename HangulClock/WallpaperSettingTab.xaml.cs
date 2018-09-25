using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using Microsoft.Win32;

namespace HangulClock
{
    /// <summary>
    /// DashboardTab.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WallpaperSettingTab : UserControl
    {
        private BackgroundSettingsByMonitor monitorSetting;

        public WallpaperSettingTab()
        {
            InitializeComponent();
        }

        public void loadInitData()
        {
            monitorSetting = MainWindow.loadBackgroundPreferences();

            backgroundToggle.IsChecked = monitorSetting.backgroundType == BackgroundSettingsByMonitor.BackgroundType.STILL_IMAGE;
            solidToggle.IsChecked = monitorSetting.backgroundType == BackgroundSettingsByMonitor.BackgroundType.SOLID;
            youtubeToggle.IsChecked = monitorSetting.backgroundType == BackgroundSettingsByMonitor.BackgroundType.YOUTUBE_VIDEO;

            if (monitorSetting.imgPath != "" && monitorSetting.imgPath != null)
            {
                if (File.Exists(monitorSetting.imgPath))
                {
                    stillImage.Source = new BitmapImage(new Uri(monitorSetting.imgPath));
                    stillImage.Stretch = Stretch.UniformToFill;
                }
            }
            else
            {
                stillImage.Source = null;
            }

            if (monitorSetting.SolidColor != null && monitorSetting.SolidColor != "")
            {
                BrushConverter bc = new BrushConverter();
                solidColorContainer.Background = (Brush)bc.ConvertFrom("#" + monitorSetting.SolidColor);
            }

            youtubeVideoCodeTextBox.Text = monitorSetting.YoutubeURL;

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
        }

        private void backgroundToggle_Checked(object sender, RoutedEventArgs e)
        {
            stillImageBorder.Opacity = 1;

            DataKit.getInstance().getSharedRealms().Write(() =>
            {
                monitorSetting.backgroundType = BackgroundSettingsByMonitor.BackgroundType.STILL_IMAGE;
            });

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

            DataKit.getInstance().getSharedRealms().Write(() =>
            {
                monitorSetting.backgroundType = BackgroundSettingsByMonitor.BackgroundType.SOLID;
            });

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

            DataKit.getInstance().getSharedRealms().Write(() =>
            {
                monitorSetting.backgroundType = BackgroundSettingsByMonitor.BackgroundType.YOUTUBE_VIDEO;
            });

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
                        stillImage.Source = new BitmapImage(new Uri(dialog.FileName));
                        stillImage.Stretch = Stretch.UniformToFill;

                        DataKit.getInstance().getSharedRealms().Write(() =>
                        {
                            monitorSetting.imgPath = dialog.FileName;
                        });
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
                DataKit.getInstance().getSharedRealms().Write(() =>
                {
                    monitorSetting.backgroundType = BackgroundSettingsByMonitor.BackgroundType.DEFAULT;
                });

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
                        DataKit.getInstance().getSharedRealms().Write(() =>
                        {
                            monitorSetting.SolidColor = (dialog.Color.ToArgb() & 0x00FFFFFF).ToString("X6");

                            BrushConverter bc = new BrushConverter();
                            solidColorContainer.Background = (Brush)bc.ConvertFrom("#" + monitorSetting.SolidColor);
                        });
                    }
                }
            }
        }

        private void youtubeVideoCodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DataKit.getInstance().getSharedRealms().Write(() =>
            {
                monitorSetting.YoutubeURL = youtubeVideoCodeTextBox.Text;
            });
        }
    }
}
