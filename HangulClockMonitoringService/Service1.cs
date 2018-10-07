using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace HangulClockMonitoringService
{
    public partial class Service1 : ServiceBase
    {
        private System.Timers.Timer serviceTimer;

        public Service1()
        {
            this.AutoLog = true;
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            serviceTimer = new System.Timers.Timer(1 * 1000);
            serviceTimer.Elapsed += new System.Timers.ElapsedEventHandler(serviceTimer_Elapsed);
            serviceTimer.Start();
        }

        protected override void OnStop()
        {
            serviceTimer.Stop();
        }

        private void serviceTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Process[] hangulClockMonitoringProcess = Process.GetProcessesByName("HangulClockMonitoringProcess");

            if (hangulClockMonitoringProcess.Length <= 0)
            {
                Process hangulClockRendererProcess = new Process();
                hangulClockRendererProcess.StartInfo.FileName = @"C:\Program Files\Hangul Clock\HangulClockMonitoringProcess.exe";
                // hangulClockRendererProcess.StartInfo = new ProcessStartInfo("HangulClockMonitoringProcess.exe");
                // hangulClockRendererProcess.StartInfo.WorkingDirectory = @"C:\Program Files\Hangul Clock";
                hangulClockRendererProcess.StartInfo.CreateNoWindow = true;
                hangulClockRendererProcess.StartInfo.UseShellExecute = false;

                hangulClockRendererProcess.Start();
            }
        }
    }
}
