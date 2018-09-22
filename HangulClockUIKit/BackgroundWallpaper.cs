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
        public static Uri DefaultImage1 = new Uri(@"/HangulClockUIKit;component/Resources/andrew-ridley-451447.jpg", UriKind.Relative);
        public static Uri DefaultImage2 = new Uri(@"/HangulClockUIKit;component/Resources/nathan-dumlao-451093.jpg", UriKind.Relative);
        public static Uri DefaultImage3 = new Uri(@"/HangulClockUIKit;component/Resources/michel-catalisano-451800.jpg", UriKind.Relative);
        public static Uri DefaultImage4 = new Uri(@"/HangulClockUIKit;component/Resources/nico-kramer-450511.jpg", UriKind.Relative);
        public static Uri DefaultImage5 = new Uri(@"/HangulClockUIKit;component/Resources/robert-zurfluh-449481.jpg", UriKind.Relative);

        private static Uri[] DefaultImages = { DefaultImage1, DefaultImage2, DefaultImage3, DefaultImage4, DefaultImage5 };

        public static BitmapImage GetRandomDefaultImage()
        {
            Random rnd = new Random();
            int randomNumber = rnd.Next(5); // 0이상 6 미만

            return new BitmapImage(DefaultImages[randomNumber]);
        }
    }
}
