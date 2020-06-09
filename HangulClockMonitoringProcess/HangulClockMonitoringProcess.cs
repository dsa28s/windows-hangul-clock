using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using HangulClockDataKit;
using HangulClockDataKit.Model;
using HangulClockMonitoringProcess.Model;

namespace HangulClockMonitoringProcess
{
    class HangulClockMonitoringProcess
    {
        // private Thread serviceTimer;
        // private System.Timers.Timer explorerProcessTimer;

        private static List<ScreenModel> screenModels = new List<ScreenModel>();


        [STAThread]
        static void Main(string[] args)
        {
            Process[] hangulClockRendererProcesses = Process.GetProcessesByName("HangulClockRenderer");
            foreach (var hangulClockRendererProcess in hangulClockRendererProcesses)
            {
                try
                {
                    hangulClockRendererProcess.Kill();
                }
                catch (Win32Exception e)
                {

                }
            }

            new Thread(new ThreadStart(ServiceThread)).Start();
            new Thread(new ThreadStart(ExplorerProcessThread)).Start();

            Console.WriteLine("[INFO] : HangulClock Monitoring Subprocess start!");
        }

        private async static void ServiceThread()
        {
            var DataKit = new DataKit();

            while (true)
            {
                try
                {
                    Thread.Sleep(3000);

                    DataKit.Realm.Refresh();

                    if (screenModels.Count <= 0)
                    {
                        Console.WriteLine("[UPDATE] : Enum Monitor processing...");
                        foreach (var screen in System.Windows.Forms.Screen.AllScreens)
                        {
                            screenModels.Add(MakeScreenModel(screen));
                        }
                        Console.WriteLine("[UPDATE] : Enum Monitor process complete.");
                    }

                    var screens = System.Windows.Forms.Screen.AllScreens;
                    int index = 0;

                    if (screens.Length != screenModels.Count) // 모니터의 개수가 줄었거나, 추가됬거나...
                    {
                        Console.WriteLine("[CHANGE] : Physical display device add / deleted. Restart HangulClockRenderer");
                        
                        RestartHangulClockRenderer();
                    }
                    else
                    {
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
                            Console.WriteLine("[CHANGE] : Monitor status changed. Restart HangulClockRenderer");
                            RestartHangulClockRenderer();
                        }
                        else
                        {
                            Process[] hangulClockRendererProcesses = Process.GetProcessesByName("HangulClockRenderer");

                            if (hangulClockRendererProcesses.Length <= 0) // 서비스는 실행됐는데 한글시계 프로세스가 1도 없으면...
                            {
                                Console.WriteLine("[CHECK] : Checking HangulClockRenderer process running...");
                                RestartHangulClockRenderer(); // 한글시계 프로세스 시작df
                            }
                            else // 근데... 머 하나는 실행되있고, 하나는 실행이 안되어있으면 이건 검사해야해
                            {
                                foreach (var item in System.Windows.Forms.Screen.AllScreens.Select((value, i) => new { i, value }))
                                {
                                    var clockSettingQ = DataKit.Realm.All<ClockSettingsByMonitor>().Where(c => c.MonitorDeviceName == item.value.DeviceName);

                                    if (clockSettingQ.Count() > 0)
                                    {
                                        var clockSetting = clockSettingQ.First();

                                        if (clockSetting.isUseHangulClock) // 해당 모니터에서 한글시계를 사용한다네...?
                                        {
                                            var isRunning = false;

                                            foreach (var hangulClockRendererProcess in hangulClockRendererProcesses)
                                            {
                                                if (GetCommandLine(hangulClockRendererProcess).Contains($"/mindex {item.i}"))
                                                {
                                                    isRunning = true;
                                                }
                                            }

                                            if (!isRunning)
                                            {
                                                Console.WriteLine($"[EXECUTE] : Starting HangulClockRenderer for monitor index {item.i}");

                                                Process hangulClockRendererProcess = new Process();
                                                hangulClockRendererProcess.StartInfo = new ProcessStartInfo("HangulClockRenderer.exe");
                                                hangulClockRendererProcess.StartInfo.WorkingDirectory = @"C:\Program Files\Hangul Clock";
                                                hangulClockRendererProcess.StartInfo.Arguments = $"/mindex {item.i}";
                                                hangulClockRendererProcess.StartInfo.CreateNoWindow = true;
                                                hangulClockRendererProcess.StartInfo.UseShellExecute = false;

                                                hangulClockRendererProcess.Start();
                                            }
                                        }
                                        else // 해당 모니터에서는 한글시계를 사용 안한데
                                        {
                                            var isKilled = false;
                                            foreach (var hangulClockRendererProcess in hangulClockRendererProcesses)
                                            {
                                                if (GetCommandLine(hangulClockRendererProcess).Contains($"/mindex {item.i}"))
                                                {
                                                    Console.WriteLine($"[KILL] : Not use HangulClock (monitor index {item.i}). Killing...");
                                                    hangulClockRendererProcess.Kill();
                                                    isKilled = true;
                                                }
                                            }

                                            if (isKilled)
                                            {
                                                Process[] explorerProcesses = Process.GetProcessesByName("explorer");
                                                foreach (var explorerProcess in explorerProcesses)
                                                {
                                                    explorerProcess.Kill();
                                                }

                                                await Task.Delay(2000);

                                                string explorer = string.Format("{0}\\{1}", Environment.GetEnvironmentVariable("WINDIR"), "explorer.exe");
                                                Process process = new Process();
                                                process.StartInfo.FileName = explorer;
                                                process.StartInfo.CreateNoWindow = true;
                                                // process.StartInfo.UseShellExecute = true;
                                                process.Start();

                                                var fileName = Assembly.GetExecutingAssembly().Location;
                                                ProcessStartInfo info = new ProcessStartInfo();
                                                info.FileName = fileName;
                                                info.CreateNoWindow = true;

                                                Process p = new Process();
                                                p.StartInfo = info;
                                                p.Start();

                                                Environment.Exit(0);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
        }

        private static void ExplorerProcessThread()
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

                    Thread.Sleep(3000);
                }
                catch (Exception e)
                {

                }
            }
        }

        private static void RestartHangulClockRenderer()
        {
            screenModels.Clear();

            Process[] hangulClockRendererProcesses = Process.GetProcessesByName("HangulClockRenderer");

            foreach (var hangulClockRendererProcess in hangulClockRendererProcesses)
            {
                hangulClockRendererProcess.Kill();
            }

            var screens = System.Windows.Forms.Screen.AllScreens;

            new Thread(() =>
            {
                var DataKit = new DataKit();

                foreach (var item in screens.Select((value, i) => new { i, value }))
                {
                    var clockSettingQ = DataKit.Realm.All<ClockSettingsByMonitor>().Where(c => c.MonitorDeviceName == item.value.DeviceName);

                    if (clockSettingQ.Count() > 0)
                    {
                        var clockSetting = clockSettingQ.First();

                        if (clockSetting.isUseHangulClock)
                        {
                            Process hangulClockRendererProcess = new Process();
                            hangulClockRendererProcess.StartInfo = new ProcessStartInfo("HangulClockRenderer.exe");

#if !DEBUG
                            hangulClockRendererProcess.StartInfo.WorkingDirectory = @"C:\Program Files\Hangul Clock";
#endif
                            hangulClockRendererProcess.StartInfo.Arguments = $"/mindex {item.i}";
                            hangulClockRendererProcess.StartInfo.CreateNoWindow = true;
                            hangulClockRendererProcess.StartInfo.UseShellExecute = false;

                            hangulClockRendererProcess.Start();
                        }
                    }
                }
            }).Start();
        }

        private static ScreenModel MakeScreenModel(System.Windows.Forms.Screen screen)
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

        private static string GetCommandLine(Process process)
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher($"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {process.Id}"))
            using (ManagementObjectCollection objects = searcher.Get())
            {
                var singleOrDefault = objects.Cast<ManagementBaseObject>().SingleOrDefault();

                if (singleOrDefault != null)
                {
                    var commandLine = singleOrDefault["CommandLine"];

                    if (commandLine != null)
                    {
                        return commandLine.ToString();
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
