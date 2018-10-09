using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangulClockRenderer.Model
{
    internal class ScreenModel
    {
        public int width;
        public int height;
        public int x;
        public int y;

        public int monitorIndex;

        public string deviceName;
        public float zoomFactor;

        public bool isPrimary;
    }
}
