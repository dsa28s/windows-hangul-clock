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

namespace HangulClock
{
    /// <summary>
    /// DashboardTab.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DashboardTab : UserControl
    {
        public DashboardTab()
        {
            InitializeComponent();
        }

        public void loadInitData()
        {
            if (hangulClockONOFFToggle.IsChecked == true)
            {
                useText.Content = "사용중이야.";
            }
            else
            {
                useText.Content = "사용중이지 않아.";
            }

            Process[] hangulClockRendererProcesses = Process.GetProcessesByName("HangulClockRenderer");

            if (hangulClockRendererProcesses.Length > 0)
            {
                useText.Content = "사용중이야.";
                hangulClockONOFFToggle.IsChecked = true;
            }
            else
            {
                useText.Content = "사용중이지 않아.";
                hangulClockONOFFToggle.IsChecked = false;
            }
        }

        private void hangulClockONOFFToggle_Checked(object sender, RoutedEventArgs e)
        {
            useText.Content = "사용중이야.";
        }

        private void hangulClockONOFFToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            useText.Content = "사용중이지 않아.";
        }
    }
}
