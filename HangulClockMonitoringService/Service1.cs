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

using HangulClockDataKit;
using HangulClockDataKit.Model;
using HangulClockLogKit;
using HangulClockMonitoringService.Model;

namespace HangulClockMonitoringService
{
    public partial class Service1 : ServiceBase
    {
        private System.Timers.Timer serviceTimer;
        private System.Timers.Timer explorerProcessTimer;

        private List<ScreenModel> screenModels = new List<ScreenModel>();

        public Service1()
        {
            this.AutoLog = true;
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            LogKit.Info("Starting HangulClock Monitoring Service!");

            serviceTimer = new System.Timers.Timer(3 * 1000);
            serviceTimer.Elapsed += new System.Timers.ElapsedEventHandler(serviceTimer_Elapsed);
            serviceTimer.Interval = 3000;
            serviceTimer.Start();

            explorerProcessTimer = new System.Timers.Timer(1 * 1000);
            explorerProcessTimer.Elapsed += new System.Timers.ElapsedEventHandler(explorerProcessTimer_Elapsed);
            explorerProcessTimer.Interval = 1000;
            explorerProcessTimer.Start();

            System.Diagnostics.Debugger.Launch();
        }

        protected override void OnStop()
        {
            LogKit.Info("HangulClock Monitoring Service Stopped.");

            serviceTimer.Stop();
            explorerProcessTimer.Stop();
        }

        private void serviceTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var DataKit = new DataKit();
            DataKit.Realm.Refresh();

            if (screenModels.Count <= 0)
            {
                foreach (var screen in System.Windows.Forms.Screen.AllScreens)
                {
                    screenModels.Add(MakeScreenModel(screen));
                }
            }

            var screens = System.Windows.Forms.Screen.AllScreens;
            int index = 0;

            if (screens.Length != screenModels.Count) // 모니터의 개수가 줄었거나, 추가됬거나...
            {
                RestartHangulClockRenderer();
                return;
            }

            // var clockSetting = DataKit.Realm.All<ClockSettingsByMonitor>().Where(c => c.MonitorDeviceName == screen.DeviceName).First();

            bool isNotChanged = true;
            foreach (var screen in screens)
            {
                if (MakeScreenModel(screen) != screenModels[index])
                {
                    isNotChanged = false;
                    break;
                }

                index++;
            }

            if (!isNotChanged) // 음... 머 어떤 모니터의 해상도라던지 위치 등이 변경된듯...?
            {
                RestartHangulClockRenderer();
                return;
            }

            Process[] hangulClockRendererProcesses = Process.GetProcessesByName("HangulClockRenderer");

            if (hangulClockRendererProcesses.Length <= 0) // 서비스는 실행됐는데 한글시계 프로세스가 1도 없으면...
            {
                RestartHangulClockRenderer(); // 한글시계 프로세스 시작
            }
            else // 근데... 머 하나는 실행되있고, 하나는 실행이 안되어있으면 이건 검사해야해
            {
                foreach (var item in System.Windows.Forms.Screen.AllScreens.Select((value, i) => new { i, value }))
                {
                    var clockSetting = DataKit.Realm.All<ClockSettingsByMonitor>().Where(c => c.MonitorDeviceName == item.value.DeviceName).First();

                    if (clockSetting.isUseHangulClock) // 해당 모니터에서 한글시계를 사용한다네...?
                    {
                        var isRunning = false;

                        foreach (var hangulClockRendererProcess in hangulClockRendererProcesses)
                        {
                            if (hangulClockRendererProcess.StartInfo.Arguments == String.Format("/mindex {0}", item.i))
                            {
                                isRunning = true;
                            }
                        }

                        if (!isRunning)
                        {
                            Process hangulClockRendererProcess = new Process();
                            hangulClockRendererProcess.StartInfo = new ProcessStartInfo("HangulClockRenderer.exe");
                            hangulClockRendererProcess.StartInfo.WorkingDirectory = @"C:\Program Files\Hangul Clock";
                            hangulClockRendererProcess.StartInfo.Arguments = String.Format("/mindex {0}", item.i);
                            hangulClockRendererProcess.StartInfo.CreateNoWindow = true;
                            hangulClockRendererProcess.StartInfo.UseShellExecute = false;

                            hangulClockRendererProcess.Start();
                        }
                    }
                }
            }
        }

        private void explorerProcessTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Process[] explorerProcesses = Process.GetProcessesByName("explorer");

            if (explorerProcesses.Length <= 0) // Windows 탐색기가 죽었으니까 한글시계도 실행될 필요는 없을듯...?
            {
                Process[] hangulClockRendererProcesses = Process.GetProcessesByName("HangulClockRenderer");

                foreach (var hangulClockRendererProcess in hangulClockRendererProcesses)
                {
                    hangulClockRendererProcess.Kill();
                }
            }
            else // 다시 나왔으니 실행하자
            {
                Process[] hangulClockRendererProcesses = Process.GetProcessesByName("HangulClockRenderer");

                if (hangulClockRendererProcesses.Length <= 0)
                {
                    RestartHangulClockRenderer();
                }
            }
        }

        private void RestartHangulClockRenderer()
        {
            Process[] hangulClockRendererProcesses = Process.GetProcessesByName("HangulClockRenderer");

            foreach (var hangulClockRendererProcess in hangulClockRendererProcesses)
            {
                hangulClockRendererProcess.Kill();
            }

            var DataKit = new DataKit();

            foreach (var item in System.Windows.Forms.Screen.AllScreens.Select((value, i) => new { i, value }))
            {
                var clockSetting = DataKit.Realm.All<ClockSettingsByMonitor>().Where(c => c.MonitorDeviceName == item.value.DeviceName).First();

                if (clockSetting.isUseHangulClock)
                {
                    Process hangulClockRendererProcess = new Process();
                    hangulClockRendererProcess.StartInfo = new ProcessStartInfo("HangulClockRenderer.exe");
                    hangulClockRendererProcess.StartInfo.WorkingDirectory = @"C:\Program Files\Hangul Clock";
                    hangulClockRendererProcess.StartInfo.Arguments = String.Format("/mindex {0}", item.i);
                    hangulClockRendererProcess.StartInfo.CreateNoWindow = true;
                    hangulClockRendererProcess.StartInfo.UseShellExecute = false;

                    hangulClockRendererProcess.Start();
                }
            }
        }

        private ScreenModel MakeScreenModel(System.Windows.Forms.Screen screen)
        {
            return new ScreenModel()
            {
                Left = screen.Bounds.Left,
                Right = screen.Bounds.Right,
                Top = screen.Bounds.Top,
                Bottom = screen.Bounds.Bottom,

                DeviceName = screen.DeviceName,
                Width = screen.Bounds.Width,
                Height = screen.Bounds.Height,
            };
        }
    }
}
