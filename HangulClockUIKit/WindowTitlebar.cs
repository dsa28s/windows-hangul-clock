using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HangulClockUIKit
{
    public class WindowTitlebar
    {
        private static Uri closeButtonImgSource = new Uri(@"/HangulClockUIKit;component/Resources/close_normal.png", UriKind.Relative);
        private static Uri closeHoverButtonImgSource = new Uri(@"/HangulClockUIKit;component/Resources/close_hover.png", UriKind.Relative);
        private static Uri minimizeButtonImgSource = new Uri(@"/HangulClockUIKit;component/Resources/minimize_normal.png", UriKind.Relative);
        private static Uri minimizeHoverButtonImgSource = new Uri(@"/HangulClockUIKit;component/Resources/minimize_hover.png", UriKind.Relative);

        public static void AttachTitlebar(Window window, Grid grid, MouseButtonEventHandler closeButtonEventHandler, MouseButtonEventHandler minimizeButtonEventHandler)
        {
            var closeButton = new Image();
            closeButton.Source = new BitmapImage(closeButtonImgSource);
            closeButton.Width = 32;
            closeButton.Height = 32;
            closeButton.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            closeButton.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            closeButton.Stretch = System.Windows.Media.Stretch.UniformToFill;

            closeButton.MouseEnter += CloseButton_MouseEnter;
            closeButton.MouseLeave += CloseButton_MouseLeave;
            closeButton.MouseDown += closeButtonEventHandler;

            // ==== //

            var minimizeButton = new Image();
            minimizeButton.Source = new BitmapImage(minimizeButtonImgSource);
            minimizeButton.Width = 32;
            minimizeButton.Height = 32;
            minimizeButton.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            minimizeButton.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            minimizeButton.Margin = new System.Windows.Thickness(0, 0, 32, 0);
            minimizeButton.Stretch = System.Windows.Media.Stretch.UniformToFill;

            minimizeButton.MouseEnter += MinimizeButton_MouseEnter;
            minimizeButton.MouseLeave += MinimizeButton_MouseLeave;
            minimizeButton.MouseDown += minimizeButtonEventHandler;

            grid.Children.Add(closeButton);
            grid.Children.Add(minimizeButton);
        }

        private static void MinimizeButton_MouseLeave(object sender, MouseEventArgs e)
        {
            Image minimizeButton = (Image)sender;
            minimizeButton.Source = new BitmapImage(minimizeButtonImgSource);
        }

        private static void MinimizeButton_MouseEnter(object sender, MouseEventArgs e)
        {
            Image minimizeButton = (Image)sender;
            minimizeButton.Source = new BitmapImage(minimizeHoverButtonImgSource);
        }

        private static void CloseButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Image closeButton = (Image)sender;
            closeButton.Source = new BitmapImage(closeButtonImgSource);
        }

        private static void CloseButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Image closeButton = (Image)sender;
            closeButton.Source = new BitmapImage(closeHoverButtonImgSource);
        }
    }
}
