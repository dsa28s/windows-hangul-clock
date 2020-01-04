using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Tools;

namespace HangulClock
{
    /// <summary>
    /// DashboardTab.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MultiMonitorSelectPage : System.Windows.Controls.UserControl
    {
        public MultiMonitorSelectPage()
        {
            InitializeComponent();

            display1.Visibility = Visibility.Hidden;
            display1_Text.Visibility = Visibility.Hidden;
            display1_Border.Visibility = Visibility.Hidden;
            display2.Visibility = Visibility.Hidden;
            display2_Text.Visibility = Visibility.Hidden;
            display2_Border.Visibility = Visibility.Hidden;
            display3.Visibility = Visibility.Hidden;
            display3_Text.Visibility = Visibility.Hidden;
            display3_Border.Visibility = Visibility.Hidden;
            display4.Visibility = Visibility.Hidden;
            display4_Text.Visibility = Visibility.Hidden;
            display4_Border.Visibility = Visibility.Hidden;
            display5.Visibility = Visibility.Hidden;
            display5_Text.Visibility = Visibility.Hidden;
            display5_Border.Visibility = Visibility.Hidden;
            display6.Visibility = Visibility.Hidden;
            display6_Text.Visibility = Visibility.Hidden;
            display6_Border.Visibility = Visibility.Hidden;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var index = 0;
            foreach (System.Windows.Forms.Screen s in System.Windows.Forms.Screen.AllScreens)
            {
                switch (index)
                {
                    case 0:
                        display1.Visibility = Visibility.Visible;
                        display1_Text.Visibility = Visibility.Visible;
                        display1_Border.Visibility = Visibility.Visible;

                        display1_Text.Content = s.DeviceFriendlyName() + ((s.Primary) ? " (주 모니터)" : "");

                        renderCapturedImage(display1, s);

                        break;
                    case 1:
                        display2.Visibility = Visibility.Visible;
                        display2_Text.Visibility = Visibility.Visible;
                        display2_Border.Visibility = Visibility.Visible;

                        display2_Text.Content = s.DeviceFriendlyName() + ((s.Primary) ? " (주 모니터)" : "");

                        renderCapturedImage(display2, s);
                        break;
                    case 2:
                        display3.Visibility = Visibility.Visible;
                        display3_Text.Visibility = Visibility.Visible;
                        display3_Border.Visibility = Visibility.Visible;

                        display3_Text.Content = s.DeviceFriendlyName() + ((s.Primary) ? " (주 모니터)" : "");

                        renderCapturedImage(display3, s);
                        break;
                    case 3:
                        display4.Visibility = Visibility.Visible;
                        display4_Text.Visibility = Visibility.Visible;
                        display4_Border.Visibility = Visibility.Visible;

                        display4_Text.Content = s.DeviceFriendlyName() + ((s.Primary) ? " (주 모니터)" : "");

                        renderCapturedImage(display4, s);
                        break;
                    case 4:
                        display5.Visibility = Visibility.Visible;
                        display5_Text.Visibility = Visibility.Visible;
                        display5_Border.Visibility = Visibility.Visible;

                        display5_Text.Content = s.DeviceFriendlyName() + ((s.Primary) ? " (주 모니터)" : "");

                        renderCapturedImage(display5, s);
                        break;
                    case 5:
                        display6.Visibility = Visibility.Visible;
                        display6_Text.Visibility = Visibility.Visible;
                        display6_Border.Visibility = Visibility.Visible;

                        display6_Text.Content = s.DeviceFriendlyName() + ((s.Primary) ? " (주 모니터)" : "");

                        renderCapturedImage(display6, s);
                        break;
                }

                index++;
            }
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void renderCapturedImage(System.Windows.Controls.Image i, System.Windows.Forms.Screen s)
        {
            System.Drawing.Size size = s.Bounds.Size;
            Bitmap b = new Bitmap(s.Bounds.Width, s.Bounds.Height);
            Graphics g = Graphics.FromImage(b as System.Drawing.Image);
            g.CopyFromScreen(s.Bounds.X, s.Bounds.Y, 0, 0, size);

            i.Source = BitmapToImageSource(b);
        }

        private void display1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (System.Windows.Forms.Screen.AllScreens.Length > 0)
            {
                MainWindow.setCurrentMonitor(System.Windows.Forms.Screen.AllScreens[0].DeviceName);
                MainWindow.activeTab = HangulClockUIKit.UIKit.HangulClockTab.DASHBOARD;
                MainWindow.updateTabStatus();
                MainWindow.pager.ShowPage(MainWindow.dashboardTab);
                MainWindow.dashboardTab.loadInitData();
            }
        }

        private void display2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (System.Windows.Forms.Screen.AllScreens.Length > 1)
            {
                MainWindow.setCurrentMonitor(System.Windows.Forms.Screen.AllScreens[1].DeviceName);
                MainWindow.activeTab = HangulClockUIKit.UIKit.HangulClockTab.DASHBOARD;
                MainWindow.updateTabStatus();
                MainWindow.pager.ShowPage(MainWindow.dashboardTab);
                MainWindow.dashboardTab.loadInitData();
            }
        }

