using Realms;

namespace HangulClockDataKit.Model
{
    public class ClockSettingsByMonitor : RealmObject
    {
        public string MonitorDeviceName { get; set; }
        public string YoutubeURL { get; set; }
        public bool IsWhiteClock { get; set; }
        public int ClockSize { get; set; }

        public bool isUseHangulClock { get; set; }
        public string FontName { get; set; }
    }
}
