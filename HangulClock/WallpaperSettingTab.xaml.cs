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

            DataContext = Enumerable.Range(1, 10)
                .Select(x => new WallpaperSetInfo()
                {
                    WallpaperPath = @"C:/Users/도라도라/Pictures/george-hiles-361978.jpg",
                    Name = "1234"
                }).ToList();
        }

        public void loadInitData()
        {
            solidColorSettingButton.IsEnabled = false;
            solidColorSettingButton.Opacity = 0.3;
            backgroundStatusText.Content = "배경화면 슬라이드쇼 사용";
            listviewTouchInterceptPanel.Visibility = Visibility.Collapsed;
            wallpaperList.Opacity = 1;
            solidColorPanel.Opacity = 0.3;
        }

        public WallpaperSetInfo SelectedWallpaper { get; set; }

        public class WallpaperSetInfo
        {
            public string WallpaperPath { get; set; }
            public string Name { get; set; }
        }

        private void backgroundToggle_Checked(object sender, RoutedEventArgs e) // 단색 사용
        {
            solidColorSettingButton.IsEnabled = true;
            solidColorSettingButton.Opacity = 1;
            backgroundStatusText.Content = "단색 배경화면 사용";
            listviewTouchInterceptPanel.Visibility = Visibility.Visible;
            wallpaperList.Opacity = 0.3;
            solidColorPanel.Opacity = 1;
        }

        private void backgroundToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            solidColorSettingButton.IsEnabled = false;
            solidColorSettingButton.Opacity = 0.3;
            backgroundStatusText.Content = "배경화면 슬라이드쇼 사용";
            listviewTouchInterceptPanel.Visibility = Visibility.Collapsed;
            wallpaperList.Opacity = 1;
            solidColorPanel.Opacity = 0.3;
        }
    }
}
