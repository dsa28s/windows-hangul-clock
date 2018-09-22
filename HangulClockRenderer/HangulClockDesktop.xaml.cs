using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Interop;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Win32;

using HangulClockUIKit.Weather;
using HangulClockLogKit;

namespace HangulClockRenderer
{
    /// <summary>
    /// HangulClockDesktop.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class HangulClockDesktop : Window
    {
        private SnowEngine snow = null;
        private string backgroundPath = "";

        public HangulClockDesktop()
        {
            InitializeComponent();

            backgroundPath = DesktopWallpaperGenerator.GetBackgroud();

            backgroundImage.Source = new BitmapImage(new Uri(backgroundPath));

            if (HangulClockRenderer.monitorIndeX == 0)
            {
                youtubeView.Address = "https://www.youtube.com/embed/xRbPAVnqtcs?controls=0&showinfo=0&rel=0&autoplay=1&loop=1&mute=1"; //.ShowYouTubeVideo("xRbPAVnqtcs");
            } else
            {
                youtubeView.Address = "https://www.youtube.com/embed/WNCl-69POro?controls=0&showinfo=0&rel=0&autoplay=1&loop=1&mute=1"; //.ShowYouTubeVideo("xRbPAVnqtcs");
            }

            setRightCommentText("상훈아, 나는 웃는 너가 참 좋은데,\n요즘따라 왜이리 웃는모습을 보기가 힘드니\n\n힘내자 오늘도, 내일도");

            new Thread(new ThreadStart(CheckBackgroundChange));
        }

        private void CheckBackgroundChange()
        {
            while(true)
            {
                string bPath = DesktopWallpaperGenerator.GetBackgroud();
                if (backgroundPath != bPath)
                {
                    backgroundPath = bPath;
                    backgroundImage.Source = new BitmapImage(new Uri(backgroundPath));
                }

                try
                {
                    Thread.Sleep(3000);
                }
                catch (ThreadInterruptedException e)
                {

                }
            }
            
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

            snow = new SnowEngine(weatherCanvas, "pack://application:,,,/Resources/snow.png");
            snow.SnowCoverage = 1;
            snow.VerticalSpeedRatio = 0.3;
            snow.HorizontalSpeedRatio = 0;
            //snow.Start();

            hangulClockTopComment.setTextSize(30);
            hangulClockBottomComment.setTextSize(30);
            hangulClockTopComment.setTextSize(30);
            hangulClockRightComment.setTextSize(30);
        }

        public void setTopCommentText(String text)
        {
            hangulClockTopComment.setContent(text);
            hangulClockLeftComment.setContent("");
            hangulClockRightComment.setContent(text);
            hangulClockBottomComment.setContent("");
        }

        public void setLeftCommentText(String text)
        {
            hangulClockTopComment.setContent("");
            hangulClockLeftComment.setContent(text);
            hangulClockRightComment.setContent("");
            hangulClockBottomComment.setContent("");
        }

        public void setRightCommentText(String text)
        {
            hangulClockTopComment.setContent("");
            hangulClockLeftComment.setContent("");
            hangulClockRightComment.setContent(text);
            hangulClockBottomComment.setContent("");
        }

        public void setBottomCommentText(String text)
        {
            hangulClockTopComment.setContent("");
            hangulClockLeftComment.setContent("");
            hangulClockRightComment.setContent("");
            hangulClockBottomComment.setContent(text);
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            
        }

    }
}