        private void display3_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (System.Windows.Forms.Screen.AllScreens.Length > 2)
            {
                MainWindow.setCurrentMonitor(System.Windows.Forms.Screen.AllScreens[2].DeviceName);
                MainWindow.activeTab = HangulClockUIKit.UIKit.HangulClockTab.DASHBOARD;
                MainWindow.updateTabStatus();
                MainWindow.pager.ShowPage(MainWindow.dashboardTab);
                MainWindow.dashboardTab.loadInitData();
            }
        }

        private void display4_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (System.Windows.Forms.Screen.AllScreens.Length > 3)
            {
                MainWindow.setCurrentMonitor(System.Windows.Forms.Screen.AllScreens[3].DeviceName);
                MainWindow.activeTab = HangulClockUIKit.UIKit.HangulClockTab.DASHBOARD;
                MainWindow.updateTabStatus();
                MainWindow.pager.ShowPage(MainWindow.dashboardTab);
                MainWindow.dashboardTab.loadInitData();
            }
        }

        private void display5_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (System.Windows.Forms.Screen.AllScreens.Length > 4)
            {
                MainWindow.setCurrentMonitor(System.Windows.Forms.Screen.AllScreens[4].DeviceName);
                MainWindow.activeTab = HangulClockUIKit.UIKit.HangulClockTab.DASHBOARD;
                MainWindow.updateTabStatus();
                MainWindow.pager.ShowPage(MainWindow.dashboardTab);
                MainWindow.dashboardTab.loadInitData();
            }
        }

        private void display6_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (System.Windows.Forms.Screen.AllScreens.Length > 5)
            {
                MainWindow.setCurrentMonitor(System.Windows.Forms.Screen.AllScreens[5].DeviceName);
                MainWindow.activeTab = HangulClockUIKit.UIKit.HangulClockTab.DASHBOARD;
                MainWindow.updateTabStatus();
                MainWindow.pager.ShowPage(MainWindow.dashboardTab);
                MainWindow.dashboardTab.loadInitData();
            }
        }
    }
}


namespace Tools
{
    public static class ScreenInterrogatory
    {
        public const int ERROR_SUCCESS = 0;

        #region enums

        public enum QUERY_DEVICE_CONFIG_FLAGS : uint
        {
            QDC_ALL_PATHS = 0x00000001,
            QDC_ONLY_ACTIVE_PATHS = 0x00000002,
            QDC_DATABASE_CURRENT = 0x00000004
        }

        public enum DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY : uint
        {
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_OTHER = 0xFFFFFFFF,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HD15 = 0,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SVIDEO = 1,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPOSITE_VIDEO = 2,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPONENT_VIDEO = 3,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DVI = 4,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HDMI = 5,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_LVDS = 6,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_D_JPN = 8,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDI = 9,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EXTERNAL = 10,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EMBEDDED = 11,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EXTERNAL = 12,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EMBEDDED = 13,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDTVDONGLE = 14,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_MIRACAST = 15,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INTERNAL = 0x80000000,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_FORCE_UINT32 = 0xFFFFFFFF
        }

        public enum DISPLAYCONFIG_SCANLINE_ORDERING : uint
        {
            DISPLAYCONFIG_SCANLINE_ORDERING_UNSPECIFIED = 0,
            DISPLAYCONFIG_SCANLINE_ORDERING_PROGRESSIVE = 1,
            DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED = 2,
            DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_UPPERFIELDFIRST = DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED,
            DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_LOWERFIELDFIRST = 3,
            DISPLAYCONFIG_SCANLINE_ORDERING_FORCE_UINT32 = 0xFFFFFFFF
        }

