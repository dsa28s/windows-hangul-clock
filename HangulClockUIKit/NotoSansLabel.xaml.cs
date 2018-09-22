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
    public partial class MinimalButton : UserControl
    {
        public MinimalButton()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ButtonLabelProperty = DependencyProperty.Register(
            "ButtonText", typeof(string), typeof(MinimalButton), new FrameworkPropertyMetadata(new PropertyChangedCallback(ButtonTextControl)));

        public string ButtonText
        {
            get { return (string)GetValue(ButtonLabelProperty); }
            set { SetValue(ButtonLabelProperty, value); }
        }

        private static void ButtonTextControl(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            (source as MinimalButton).UpdateButtonText((string)e.NewValue);
        }

        private void UpdateButtonText(string value)
        {
            buttonLabel.Content = value;
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            buttonBorder.Opacity = 0.5;
            buttonLabel.Opacity = 0.5;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            buttonBorder.Opacity = 1;
            buttonLabel.Opacity = 1;
        }
    }
}
