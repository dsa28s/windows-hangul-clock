using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangulClockMonitoringService.Model
{
    public class ScreenModel
    {
        public int Left { get; set; }
        public int Right { get; set; }
        public int Top { get; set; }
        public int Bottom { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public string DeviceName { get; set; }

        public static bool operator == (ScreenModel s1, ScreenModel s2)
        {
            return s1.Left == s2.Left && s1.Right == s2.Right && s1.Top == s2.Top && s1.Bottom == s2.Bottom && s1.Width == s2.Width && s1.Height == s2.Height && s1.DeviceName == s2.DeviceName;
        }

        public static bool operator != (ScreenModel s1, ScreenModel s2)
        {
            return s1.Left != s2.Left || s1.Right != s2.Right || s1.Top != s2.Top || s1.Bottom != s2.Bottom || s1.Width != s2.Width || s1.Height != s2.Height || s1.DeviceName != s2.DeviceName;
        }
    }
}
