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
        private Thread serviceThread;
        private Thread explorerProcessThread;

        private List<ScreenModel> screenModels = new List<ScreenModel>();

        public Service1()
        {
            this.AutoLog = true;
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            LogKit.Info("Starting HangulClock Monitoring Service!");
            System.Diagnostics.Debugger.Launch();

            serviceThread = new Thread(ServiceThread);
            explorerProcessThread = new Thread(ExplorerProcessCheckThread);

            serviceThread.Start();
            explorerProcessThread.Start();
        }

        protected override void OnStop()
        {
            LogKit.Info("HangulClock Monitoring Service Stopped.");

            serviceThread.Interrupt();
            explorerProcessThread.Interrupt();
        }

        private void ExplorerProcessCheckThread()
        {
            while (true)
            {
                try
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

                    Thread.Sleep(1000);
                }
                catch (Exception e)
                {

                }
            }
        }

        private void ServiceThread()
        {
            var DataKit = new DataKit();

            while (true)
            {
                DataKit.Realm.Refresh();

                try
                {
                    Thread.Sleep(3000);

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
                }
                catch (Exception e)
                {

                }
            }
        }

        private void RestartHangulClockRenderer()
        {
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