        public enum DISPLAYCONFIG_ROTATION : uint
        {
            DISPLAYCONFIG_ROTATION_IDENTITY = 1,
            DISPLAYCONFIG_ROTATION_ROTATE90 = 2,
            DISPLAYCONFIG_ROTATION_ROTATE180 = 3,
            DISPLAYCONFIG_ROTATION_ROTATE270 = 4,
            DISPLAYCONFIG_ROTATION_FORCE_UINT32 = 0xFFFFFFFF
        }

        public enum DISPLAYCONFIG_SCALING : uint
        {
            DISPLAYCONFIG_SCALING_IDENTITY = 1,
            DISPLAYCONFIG_SCALING_CENTERED = 2,
            DISPLAYCONFIG_SCALING_STRETCHED = 3,
            DISPLAYCONFIG_SCALING_ASPECTRATIOCENTEREDMAX = 4,
            DISPLAYCONFIG_SCALING_CUSTOM = 5,
            DISPLAYCONFIG_SCALING_PREFERRED = 128,
            DISPLAYCONFIG_SCALING_FORCE_UINT32 = 0xFFFFFFFF
        }

        public enum DISPLAYCONFIG_PIXELFORMAT : uint
        {
            DISPLAYCONFIG_PIXELFORMAT_8BPP = 1,
            DISPLAYCONFIG_PIXELFORMAT_16BPP = 2,
            DISPLAYCONFIG_PIXELFORMAT_24BPP = 3,
            DISPLAYCONFIG_PIXELFORMAT_32BPP = 4,
            DISPLAYCONFIG_PIXELFORMAT_NONGDI = 5,
            DISPLAYCONFIG_PIXELFORMAT_FORCE_UINT32 = 0xffffffff
        }

        public enum DISPLAYCONFIG_MODE_INFO_TYPE : uint
        {
            DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE = 1,
            DISPLAYCONFIG_MODE_INFO_TYPE_TARGET = 2,
            DISPLAYCONFIG_MODE_INFO_TYPE_FORCE_UINT32 = 0xFFFFFFFF
        }

        public enum DISPLAYCONFIG_DEVICE_INFO_TYPE : uint
        {
            DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME = 1,
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME = 2,
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_PREFERRED_MODE = 3,
            DISPLAYCONFIG_DEVICE_INFO_GET_ADAPTER_NAME = 4,
            DISPLAYCONFIG_DEVICE_INFO_SET_TARGET_PERSISTENCE = 5,
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_BASE_TYPE = 6,
            DISPLAYCONFIG_DEVICE_INFO_FORCE_UINT32 = 0xFFFFFFFF
        }

        #endregion

        #region structs

