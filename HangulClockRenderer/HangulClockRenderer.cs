using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

using HangulClockDataKit;
using HangulClockDataKit.Model;
using HangulClockHookKit;
using HangulClockKit;
using HangulClockLogKit;
using HangulClockRenderer.Model;
using Newtonsoft.Json.Linq;

namespace HangulClockRenderer
{
    class HangulClockRenderer
    {
        private const string COMMENT_STRING_URL = "https://us-central1-hangul-clock.cloudfunctions.net/comment/";

        private static HangulClockDesktop hangulClockDesktop;
        private static List<ScreenModel> screenModels = new List<ScreenModel>();
        internal static string MonitorDeviceName = "";
        internal static int monitorIndeX = 0;

        private static DateTime lastCommentRequestTime = DateTime.MinValue;

        private static IntPtr hangulClockDesktopHwnd = IntPtr.Zero;

        private static string hu = "";
        private static string message = "";

        // HANGULCLOCK RENDERER IF WHEN EXIT EVENT RECEIVED
        enum ConsoleCtrlHandlerCode : uint
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }
        delegate bool ConsoleCtrlHandlerDelegate(ConsoleCtrlHandlerCode eventCode);
        [DllImport("kernel32.dll")]
        static extern bool SetConsoleCtrlHandler(ConsoleCtrlHandlerDelegate handlerProc, bool add);
        static ConsoleCtrlHandlerDelegate _consoleHandler;

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,
        }

        private static Application app = new Application();

        [STAThread]
        static void Main(string[] args)
        {
            if (args != null)
            {
                if (args.Length > 1)
                {
                    if (String.Equals(args[0], "/mindex"))
                    {
                        if (Regex.IsMatch(args[1], @"^\d+$"))
                        {
                            Console.WriteLine("Monitor index : " + args[1]);
                            LogKit.Info("Monitor index : " + args[1]);

                            var isRunningInstance = 0;
                            monitorIndeX = Convert.ToInt32(args[1]);

                            Process[] hangulClockRendererProcesses = Process.GetProcessesByName("HangulClockRenderer");

                            foreach (var hangulClockRendererProcess in hangulClockRendererProcesses)
                            {
                                if (GetCommandLine(hangulClockRendererProcess).Contains($"/mindex {monitorIndeX}"))
                                {
                                    isRunningInstance++;
                                }
                            }

                            if (isRunningInstance <= 1)
                            {
                                _consoleHandler = new ConsoleCtrlHandlerDelegate(ConsoleEventHandler);
                                SetConsoleCtrlHandler(_consoleHandler, true);

                                AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

                                start();
                            }
                            else
                            {
                                Console.WriteLine("Already HangulClockRenderer Process running. Exit.");
                            }
                        }
                    }
                }
            }
        }

        private static void start()
        {
            new Thread(() =>
            {
                hu = new DataKit().Realm.All<HangulClockCommonSetting>().First().hu;
            }).Start();

            LogKit.Info("Hangul Clock Renderer process started!");
            Console.Write("Hangul Clock Renderer process started!\n\n");

            IntPtr progman = HookKit.FindWindow("Progman", null);

            IntPtr result = IntPtr.Zero;

            HookKit.SendMessageTimeout(progman, 0x052C, new IntPtr(0), IntPtr.Zero, HookKit.SendMessageTimeoutFlags.SMTO_NORMAL, 1000, out result);

            //

            IntPtr workerw = IntPtr.Zero;

            // System.Windows.Forms.Screen currentScreen = System.Windows.Forms.Screen.AllScreens[monitorIndex];
            // Console.WriteLine(currentScreen.Bounds);

            try
            {
                MonitorDeviceName = System.Windows.Forms.Screen.AllScreens[monitorIndeX].DeviceName;
                foreach (var item in System.Windows.Forms.Screen.AllScreens.Select((value, index) => new { Value = value, Index = index }))
                {
                    ScreenModel model = new ScreenModel();

                    Console.WriteLine(item.Value.Bounds);

                    model.width = item.Value.Bounds.Width;
                    model.height = item.Value.Bounds.Height;
                    model.x = item.Value.Bounds.X;
                    model.y = item.Value.Bounds.Y;

                    model.monitorIndex = item.Index;
                    model.deviceName = item.Value.DeviceName;

                    model.isPrimary = item.Value.Primary;

                    screenModels.Add(model);
                }
            }
            catch (Exception e)
            {
                Application.Current.Shutdown();
            }

            hangulClockDesktop = new HangulClockDesktop();

            HookKit.NativeDisplay.DISPLAY_DEVICE d = new HookKit.NativeDisplay.DISPLAY_DEVICE();
            d.cb = Marshal.SizeOf(d);

            var isRequiredZoomFactorFractal = true;

            for (uint id = 0; HookKit.EnumDisplayDevices(null, id, ref d, 0); id++)
            {
                Console.WriteLine(d.StateFlags);

                if (d.StateFlags == HookKit.NativeDisplay.DisplayDeviceStateFlags.PrimaryDevice
                        || d.StateFlags == HookKit.NativeDisplay.DisplayDeviceStateFlags.AttachedToDesktop
                        || d.StateFlags == (HookKit.NativeDisplay.DisplayDeviceStateFlags.PrimaryDevice | HookKit.NativeDisplay.DisplayDeviceStateFlags.AttachedToDesktop))
                {

                    Console.Write("OK");
                    d.cb = Marshal.SizeOf(d);

                    HookKit.NativeDisplay.DEVMODE dm = HookKit.GetDevMode();

                    if (HookKit.EnumDisplaySettingsEx(d.DeviceName, -1, ref dm, 0) != 0)
                    {
                        foreach (var item in screenModels)
                        {
                            if (String.Equals(item.deviceName, d.DeviceName))
                            {
                                item.zoomFactor = (float)dm.dmPelsWidth / (float)item.width;

                                item.width = dm.dmPelsWidth;
                                item.height = dm.dmPelsHeight;

                                Console.WriteLine(item.zoomFactor);

                                if (item.zoomFactor <= 1.0)
                                {
                                    isRequiredZoomFactorFractal = false;
                                }
                            }
                        }

                        foreach (var item in screenModels)
                        {
                            if (String.Equals(item.deviceName, d.DeviceName))
                            {
                                if (isRequiredZoomFactorFractal)
                                {
                                    // Console.WriteLine("asdfsadfsadfsafdsdfa");
                                    item.x = (int)(item.x * item.zoomFactor);
                                    item.y = (int)(item.y * item.zoomFactor);
                                }
                            }
                        }
                    }
                }
            }

            screenModels.Sort((e1, e2) => e2.x.CompareTo(e1.x));
            screenModels.Sort((e1, e2) => e2.y.CompareTo(e1.y));

            int beforeX = 0;
            int beforeY = 0;

            // var primaryScreen = screenModels.Where(screen => (screen.isPrimary)).First();

            bool requiredSort = false;
            bool isNotChange = false;

            foreach (var item in screenModels)
            {
                if (item.x < 0)
                {
                    requiredSort = true;
                }

                if (requiredSort)
                {
                    if (!isNotChange)
                    {
                        beforeX = Math.Abs(item.x);
                        isNotChange = true;
                    }
                    
                    item.x = item.x + beforeX; // 0
                }
            }

            requiredSort = false;
            isNotChange = false;

            foreach (var item in screenModels)
            {
                if (item.y < 0)
                {
                    requiredSort = true;
                }

                if (requiredSort)
                {
                    if (!isNotChange)
                    {
                        beforeY = Math.Abs(item.y);
                        isNotChange = true;
                    }
                        
                    item.y = item.y + beforeY;
                }
            }

            HookKit.EnumWindows(new HookKit.EnumWindowsProc((topHandle, topParamHandle) =>
            {
                IntPtr p = HookKit.FindWindowEx(topHandle, IntPtr.Zero, "SHELLDLL_DefView", IntPtr.Zero);

                if (p != IntPtr.Zero)
                {
                    workerw = HookKit.FindWindowEx(IntPtr.Zero, topHandle, "WorkerW", IntPtr.Zero);
                }

                return true;
            }), IntPtr.Zero);

            //  IntPtr dc = HookKit.GetDCEx(workerw, IntPtr.Zero, (HookKit.DeviceContextValues)0x403);

            hangulClockDesktop.Loaded += new RoutedEventHandler(async (s, e) =>
            {
                // const UInt32 WS_POPUP = 0x80000000;
                // const UInt32 WS_CHILD = 0x40000000;
                // UInt32 style = (UInt32)HookKit.GetWindowLong(hangulClockDesktopHwnd, -16);
                // style = (style & ~(WS_POPUP)) | WS_CHILD; // | 0x00C00000 | 0x10000000 | 0x04000000 | 0x02000000;

                hangulClockDesktopHwnd = new WindowInteropHelper(hangulClockDesktop).Handle;

                HookKit.SetParent(hangulClockDesktopHwnd, workerw);

                // HookKit.SetWindowLong(hangulClockDesktopHwnd, HookKit.WindowLongFlags.GWL_STYLE, (int)style);

                var currentScreen = screenModels.Where(screen => (screen.monitorIndex == monitorIndeX)).First();

                /* hangulClockDesktop.Width = currentScreen.width;
                hangulClockDesktop.Height = currentScreen.height;

                hangulClockDesktop.Left = currentScreen.x;
                hangulClockDesktop.Top = currentScreen.y; */

                Console.WriteLine(String.Format("x : {0}, y : {1} / {2} x {3}", currentScreen.x, currentScreen.y, currentScreen.width, currentScreen.height));
                HookKit.MoveWindow(hangulClockDesktopHwnd, currentScreen.x, currentScreen.y, (int)(currentScreen.width), (int)(currentScreen.height), true);
            });

            // hangulClockDesktop.Show();

            new Thread(MainThread).Start();
            app.Run(hangulClockDesktop);
        }

        private static void MainThread()
        {
            var DataKit = new DataKit();
            
            while (true)
            {
                try
                {
                    DataKit.Realm.Refresh();

                    var clockSetting = DataKit.Realm.All<ClockSettingsByMonitor>().Where(c => c.MonitorDeviceName == MonitorDeviceName).First();
                    var commentSetting = DataKit.Realm.All<CommentSettingsByMonitor>().Where(c => c.MonitorDeviceName == MonitorDeviceName).First();

                    int clockSize = clockSetting.ClockSize;
                    bool isWhiteClick = clockSetting.IsWhiteClock;
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
                                comment = String.Format("{0}야, {1}", name, message);
                            }
                            else
                            {
                                comment = String.Format("{0}아, {1}", name, message);
                            }
                        }
                        else
                        {
                            comment = message;
                        }
                    }
                    catch (Exception e)
                    {

                    }

                    hangulClockDesktop.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        hangulClockDesktop.ucScale.ScaleX = (double)clockSize / 100;
                        hangulClockDesktop.ucScale.ScaleY = (double)clockSize / 100;

                        hangulClockDesktop.SetClockColor(isWhiteClick);

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

                    Thread.Sleep(300);
                }
                catch(ThreadInterruptedException e)
                {

                }
            }
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            HookKit.SetParent(hangulClockDesktopHwnd, IntPtr.Zero);
            Console.WriteLine("HangulClockRenderer : Kill!");
            // Environment.Exit(0);
        }

        static bool ConsoleEventHandler(ConsoleCtrlHandlerCode eventCode)
        {
            HookKit.SetParent(hangulClockDesktopHwnd, IntPtr.Zero); 
            /* if (hangulClockDesktop != null)
            {
                hangulClockDesktop.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    hangulClockDesktop.Close();
                }));
            } */

            /* app.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                app.Shutdown();
            })); */

            // HookKit.SendMessage(hangulClockDesktopHwnd, HookKit.WM_SYSCOMMAND, HookKit.SC_CLOSE, IntPtr.Zero);
            //HookKit.MoveWindow(hangulClockDesktopHwnd, 0, 0, 0, 0, true);

            Console.WriteLine("HangulClockRenderer : Stop!");
            Environment.Exit(0);

            return (false);
        }

        private static string loadCommentFromServer()
        {
            try
            {
                if (lastCommentRequestTime == DateTime.MinValue || (lastCommentRequestTime != DateTime.MinValue && (DateTime.Now - lastCommentRequestTime).TotalSeconds > 3600)) // 1시간에 한번씩 문구 체크하기
                {
                    Console.WriteLine("Updating comment from server...");

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
                    return obj["message"].ToString();
                }
                else
                {
                    if (message == "")
                    {
                        message = "오늘도 너가 있어 아름다워.";
                    }

                    return message;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to load comment.");
                return "오늘도 너가 있어 아름다워.";
            }
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
