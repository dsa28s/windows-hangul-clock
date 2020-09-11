using HangulClockDataKit;
using HangulClockDataKit.Model;
using HangulClockHookKit;
using HangulClockKit;
using HangulClockLogKit;
using HangulClockRenderer.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Drawing;

namespace HangulClockRenderer
{
    internal class Renderer
    {
        private const string COMMENT_STRING_URL = "https://us-central1-hangul-clock.cloudfunctions.net/comment/";

        private static HangulClockDesktop hangulClockDesktop;
        private static readonly List<ScreenModel> screenModels = new List<ScreenModel>();
        internal static string MonitorDeviceName = "";
        internal static int monitorIndeX = 0;

        private static DateTime lastCommentRequestTime = DateTime.MinValue;

        private static IntPtr hangulClockDesktopHwnd = IntPtr.Zero;
        private static IntPtr workerw = IntPtr.Zero;

        private static string hu = "";
        private static string message = "";

        private static double primaryDisplayZoomFactor = 1;
        private static HookKit.ConsoleCtrlHandlerDelegate _consoleHandler;

        private static readonly Application app = new Application();

        [STAThread]
        private static void Main(string[] args)
        {
            NativeDPIAwareSettings();

            if (args != null)
            {
                if (args.Length > 1)
                {
                    if (string.Equals(args[0], "/mindex"))
                    {
                        if (Regex.IsMatch(args[1], @"^\d+$"))
                        {
                            LogKit.Info("HangulClockRenderer will be displayed at Monitor index : " + args[1]);

                            int isRunningInstance = 0;
                            monitorIndeX = Convert.ToInt32(args[1]);

                            Process[] hangulClockRendererProcesses = Process.GetProcessesByName("HangulClockRenderer");

                            foreach (Process hangulClockRendererProcess in hangulClockRendererProcesses)
                            {
                                if (GetCommandLine(hangulClockRendererProcess).Contains($"/mindex {monitorIndeX}"))
                                {
                                    isRunningInstance++;
                                }
                            }

                            if (isRunningInstance <= 1)
                            {
                                _consoleHandler = new HookKit.ConsoleCtrlHandlerDelegate(ConsoleEventHandler);
                                HookKit.SetConsoleCtrlHandler(_consoleHandler, true);

                                AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

                                if (monitorIndeX < System.Windows.Forms.Screen.AllScreens.Count())
                                {
                                    start();
                                }
                                else
                                {
                                    LogKit.Error("Monitor index out of range. Exit.");
                                }
                            }
                            else
                            {
                                LogKit.Error("Already HangulClockRenderer process running at monitor index " + args[1] + ". Exit.");
                            }
                        }
                    }
                }
            }
        }