        [StructLayout(LayoutKind.Sequential)]
        public struct LUID
        {
            public uint LowPart;
            public int HighPart;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_PATH_SOURCE_INFO
        {
            public LUID adapterId;
            public uint id;
            public uint modeInfoIdx;
            public uint statusFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_PATH_TARGET_INFO
        {
            public LUID adapterId;
            public uint id;
            public uint modeInfoIdx;
            private DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology;
            private DISPLAYCONFIG_ROTATION rotation;
            private DISPLAYCONFIG_SCALING scaling;
            private DISPLAYCONFIG_RATIONAL refreshRate;
            private DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
            public bool targetAvailable;
            public uint statusFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_RATIONAL
        {
            public uint Numerator;
            public uint Denominator;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_PATH_INFO
        {
            public DISPLAYCONFIG_PATH_SOURCE_INFO sourceInfo;
            public DISPLAYCONFIG_PATH_TARGET_INFO targetInfo;
            public uint flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_2DREGION
        {
            public uint cx;
            public uint cy;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_VIDEO_SIGNAL_INFO
        {
            public ulong pixelRate;
            public DISPLAYCONFIG_RATIONAL hSyncFreq;
            public DISPLAYCONFIG_RATIONAL vSyncFreq;
            public DISPLAYCONFIG_2DREGION activeSize;
            public DISPLAYCONFIG_2DREGION totalSize;
            public uint videoStandard;
            public DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_TARGET_MODE
        {
            public DISPLAYCONFIG_VIDEO_SIGNAL_INFO targetVideoSignalInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTL
        {
            private int x;
            private int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_SOURCE_MODE
        {
            public uint width;
            public uint height;
            public DISPLAYCONFIG_PIXELFORMAT pixelFormat;
            public POINTL position;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct DISPLAYCONFIG_MODE_INFO_UNION
        {
            [FieldOffset(0)]
            public DISPLAYCONFIG_TARGET_MODE targetMode;

            [FieldOffset(0)]
            public DISPLAYCONFIG_SOURCE_MODE sourceMode;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_MODE_INFO
        {
            public DISPLAYCONFIG_MODE_INFO_TYPE infoType;
            public uint id;
            public LUID adapterId;
            public DISPLAYCONFIG_MODE_INFO_UNION modeInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS
        {
            public uint value;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_DEVICE_INFO_HEADER
        {
            public DISPLAYCONFIG_DEVICE_INFO_TYPE type;
            public uint size;
            public LUID adapterId;
            public uint id;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct DISPLAYCONFIG_TARGET_DEVICE_NAME
        {
            public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
            public DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS flags;
            public DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology;
            public ushort edidManufactureId;
            public ushort edidProductCodeId;
            public uint connectorInstance;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string monitorFriendlyDeviceName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string monitorDevicePath;
        }

        #endregion

        #region DLL-Imports

        [DllImport("user32.dll")]
        public static extern int GetDisplayConfigBufferSizes(
            QUERY_DEVICE_CONFIG_FLAGS flags, out uint numPathArrayElements, out uint numModeInfoArrayElements);

        [DllImport("user32.dll")]
        public static extern int QueryDisplayConfig(
            QUERY_DEVICE_CONFIG_FLAGS flags,
            ref uint numPathArrayElements, [Out] DISPLAYCONFIG_PATH_INFO[] PathInfoArray,
            ref uint numModeInfoArrayElements, [Out] DISPLAYCONFIG_MODE_INFO[] ModeInfoArray,
            IntPtr currentTopologyId
            );

        [DllImport("user32.dll")]
        public static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_TARGET_DEVICE_NAME deviceName);

        #endregion

        private static string MonitorFriendlyName(LUID adapterId, uint targetId)
        {
            var deviceName = new DISPLAYCONFIG_TARGET_DEVICE_NAME
            {
                header =
                {
                    size = (uint)Marshal.SizeOf(typeof (DISPLAYCONFIG_TARGET_DEVICE_NAME)),
                    adapterId = adapterId,
                    id = targetId,
                    type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME
                }
            };
            var error = DisplayConfigGetDeviceInfo(ref deviceName);
            if (error != ERROR_SUCCESS)
                throw new Win32Exception(error);
            return deviceName.monitorFriendlyDeviceName;
        }

        private static IEnumerable<string> GetAllMonitorsFriendlyNames()
        {
            uint pathCount, modeCount;
            var error = GetDisplayConfigBufferSizes(QUERY_DEVICE_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS, out pathCount, out modeCount);
            if (error != ERROR_SUCCESS)
                throw new Win32Exception(error);

            var displayPaths = new DISPLAYCONFIG_PATH_INFO[pathCount];
            var displayModes = new DISPLAYCONFIG_MODE_INFO[modeCount];
            error = QueryDisplayConfig(QUERY_DEVICE_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS,
                ref pathCount, displayPaths, ref modeCount, displayModes, IntPtr.Zero);
            if (error != ERROR_SUCCESS)
                throw new Win32Exception(error);

            for (var i = 0; i < modeCount; i++)
                if (displayModes[i].infoType == DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_TARGET)
                    yield return MonitorFriendlyName(displayModes[i].adapterId, displayModes[i].id);
        }

        public static string DeviceFriendlyName(this Screen screen)
        {
            var allFriendlyNames = GetAllMonitorsFriendlyNames();
            for (var index = 0; index < Screen.AllScreens.Length; index++)
                if (Equals(screen, Screen.AllScreens[index]))
                    return allFriendlyNames.ToArray()[index];
            return null;
        }

    }

}
