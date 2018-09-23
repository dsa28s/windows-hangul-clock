using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using HangulClockHookKit;
using HangulClockLogKit;
using HangulClockRenderer.Model;

namespace HangulClockRenderer
{
    class HangulClockRenderer
    {
        private static HangulClockDesktop hangulClockDesktop;
        private static List<ScreenModel> screenModels = new List<ScreenModel>();
        internal static int monitorIndeX = 0;

        private static IntPtr hangulClockDesktopHwnd = IntPtr.Zero;

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

                            monitorIndeX = Convert.ToInt32(args[1]);

                            _consoleHandler = new ConsoleCtrlHandlerDelegate(ConsoleEventHandler);
                            SetConsoleCtrlHandler(_consoleHandler, true);

                            start();
                        }
                    }
                }
            }
        }

        private static void start()
        {
            hangulClockDesktop = new HangulClockDesktop();

            LogKit.Info("Hangul Clock Renderer process started!");
            Console.Write("Hangul Clock Renderer process started!\n\n");

            IntPtr progman = HookKit.FindWindow("Progman", null);

            IntPtr result = IntPtr.Zero;

            HookKit.SendMessageTimeout(progman, 0x052C, new IntPtr(0), IntPtr.Zero, HookKit.SendMessageTimeoutFlags.SMTO_NORMAL, 1000, out result);

            //

            IntPtr workerw = IntPtr.Zero;

            // System.Windows.Forms.Screen currentScreen = System.Windows.Forms.Screen.AllScreens[monitorIndex];
            // Console.WriteLine(currentScreen.Bounds);

            foreach (var item in System.Windows.Forms.Screen.AllScreens.Select((value, index) => new { Value = value, Index = index }))
            {
                ScreenModel model = new ScreenModel();

                model.width = item.Value.Bounds.Width;
                model.height = item.Value.Bounds.Height;
                model.x = item.Value.Bounds.X;
                model.y = item.Value.Bounds.Y;

                model.monitorIndex = item.Index;
                model.deviceName = item.Value.DeviceName;

                model.isPrimary = item.Value.Primary;

                screenModels.Add(model);
            }

            HookKit.NativeDisplay.DISPLAY_DEVICE d = new HookKit.NativeDisplay.DISPLAY_DEVICE();
            d.cb = Marshal.SizeOf(d);

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

                                item.x = (int)(item.x * item.zoomFactor);
                                item.y = (int)(item.y * item.zoomFactor);
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

            new Thread(new ThreadStart(MainThread)).Start();
            app.Run(hangulClockDesktop);
        }

        private static void MainThread()
        {
            while(true)
            {
                try
                {
                    hangulClockDesktop.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        hangulClockDesktop.ucScale.ScaleX = 1.5;
                        hangulClockDesktop.ucScale.ScaleY = 1.5;

                        hangulClockDesktop.setRightCommentText("누군가는 너를 필요로하겠지.\n\n그 사람때문이라도\n힘내줬으면 해.");
                    }));

                    Thread.Sleep(1000);
                }
                catch(ThreadInterruptedException e)
                {

                }
            }
        }

        static bool ConsoleEventHandler(ConsoleCtrlHandlerCode eventCode)
        {
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

            Console.Write("HangulClockRenderer : Stop!");
            Environment.Exit(0);

            return (false);
        }

        private static float getScalingFactor(IntPtr hwnd)
        {
            Graphics g = Graphics.FromHwnd(hwnd);
            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

            float ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;

            return ScreenScalingFactor; // 1.25 = 125%
        }
    }
}