        private static void NativeDPIAwareSettings()
        {
            if (Environment.OSVersion.Version >= new Version(6, 3, 0)) // Windows 8.0 이상부터 지원하는 함수를 쓸거야
            {
                if (Environment.OSVersion.Version >= new Version(10, 0, 15063)) // Windows 10 크리에이터 업데이트 부터 방식이 바꼈어..
                {
                    HookKit.SetProcessDpiAwarenessContext((int)HookKit.DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
                }
                else
                {
                    HookKit.SetProcessDpiAwareness(HookKit.PROCESS_DPI_AWARENESS.Process_Per_Monitor_DPI_Aware);
                }
            }
            else
            {
                HookKit.SetProcessDPIAware();
            }
        }

        private static void start()
        {
            new Thread(() =>
            {
                hu = new DataKit().Realm.All<HangulClockCommonSetting>().First().hu;
            }).Start();

            LogKit.Info("HangulClockRenderer main thread is started.");

            IntPtr progman = HookKit.FindWindow("Progman", null);
            IntPtr result = IntPtr.Zero;

            HookKit.SendMessageTimeout(progman, 0x052C, new IntPtr(0), IntPtr.Zero, HookKit.SendMessageTimeoutFlags.SMTO_NORMAL, 1000, out result);

            hangulClockDesktop = new HangulClockDesktop();

            HookKit.MonitorEnumDelegate enumDisplayMonitorsCallback = EnumDeviceMonitorDelegate;
            HookKit.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, enumDisplayMonitorsCallback, 0);

            HookKit.NativeDisplay.DISPLAY_DEVICE d = new HookKit.NativeDisplay.DISPLAY_DEVICE();
            d.cb = Marshal.SizeOf(d);

            int mIdx = 0;

            for (uint id = 0; HookKit.EnumDisplayDevices(null, id, ref d, 0); id++)
            {
                d.cb = Marshal.SizeOf(d);

                HookKit.NativeDisplay.DEVMODE dm = HookKit.GetDevMode();

                if (HookKit.EnumDisplaySettingsEx(d.DeviceName, -1, ref dm, 0) != 0)
                {
                    ScreenModel model = screenModels.Find((m) => m.deviceName == d.DeviceName);
                    System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.AllScreens.Where((s) => s.DeviceName == d.DeviceName).FirstOrDefault();
                    model.deviceName = d.DeviceName;
                    model.isPrimary = (d.StateFlags & HookKit.NativeDisplay.DisplayDeviceStateFlags.PrimaryDevice) != 0;
                    model.width = dm.dmPelsWidth;
                    model.height = dm.dmPelsHeight;
                    model.zoomFactor = (double)model.width / screen.Bounds.Width;
                    model.monitorIndex = mIdx;

                    mIdx++;

                    if (model.isPrimary && model.zoomFactor > 1)
                    {
                        primaryDisplayZoomFactor = model.zoomFactor;
                    }

                    model.x = (int)(model.x * primaryDisplayZoomFactor);
                    model.y = (int)(model.y * primaryDisplayZoomFactor);


                    LogKit.Info(string.Format("{0} : Resolution ({1} x {2}) without DPI / XY({3}, {4}) / IsPrimary({5}) / Scale({6})", model.deviceName, model.width, model.height, model.originalX, model.originalY, model.isPrimary, model.zoomFactor));
                }
            }

            MonitorDeviceName = screenModels[monitorIndeX].deviceName;

            // X 축부터 재정렬
            // X 축에 음수가 있다면, 제일 작은 축을 0으로 변경 해줘야 함
            if (screenModels.FindAll((element) => element.x < 0).Count > 0)
            {
                screenModels.Sort((e1, e2) => e1.x.CompareTo(e2.x));

                int minXValue = 0;

                foreach (var item in screenModels.Select((screen, index) => new { index, screen }))
                {
                    ScreenModel screen = item.screen;
                    int index = item.index;

                    if (index == 0)
                    {
                        minXValue = Math.Abs(screen.x);
                    }

                    screen.x = screen.x + minXValue;
                }
            }

            // Y 축도 재정렬
            // Y 축에 음수가 있다면, 제일 작은 축을 0으로 변경 해줘야 함
            if (screenModels.FindAll((element) => element.y < 0).Count > 0)
            {
                screenModels.Sort((e1, e2) => e1.y.CompareTo(e2.y));

                int minYValue = 0;

                foreach (var item in screenModels.Select((screen, index) => new { index, screen }))
                {
                    ScreenModel screen = item.screen;
                    int index = item.index;

                    if (index == 0)
                    {
                        minYValue = Math.Abs(screen.y);
                    }

                    screen.y = screen.y + minYValue;
                }
            }

            LogKit.Info("Inserting HangulClock to index 1 layer in windows explorer workerw...");

            // 탐색기에 후킹
            HookKit.EnumWindows(new HookKit.EnumWindowsProc((topHandle, topParamHandle) =>
            {
                IntPtr p = HookKit.FindWindowEx(topHandle, IntPtr.Zero, "SHELLDLL_DefView", IntPtr.Zero);

                if (p != IntPtr.Zero)
                {
                    workerw = HookKit.FindWindowEx(IntPtr.Zero, topHandle, "WorkerW", IntPtr.Zero);
                }

                return true;
            }), IntPtr.Zero);

            hangulClockDesktop.Loaded += new RoutedEventHandler(async (s, e) =>
            {
                hangulClockDesktopHwnd = new WindowInteropHelper(hangulClockDesktop).Handle;

                ScreenModel currentScreen = screenModels.Where(screen => (screen.monitorIndex == monitorIndeX)).First();

                hangulClockDesktop.WindowStartupLocation = WindowStartupLocation.Manual;
                hangulClockDesktop.Left = currentScreen.originalX;
                hangulClockDesktop.Top = currentScreen.originalY;

                // 만약에 각각의 모니터의 DPI 가 100% 를 넘는게 2개 이상인 경우, {width=0, height=0} 으로는
                // 올바르게 표시되지 않는 문제가 있엉...
                if (screenModels.FindAll((models) => models.zoomFactor > 1).Count <= 1)
                {
                    hangulClockDesktop.Width = 0;
                    hangulClockDesktop.Height = 0;
                }
                else
                {
                    hangulClockDesktop.Left = -10000;
                    hangulClockDesktop.Top = -10000;
                }

                HookKit.SetParent(hangulClockDesktopHwnd, workerw);

                await Task.Delay(1000);

                HookKit.SetWindowPos(hangulClockDesktopHwnd, new IntPtr(0x01), (int)(currentScreen.x / primaryDisplayZoomFactor), (int)(currentScreen.y / primaryDisplayZoomFactor), (int)(currentScreen.width / primaryDisplayZoomFactor), (int)(currentScreen.height / primaryDisplayZoomFactor), HookKit.SetWindowPosFlags.NoActivate | HookKit.SetWindowPosFlags.FrameChanged);
            });

            // hangulClockDesktop.Show();

            LogKit.Info("Boot complete!");

            new Thread(MainThread).Start();
            app.Run(hangulClockDesktop);
        }

