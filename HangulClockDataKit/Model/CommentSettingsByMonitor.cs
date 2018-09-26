using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangulClockDataKit.Model
{
    public class CommentSettingsByMonitor : RealmObject
    {
        public class CommentDirection
        {
            public static readonly int TOP = 0;
            public static readonly int LEFT = 1;
            public static readonly int RIGHT = 2;
            public static readonly int BOTTOM = 3;
        }

        public string MonitorDeviceName { get; set; }

        public bool IsEnabled { get; set; }
        public bool IsEnabledLoadFromServer { get; set; }

        public string Name { get; set; }
        public string Comment { get; set; }

        public int DirectionOfComment { get; set; }
    }
}
