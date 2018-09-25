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

using HangulClockDataKit;
using HangulClockDataKit.Model;

namespace HangulClock
{
    /// <summary>
    /// DashboardTab.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommentSettingTab : UserControl
    {
        private CommentSettingsByMonitor monitorSetting;

        public CommentSettingTab()
        {
            InitializeComponent();
        }

        public void loadInitData()
        {
            monitorSetting = MainWindow.loadCommentPreferences();

            commentSettingONToggle.IsChecked = monitorSetting.IsEnabled;
            commentFromServerToggle.IsChecked = monitorSetting.IsEnabledLoadFromServer;

            commentField.IsEnabled = commentSettingONToggle.IsChecked ?? false;
            nameJongsungText.Opacity = commentSettingONToggle.IsChecked ?? false ? 1 : 0.3;
            commentNameField.IsEnabled = commentSettingONToggle.IsChecked ?? false;

            commentNameField.Text = monitorSetting.Name;
            commentField.Text = monitorSetting.Comment;
        }

        private void commentSettingONToggle_Checked(object sender, RoutedEventArgs e)
        {
            DataKit.getInstance().getSharedRealms().Write(() =>
            {
                monitorSetting.IsEnabled = true;
            });

            commentField.IsEnabled = true;
            nameJongsungText.Opacity = 1;
            commentNameField.IsEnabled = true;
        }

        private void commentSettingONToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            DataKit.getInstance().getSharedRealms().Write(() =>
            {
                monitorSetting.IsEnabled = false;
            });

            commentField.IsEnabled = false;
            nameJongsungText.Opacity = 0.3;
            commentNameField.IsEnabled = false;
        }

        private void commentFromServerToggle_Checked(object sender, RoutedEventArgs e)
        {
            DataKit.getInstance().getSharedRealms().Write(() =>
            {
                monitorSetting.IsEnabledLoadFromServer = true;
            });
        }

        private void commentFromServerToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            DataKit.getInstance().getSharedRealms().Write(() =>
            {
                monitorSetting.IsEnabledLoadFromServer = false;
            });
        }

        private void commentNameField_TextChanged(object sender, TextChangedEventArgs e)
        {
            DataKit.getInstance().getSharedRealms().Write(() =>
            {
                monitorSetting.Name = commentNameField.Text;
            });
        }

        private void commentField_TextChanged(object sender, TextChangedEventArgs e)
        {
            DataKit.getInstance().getSharedRealms().Write(() =>
            {
                monitorSetting.Comment = commentField.Text;
            });
        }
    }
}