        private static void MainThread()
        {
            while (true)
            {
                try
                {
                    DataKit DataKit = new DataKit();
                    DataKit.Realm.Refresh();

                    ClockSettingsByMonitor clockSetting = DataKit.Realm.All<ClockSettingsByMonitor>().Where(c => c.MonitorDeviceName == MonitorDeviceName).First();
                    CommentSettingsByMonitor commentSetting = DataKit.Realm.All<CommentSettingsByMonitor>().Where(c => c.MonitorDeviceName == MonitorDeviceName).First();

                    int clockSize = clockSetting.ClockSize;
                    bool isWhiteClick = clockSetting.IsWhiteClock;
                    string fontName;

                    try
                    {
                        var cvt = new FontConverter();
                        Font f = cvt.ConvertFromString(clockSetting.FontName) as Font;
                        fontName = f.Name;
                    }
                    catch
                    {
                        fontName = "/Resources/#Noto Sans KR Regular";
                    }

                    int clockDirection = commentSetting.DirectionOfComment;

                    string name = commentSetting.Name;
                    string comment = commentSetting.Comment;

                    bool isEnabledCommentLoadFromServer = commentSetting.IsEnabledLoadFromServer;
                    bool isUseCommentInName = commentSetting.IsEnabledNameInComment;

                    if (isEnabledCommentLoadFromServer)
                    {
                        message = loadCommentFromServer();
                    }
                    else
                    {
                        message = comment;
                        lastCommentRequestTime = DateTime.MinValue;
                    }

                    try
                    {
                        if (isUseCommentInName)
                        {
                            HangulKit.HANGUL_INFO partOfName = HangulKit.HangulJaso.DevideJaso(name[name.Length - 1]);
                            if (partOfName.chars[2] == ' ')
                            {
                                comment = string.Format("{0}야, {1}", name, message);
                            }
                            else
                            {
                                comment = string.Format("{0}아, {1}", name, message);
                            }
                        }
                        else
                        {
                            comment = message;
                        }
                    }
                    catch (Exception)
                    {

                    }

                    hangulClockDesktop.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        hangulClockDesktop.ucScale.ScaleX = (double)clockSize / 100;
                        hangulClockDesktop.ucScale.ScaleY = (double)clockSize / 100;

                        hangulClockDesktop.SetClockColor(isWhiteClick);
                        hangulClockDesktop.SetClockFont(fontName);

                        if (clockDirection == CommentSettingsByMonitor.CommentDirection.TOP)
                        {
                            hangulClockDesktop.setTopCommentText(comment);
                        }
                        else if (clockDirection == CommentSettingsByMonitor.CommentDirection.LEFT)
                        {
                            hangulClockDesktop.setLeftCommentText(comment);
                        }
                        else if (clockDirection == CommentSettingsByMonitor.CommentDirection.RIGHT)
                        {
                            hangulClockDesktop.setRightCommentText(comment);
                        }
                        else
                        {
                            hangulClockDesktop.setBottomCommentText(comment);
                        }
                    }));

