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

namespace HangulClockUIKit
{
    /// <summary>
    /// MinimalButton.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NotoSansLabel : UserControl
    {
        public NotoSansLabel()
        {
            InitializeComponent();

            label.HorizontalContentAlignment = HorizontalAlignment.Center;
        }

        public void setTextSize(int size)
        {
            label.FontSize = size;
        }

        public void setContent(string content)
        {
            label.Content = content;
        }

        public void setTextColor(Brush color)
        {
            label.Foreground = color;
        }
    }
}
