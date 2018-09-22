using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangulClockDataKit.Model
{
    public class ClockSettingsByMonitor : RealmObject
    {
        public string MonitorDeviceName { get; set; }
        public string YoutubeURL { get; set; }
        public bool IsWhiteClock { get; set; }
        public int ClockSize { get; set; }
    }
}
