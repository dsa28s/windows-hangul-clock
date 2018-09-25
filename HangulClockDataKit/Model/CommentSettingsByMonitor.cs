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
        public string MonitorDeviceName { get; set; }

        public bool IsEnabled { get; set; }
        public bool IsEnabledLoadFromServer { get; set; }

        public string Name { get; set; }
        public string Comment { get; set; }
    }
}
