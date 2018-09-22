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
    public partial class CommentSettingTab : UserControl
    {
        public CommentSettingTab()
        {
            InitializeComponent();
        }

        public void loadInitData()
        {
            commentField.IsEnabled = false;
            nameJongsungText.Opacity = 0.3;
            commentNameField.IsEnabled = false;
        }

        private void commentSettingONToggle_Checked(object sender, RoutedEventArgs e)
        {
            commentField.IsEnabled = true;
            nameJongsungText.Opacity = 1;
            commentNameField.IsEnabled = true;
        }

        private void commentSettingONToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            commentField.IsEnabled = false;
            nameJongsungText.Opacity = 0.3;
            commentNameField.IsEnabled = false;
        }
    }
}
