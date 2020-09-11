using HangulClockDataKit;
using HangulClockDataKit.Model;
using HangulClockHookKit;
using HangulClockUIKit.Weather;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HangulClockRenderer
{
    /// <summary>
    /// HangulClockDesktop.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class HangulClockDesktop : Window
    {
        private SnowEngine snow = null;
        private string systemBackgroundPath = "";
        private string backgroundPath = "";
        private string solidColor = "";
        private string youtubeVCode = "";

        public HangulClockDesktop()
        {
            InitializeComponent();
        }

        private void HideWindowFromAltTab()
        {
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);

            int exStyle = HookKit.GetWindowLong(wndHelper.Handle, (int)HookKit.GetWindowLongFields.GWL_EXSTYLE);

            exStyle |= (int)HookKit.ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            HookKit.SetWindowLong(wndHelper.Handle, HookKit.WindowLongFlags.GWL_EXSTYLE, exStyle);
        }

        private void CheckBackgroundChange()
        {
            DataKit DataKit = new DataKit();
            while (true)
            {
                DataKit.Realm.Refresh();
                BackgroundSettingsByMonitor backgroundSetting = DataKit.Realm.All<BackgroundSettingsByMonitor>().Where(c => c.MonitorDeviceName == Renderer.MonitorDeviceName).First();

                bool isSolid = backgroundSetting.backgroundType == BackgroundSettingsByMonitor.BackgroundType.SOLID;
                bool isYoutube = backgroundSetting.backgroundType == BackgroundSettingsByMonitor.BackgroundType.YOUTUBE_VIDEO;

                // caption-window;
                if (isYoutube)
                {
                    youtubeView.GetBrowser().MainFrame.EvaluateScriptAsync("if (typeof css === 'undefined') { var css = '.ytp-caption-window-bottom { display: none !important }', head = document.head, style = document.createElement('style'); style.type = 'text/css'; style.appendChild(document.createTextNode(css)); head.appendChild(style); }");
                    youtubeView.GetBrowser().MainFrame.EvaluateScriptAsync("if (typeof css2 === 'undefined') { var css2 = '.video-annotations { display: none !important }', head = document.head, style = document.createElement('style'); style.type = 'text/css'; style.appendChild(document.createTextNode(css2)); head.appendChild(style); }");
                }

                Dispatcher.Invoke(new Action(() =>
                {
                    if (!isSolid)
                    {
                        BrushConverter bc = new BrushConverter();
                        overlayView.Background = (Brush)bc.ConvertFrom("#10000000");
                    }
                }));

                if (backgroundSetting.backgroundType == BackgroundSettingsByMonitor.BackgroundType.DEFAULT)
                {
                    string path = DesktopWallpaperGenerator.GetBackgroud();

                    if (systemBackgroundPath != path)
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            backgroundImage.Source = new BitmapImage(new Uri(path));

                            youtubeView.Address = "";
                            youtubeView.Visibility = Visibility.Hidden;
                        }));

                        systemBackgroundPath = path;
                        backgroundPath = "";
                        youtubeVCode = "";
                        solidColor = "";
                    }
                }
                else if (backgroundSetting.backgroundType == BackgroundSettingsByMonitor.BackgroundType.STILL_IMAGE)
                {
                    string path = backgroundSetting.imgPath;

                    if (File.Exists(path))
                    {
                        if (backgroundPath != path)
                        {
                            backgroundPath = backgroundSetting.imgPath;

                            Dispatcher.Invoke(new Action(() =>
                            {
                                if (File.Exists(backgroundPath))
                                {
                                    backgroundImage.Source = new BitmapImage(new Uri(backgroundPath));
                                }
                                else
                                {
                                    systemBackgroundPath = DesktopWallpaperGenerator.GetBackgroud();
                                    backgroundImage.Source = new BitmapImage(new Uri(systemBackgroundPath));
                                }

                                BrushConverter bc = new BrushConverter();
                                overlayView.Background = (Brush)bc.ConvertFrom("#10000000");

                                youtubeView.Address = "";
                                youtubeView.Visibility = Visibility.Hidden;

                                systemBackgroundPath = "";
                                youtubeVCode = "";
                                solidColor = "";
                            }));
                        }
                    }
                    else
                    {
                        string spath = DesktopWallpaperGenerator.GetBackgroud();

                        if (systemBackgroundPath != spath)
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                backgroundImage.Source = new BitmapImage(new Uri(spath));

                                systemBackgroundPath = spath;

                                youtubeView.Address = "";
                                youtubeView.Visibility = Visibility.Hidden;
                            }));

                            backgroundPath = "";
                            youtubeVCode = "";
                            solidColor = "";
                        }
                    }
                }
                else if (backgroundSetting.backgroundType == BackgroundSettingsByMonitor.BackgroundType.SOLID)
                {
                    string solid = backgroundSetting.SolidColor;

                    if (solidColor != solid)
                    {
                        solidColor = solid;
                        Dispatcher.Invoke(new Action(() =>
                        {
                            // backgroundImage.Source = null;

                            BrushConverter bc = new BrushConverter();
                            overlayView.Background = (Brush)bc.ConvertFrom("#" + solidColor);

                            youtubeView.Address = "";
                            youtubeView.Visibility = Visibility.Hidden;

                            systemBackgroundPath = "";
                            backgroundPath = "";
                            youtubeVCode = "";
                        }));
                    }
                }
                else
                {
                    string youtubeVideo = backgroundSetting.YoutubeURL;

                    if (youtubeVCode != youtubeVideo)
                    {
                        youtubeVCode = youtubeVideo;
                        Dispatcher.Invoke(new Action(() =>
                        {
                            if (youtubeVideo == "" || youtubeVideo == null)
                            {
                                backgroundPath = DesktopWallpaperGenerator.GetBackgroud();
                                backgroundImage.Source = new BitmapImage(new Uri(backgroundPath));
                            }
                            else
                            {
                                youtubeView.Visibility = Visibility.Visible;
                                youtubeView.Address = string.Format("https://www.youtube.com/embed/{0}?controls=0&showinfo=0&rel=0&autoplay=1&loop=1&mute=1&cc_load_policy=3&playlist={0}", youtubeVCode);
                                // youtubeView.GetBrowser().MainFrame.EvaluateScriptAsync("setInterval(() => { document.getElementsByClassName('caption-window ytp-caption-window-bottom')[0].style.display = 'none'; console.clear(); }, 100);");
                            }

                            BrushConverter bc = new BrushConverter();
                            overlayView.Background = (Brush)bc.ConvertFrom("#10000000");

                            systemBackgroundPath = "";
                            backgroundPath = "";
                            solidColor = "";
                        }));
                    }
                }

                // Console.WriteLine("Communicating HangulClock Data Kit...");

                try
                {
                    Thread.Sleep(5000);
                }
                catch (ThreadInterruptedException)
                {

                }
            }

        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

            snow = new SnowEngine(weatherCanvas, "pack://application:,,,/Resources/snow.png")
            {
                SnowCoverage = 1,
                VerticalSpeedRatio = 0.3,
                HorizontalSpeedRatio = 0
            };
            //snow.Start();

            hangulClockTopComment.setTextSize(30);
            hangulClockBottomComment.setTextSize(30);
            hangulClockLeftComment.setTextSize(30);
            hangulClockRightComment.setTextSize(30);
        }

        public void setTopCommentText(string text)
        {
            hangulClockTopComment.setContent(text);
            hangulClockLeftComment.setContent("");
            hangulClockRightComment.setContent("");
            hangulClockBottomComment.setContent("");
        }

        public void setLeftCommentText(string text)
        {
            hangulClockTopComment.setContent("");
            hangulClockLeftComment.setContent(text);
            hangulClockRightComment.setContent("");
            hangulClockBottomComment.setContent("");
        }

        public void setRightCommentText(string text)
        {
            hangulClockTopComment.setContent("");
            hangulClockLeftComment.setContent("");
            hangulClockRightComment.setContent(text);
            hangulClockBottomComment.setContent("");
        }

        public void setBottomCommentText(string text)
        {
            hangulClockTopComment.setContent("");
            hangulClockLeftComment.setContent("");
            hangulClockRightComment.setContent("");
            hangulClockBottomComment.setContent(text);
        }

        public void SetClockColor(bool isWhite)
        {
            BrushConverter bc = new BrushConverter();

            if (isWhite)
            {
                hangulClock.Visibility = Visibility.Visible;
                hangulClockBlack.Visibility = Visibility.Hidden;

                hangulClockTopComment.Foreground = (Brush)bc.ConvertFrom("#FFFFFF");
                hangulClockLeftComment.Foreground = (Brush)bc.ConvertFrom("#FFFFFF");
                hangulClockRightComment.Foreground = (Brush)bc.ConvertFrom("#FFFFFF");
                hangulClockBottomComment.Foreground = (Brush)bc.ConvertFrom("#FFFFFF");
            }
            else
            {
                hangulClock.Visibility = Visibility.Hidden;
                hangulClockBlack.Visibility = Visibility.Visible;

                hangulClockTopComment.Foreground = (Brush)bc.ConvertFrom("#000000");
                hangulClockLeftComment.Foreground = (Brush)bc.ConvertFrom("#000000");
                hangulClockRightComment.Foreground = (Brush)bc.ConvertFrom("#000000");
                hangulClockBottomComment.Foreground = (Brush)bc.ConvertFrom("#000000");
            }
        }

        public void SetClockFont(string fontName)
        {
            BrushConverter bc = new BrushConverter();
            hangulClock.FontFamily = new FontFamily(fontName);
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {

        }

        private void Desktop_Loaded(object sender, RoutedEventArgs e)
        {
            HideWindowFromAltTab();

            new Thread(() =>
            {
                DataKit DataKit = new DataKit();
                BackgroundSettingsByMonitor backgroundSetting = DataKit.Realm.All<BackgroundSettingsByMonitor>().Where(c => c.MonitorDeviceName == Renderer.MonitorDeviceName).First();

                if (backgroundSetting.backgroundType == BackgroundSettingsByMonitor.BackgroundType.DEFAULT)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        systemBackgroundPath = DesktopWallpaperGenerator.GetBackgroud();
                        backgroundImage.Source = new BitmapImage(new Uri(systemBackgroundPath));

                        youtubeView.Address = "";
                        youtubeView.Visibility = Visibility.Hidden;
                    }));
                }
                else if (backgroundSetting.backgroundType == BackgroundSettingsByMonitor.BackgroundType.STILL_IMAGE)
                {
                    backgroundPath = backgroundSetting.imgPath;

                    Dispatcher.Invoke(new Action(() =>
                    {
                        if (File.Exists(backgroundPath))
                        {
                            backgroundImage.Source = new BitmapImage(new Uri(backgroundPath));
                        }
                        else
                        {
                            systemBackgroundPath = DesktopWallpaperGenerator.GetBackgroud();
                            backgroundImage.Source = new BitmapImage(new Uri(systemBackgroundPath));
                        }

                        youtubeView.Address = "";
                        youtubeView.Visibility = Visibility.Hidden;
                    }));
                }
                else if (backgroundSetting.backgroundType == BackgroundSettingsByMonitor.BackgroundType.SOLID)
                {
                    solidColor = backgroundSetting.SolidColor;
                    Dispatcher.Invoke(new Action(() =>
                    {
                        // backgroundImage.Source = null;
                        BrushConverter bc = new BrushConverter();

                        if (solidColor != null && solidColor != "")
                        {
                            overlayView.Background = (Brush)bc.ConvertFrom("#" + solidColor);
                        }
                        else
                        {
                            overlayView.Background = (Brush)bc.ConvertFrom("#000000");
                        }

                        youtubeView.Address = "";
                        youtubeView.Visibility = Visibility.Hidden;
                    }));
                }
                else
                {
                    youtubeVCode = backgroundSetting.YoutubeURL;
                    Dispatcher.Invoke(new Action(() =>
                    {
                        if (youtubeVCode == "" || youtubeVCode == null)
                        {
                            backgroundPath = DesktopWallpaperGenerator.GetBackgroud();
                            backgroundImage.Source = new BitmapImage(new Uri(backgroundPath));
                        }
                        else
                        {
                            youtubeView.Visibility = Visibility.Visible;
                            youtubeView.Address = string.Format("https://www.youtube.com/embed/{0}?controls=0&showinfo=0&rel=0&autoplay=1&loop=1&cc_load_policy=3&mute=1&playlist={0}", youtubeVCode);
                        }
                    }));
                }

                new Thread(CheckBackgroundChange).Start();
            }).Start();
        }
    }
}
