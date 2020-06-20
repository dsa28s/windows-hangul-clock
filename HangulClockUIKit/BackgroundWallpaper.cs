using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace HangulClockUIKit
{
    public class BackgroundWallpaper
    {
        public static Uri DefaultImage1 = new Uri(@"/HangulClockUIKit;component/Resources/yujing-zhang-KMJbSDVWNZY-unsplash.jpg", UriKind.Relative);
        public static Uri DefaultImage2 = new Uri(@"/HangulClockUIKit;component/Resources/ping-onganankun-5htrsUUbFGI-unsplash.jpg", UriKind.Relative);
        public static Uri DefaultImage3 = new Uri(@"/HangulClockUIKit;component/Resources/yohan-cho-Mwvhyd22Lyw-unsplash.jpg", UriKind.Relative);
        public static Uri DefaultImage4 = new Uri(@"/HangulClockUIKit;component/Resources/sunyu-kim-HjsWTyyVDgg-unsplash.jpg", UriKind.Relative);
        public static Uri DefaultImage5 = new Uri(@"/HangulClockUIKit;component/Resources/mathew-schwartz-01hH6y7oZFk-unsplash.jpg", UriKind.Relative);

        private static Uri[] DefaultImages = { DefaultImage1, DefaultImage2, DefaultImage3, DefaultImage4, DefaultImage5 };

        public static BitmapImage GetRandomDefaultImage()
        {
            Random rnd = new Random();
            int randomNumber = rnd.Next(5); // 0이상 6 미만

            return new BitmapImage(DefaultImages[randomNumber]);
        }
    }
}
