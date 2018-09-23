using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Forms;

namespace HangulClockUIKit
{
    public class UIKit
    {
        private static Uri LogoURL = new Uri(@"/HangulClockUIKit;component/Resources/icon.png", UriKind.Relative);

        public static BitmapImage GetLogoImage()
        {
            return new BitmapImage(LogoURL);
        }

        public enum HangulClockTab
        {
            DASHBOARD = 1,
            CLOCK_SETTINGS = 2,
            WALLPAPER_SETTINGS = 3,
            THEME_SETTINGS = 4,
            COMMENT_SETTINGS = 5,
            INFORMATION = 6
        }

        public static DateTime Delay(int MS)
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);

            while (AfterWards >= ThisMoment)
            {
                System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;
            }

            return DateTime.Now;
        }
    }
}
