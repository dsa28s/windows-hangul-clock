using System.Linq;
using System.Windows.Controls;

namespace HangulClock
{
    /// <summary>
    /// DashboardTab.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ThemeSettingTab : UserControl
    {
        public ThemeSettingTab()
        {
            InitializeComponent();

            DataContext = Enumerable.Range(1, 10)
                .Select(x => new WallpaperSetInfo()
                {
                    WallpaperPath = @"C:/Users/도라도라/Pictures/george-hiles-361978.jpg",
                    Name = "1234"
                }).ToList();
        }

        public WallpaperSetInfo SelectedWallpaper { get; set; }

        public class WallpaperSetInfo
        {
            public string WallpaperPath { get; set; }
            public string Name { get; set; }
        }
    }
}