                    Thread.Sleep(5000);
                }
                catch (ThreadInterruptedException)
                {

                }
            }
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            LogKit.Info("Received current domain exit event. Kill renderer process.");
        }

        private static bool ConsoleEventHandler(ConsoleCtrlHandlerCode eventCode)
        {
            // HookKit.SetParent(hangulClockDesktopHwnd, IntPtr.Zero);

            LogKit.Info("Received Ctrl+C keypress event. Stop renderer process.");
            Environment.Exit(0);

            return false;
        }

        private static bool EnumDeviceMonitorDelegate(IntPtr hDesktop, IntPtr hdc, ref HookKit.RECT prect, int d)
        {
            HookKit.MONITORINFOEX monitorInfo = new HookKit.MONITORINFOEX
            {
                Size = Marshal.SizeOf(typeof(HookKit.MONITORINFOEX))
            };

            HookKit.GetMonitorInfo(hDesktop, ref monitorInfo);

            ScreenModel model = new ScreenModel
            {
                deviceName = monitorInfo.DeviceName,
                originalX = monitorInfo.Monitor.Left,
                originalY = monitorInfo.Monitor.Top,
                x = monitorInfo.Monitor.Left,
                y = monitorInfo.Monitor.Top
            };

            screenModels.Add(model);

            return true;
        }

        private static string loadCommentFromServer()
        {
            try
            {
                if (lastCommentRequestTime == DateTime.MinValue || (lastCommentRequestTime != DateTime.MinValue && (DateTime.Now - lastCommentRequestTime).TotalSeconds > 3600)) // 1시간에 한번씩 문구 체크하기
                {
                    LogKit.Info("Updating comment from server...");

                    lastCommentRequestTime = DateTime.Now;

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(COMMENT_STRING_URL);
                    request.Headers["hu"] = hu;
                    request.Headers["platform"] = "windows";
                    request.Headers["version"] = VersionKit.HANGULCLOCK_VERSION;

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream);

                    string result = reader.ReadToEnd();

                    stream.Close();
                    response.Close();

                    // JSON Parsing

                    JObject obj = JObject.Parse(result);
                    return String.Join("\n", ((string)obj["message"]).Split(','));
                }
                else
                {
                    if (message == "")
                    {
                        message = "남을 위해 사는 착한 사람 말고,너를 위해 사는 좋은 사람이 되길";
                    }

                    return String.Join("\n", message.Split(','));
                }
            }
            catch (Exception e)
            {
                LogKit.Error("Failed to load comment from server.");
                LogKit.Error(e.ToString());
                return String.Join("\n", "남을 위해 사는 착한 사람 말고,너를 위해 사는 좋은 사람이 되길".Split(','));
            }
        }

        private static string GetCommandLine(Process process)
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher($"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {process.Id}"))
            using (ManagementObjectCollection objects = searcher.Get())
            {
                ManagementBaseObject singleOrDefault = objects.Cast<ManagementBaseObject>().SingleOrDefault();

                if (singleOrDefault != null)
                {
                    object commandLine = singleOrDefault["CommandLine"];

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

        private static IEnumerable<string> Split(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }
    }
}
