using System;
using System.Collections.Generic;
using System.Diagnostics;
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

using HangulClockKit;
using HangulClockDataKit;
using HangulClockDataKit.Model;
using System.Threading;

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
            new Thread(() =>
            {
                var DataKit = new DataKit();
                var monitorSettingQuery = from c in DataKit.Realm.All<CommentSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c;

                CommentSettingsByMonitor monitorSetting = monitorSettingQuery.First();

                bool isEnabled = monitorSetting.IsEnabled;
                bool isEnabledLoadFromServer = monitorSetting.IsEnabledLoadFromServer;
                string name = monitorSetting.Name;
                string comment = monitorSetting.Comment;

                int commentPosition = monitorSetting.DirectionOfComment;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    commentSettingONToggle.IsChecked = isEnabled;
                    commentFromServerToggle.IsChecked = isEnabledLoadFromServer;

                    commentField.IsEnabled = commentSettingONToggle.IsChecked ?? false;
                    nameJongsungText.Opacity = commentSettingONToggle.IsChecked ?? false ? 1 : 0.3;
                    commentNameField.IsEnabled = commentSettingONToggle.IsChecked ?? false;

                    commentNameField.Text = name;
                    commentField.Text = comment;

                    saveCommentPosition(commentPosition);
                }));
            }).Start();
        }

        private void commentSettingONToggle_Checked(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                var DataKit = new DataKit();
                var monitorSetting = (from c in DataKit.Realm.All<CommentSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();

                DataKit.Realm.Write(() =>
                {
                    monitorSetting.IsEnabled = true;
                });
            }).Start();

            commentField.IsEnabled = true;
            nameJongsungText.Opacity = 1;
            commentNameField.IsEnabled = true;
        }

        private void commentSettingONToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                var DataKit = new DataKit();
                var monitorSetting = (from c in DataKit.Realm.All<CommentSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();

                DataKit.Realm.Write(() =>
                {
                    monitorSetting.IsEnabled = false;
                });
            }).Start();

            commentField.IsEnabled = false;
            nameJongsungText.Opacity = 0.3;
            commentNameField.IsEnabled = false;
        }

        private void commentFromServerToggle_Checked(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                var DataKit = new DataKit();
                var monitorSetting = (from c in DataKit.Realm.All<CommentSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();

                DataKit.Realm.Write(() =>
                {
                    monitorSetting.IsEnabledLoadFromServer = true;
                });
            }).Start();
        }

        private void commentFromServerToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                var DataKit = new DataKit();
                var monitorSetting = (from c in DataKit.Realm.All<CommentSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();

                DataKit.Realm.Write(() =>
                {
                    monitorSetting.IsEnabledLoadFromServer = false;
                });
            }).Start();
        }

        private void commentNameField_TextChanged(object sender, TextChangedEventArgs e)
        {
            string name = commentNameField.Text;

            new Thread(() =>
            {
                var DataKit = new DataKit();
                var monitorSetting = (from c in DataKit.Realm.All<CommentSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();

                DataKit.Realm.Write(() =>
                {
                    monitorSetting.Name = name;
                });
            }).Start();
            
            try
            {
                HangulKit.HANGUL_INFO partOfName = HangulKit.HangulJaso.DevideJaso(commentNameField.Text[commentNameField.Text.Length - 1]);
                if (partOfName.chars[2] == ' ')
                {
                    nameJongsungText.Content = "야, ";
                }
                else
                {
                    nameJongsungText.Content = "아, ";
                }
            }
            catch (NullReferenceException ee)
            {

            }
            catch (IndexOutOfRangeException ee)
            {

            }
        }

        private void commentField_TextChanged(object sender, TextChangedEventArgs e)
        {
            string comment = commentField.Text;

            new Thread(() =>
            {
                var DataKit = new DataKit();
                var monitorSetting = (from c in DataKit.Realm.All<CommentSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();

                DataKit.Realm.Write(() =>
                {
                    monitorSetting.Comment = comment;
                });
            }).Start();
        }

        private void setButtonHoverEnterEvent(Grid container)
        {
            BrushConverter bc = new BrushConverter();
            container.Background = (Brush)bc.ConvertFrom("#99FFFFFF");
        }

        private void setButtonHoverOutEvent(Grid container)
        {
            BrushConverter bc = new BrushConverter();
            container.Background = (Brush)bc.ConvertFrom("#03FFFFFF");
        }

        private void saveCommentPosition(int direction)
        {
            new Thread(() =>
            {
                var DataKit = new DataKit();
                var monitorSetting = (from c in DataKit.Realm.All<CommentSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();

                DataKit.Realm.Write(() =>
                {
                    monitorSetting.DirectionOfComment = direction;
                });
            }).Start();

            BrushConverter bc = new BrushConverter();

            commentPositionTopContainer.Background = (Brush)bc.ConvertFrom("#03FFFFFF");
            commentPositionLeftContainer.Background = (Brush)bc.ConvertFrom("#03FFFFFF");
            commentPositionRightContainer.Background = (Brush)bc.ConvertFrom("#03FFFFFF");
            commentPositionBottomContainer.Background = (Brush)bc.ConvertFrom("#03FFFFFF");

            if (direction == CommentSettingsByMonitor.CommentDirection.TOP)
            {
                commentPositionTopContainer.Background= (Brush)bc.ConvertFrom("#FFFFFF");
            }
            else if (direction == CommentSettingsByMonitor.CommentDirection.LEFT)
            {
                commentPositionLeftContainer.Background = (Brush)bc.ConvertFrom("#FFFFFF");
            }
            else if (direction == CommentSettingsByMonitor.CommentDirection.RIGHT)
            {
                commentPositionRightContainer.Background = (Brush)bc.ConvertFrom("#FFFFFF");
            }
            else
            {
                commentPositionBottomContainer.Background = (Brush)bc.ConvertFrom("#FFFFFF");
            }
        }

        #region Comment Up Container Event Handler
        private void commentPositionTopContainer_MouseEnter(object sender, MouseEventArgs e)
        {
            new Thread(() =>
            {
                var DataKit = new DataKit();
                var monitorSetting = (from c in DataKit.Realm.All<CommentSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();

                int commentDirection = monitorSetting.DirectionOfComment;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    if (commentDirection != CommentSettingsByMonitor.CommentDirection.TOP)
                    {
                        setButtonHoverEnterEvent(commentPositionTopContainer);
                    }
                }));
            }).Start();
        }

        private void commentPositionTopContainer_MouseLeave(object sender, MouseEventArgs e)
        {
            new Thread(() =>
            {
                var DataKit = new DataKit();
                var monitorSetting = (from c in DataKit.Realm.All<CommentSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();

                int commentDirection = monitorSetting.DirectionOfComment;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    if (commentDirection != CommentSettingsByMonitor.CommentDirection.TOP)
                    {
                        setButtonHoverOutEvent(commentPositionTopContainer);
                    }
                }));
            }).Start();
        }

        private void commentPositionTopContainer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            saveCommentPosition(CommentSettingsByMonitor.CommentDirection.TOP);
        }
        #endregion

        #region Comment Left Container Event Handler
        private void commentPositionLeftContainer_MouseEnter(object sender, MouseEventArgs e)
        {
            new Thread(() =>
            {
                var DataKit = new DataKit();
                var monitorSetting = (from c in DataKit.Realm.All<CommentSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();

                int commentDirection = monitorSetting.DirectionOfComment;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    if (commentDirection != CommentSettingsByMonitor.CommentDirection.LEFT)
                    {
                        setButtonHoverEnterEvent(commentPositionLeftContainer);
                    }
                }));
            }).Start();
        }

        private void commentPositionLeftContainer_MouseLeave(object sender, MouseEventArgs e)
        {
            new Thread(() =>
            {
                var DataKit = new DataKit();
                var monitorSetting = (from c in DataKit.Realm.All<CommentSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();

                int commentDirection = monitorSetting.DirectionOfComment;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    if (commentDirection != CommentSettingsByMonitor.CommentDirection.LEFT)
                    {
                        setButtonHoverOutEvent(commentPositionLeftContainer);
                    }
                }));
            }).Start();
        }

        private void commentPositionLeftContainer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            saveCommentPosition(CommentSettingsByMonitor.CommentDirection.LEFT);
        }
        #endregion

        #region Comment Right Container Event Handler
        private void commentPositionRightContainer_MouseEnter(object sender, MouseEventArgs e)
        {
            new Thread(() =>
            {
                var DataKit = new DataKit();
                var monitorSetting = (from c in DataKit.Realm.All<CommentSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();

                int commentDirection = monitorSetting.DirectionOfComment;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    if (commentDirection != CommentSettingsByMonitor.CommentDirection.RIGHT)
                    {
                        setButtonHoverEnterEvent(commentPositionRightContainer);
                    }
                }));
            }).Start();
        }

        private void commentPositionRightContainer_MouseLeave(object sender, MouseEventArgs e)
        {
            new Thread(() =>
            {
                var DataKit = new DataKit();
                var monitorSetting = (from c in DataKit.Realm.All<CommentSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();

                int commentDirection = monitorSetting.DirectionOfComment;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    if (commentDirection != CommentSettingsByMonitor.CommentDirection.RIGHT)
                    {
                        setButtonHoverOutEvent(commentPositionRightContainer);
                    }
                }));
            }).Start();
        }

        private void commentPositionRightContainer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            saveCommentPosition(CommentSettingsByMonitor.CommentDirection.RIGHT);
        }
        #endregion

        #region Comment Bottom Container Event Handler
        private void commentPositionBottomContainer_MouseEnter(object sender, MouseEventArgs e)
        {
            new Thread(() =>
            {
                var DataKit = new DataKit();
                var monitorSetting = (from c in DataKit.Realm.All<CommentSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();

                int commentDirection = monitorSetting.DirectionOfComment;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    if (commentDirection != CommentSettingsByMonitor.CommentDirection.BOTTOM)
                    {
                        setButtonHoverEnterEvent(commentPositionBottomContainer);
                    }
                }));
            }).Start();
        }

        private void commentPositionBottomContainer_MouseLeave(object sender, MouseEventArgs e)
        {
            new Thread(() =>
            {
                var DataKit = new DataKit();
                var monitorSetting = (from c in DataKit.Realm.All<CommentSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();

                int commentDirection = monitorSetting.DirectionOfComment;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    if (commentDirection != CommentSettingsByMonitor.CommentDirection.BOTTOM)
                    {
                        setButtonHoverOutEvent(commentPositionBottomContainer);
                    }
                }));
            }).Start();
        }

        private void commentPositionBottomContainer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            saveCommentPosition(CommentSettingsByMonitor.CommentDirection.BOTTOM);
        }
        #endregion

        private void registerCommentOnServerToggle_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void commentUseNameToggle_Checked(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                var DataKit = new DataKit();
                var monitorSetting = (from c in DataKit.Realm.All<CommentSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();

                DataKit.Realm.Write(() =>
                {
                    monitorSetting.IsEnabledNameInComment = true;
                });
            }).Start();
        }

        private void commentUseNameToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                var DataKit = new DataKit();
                var monitorSetting = (from c in DataKit.Realm.All<CommentSettingsByMonitor>() where c.MonitorDeviceName == MainWindow.MonitorDeviceName select c).First();

                DataKit.Realm.Write(() =>
                {
                    monitorSetting.IsEnabledNameInComment = false;
                });
            }).Start();
        }
    }
}
