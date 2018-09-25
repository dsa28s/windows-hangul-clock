using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangulClockDataKit.Model
{
    public class BackgroundSettingsByMonitor : RealmObject
    {
        public class BackgroundType
        {
            public static int STILL_IMAGE = 0;
            public static int SOLID = 1;
            public static int YOUTUBE_VIDEO = 2;
            public static int DEFAULT = -1;
        }

        public int backgroundType { get; set; }
        public string MonitorDeviceName { get; set; }

        public string YoutubeURL { get; set; }
        public string SolidColor { get; set; } // HEX VALUE
        public string imgPath { get; set; }
    }
}
