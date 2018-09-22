using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace HangulClockUIKit
{
    /// <summary>
    /// HangulClock.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class HangulClock : UserControl
    {
        public HangulClock()
        {
            InitializeComponent();

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(Timer_Tick);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateRealtimeFlag(true);
        }

        public static readonly DependencyProperty HangulClockFontSizeProperty = DependencyProperty.Register(
            "ClockFontSize", typeof(int), typeof(HangulClock), new FrameworkPropertyMetadata(new PropertyChangedCallback(AdjustControl)));

        public static readonly DependencyProperty HangulClockRealtimeClockProperty = DependencyProperty.Register(
            "ShowClockBold", typeof(bool), typeof(HangulClock), new FrameworkPropertyMetadata(new PropertyChangedCallback(IsRealtimeUpdateControl)));

        public int ClockFontSize
        {
            get { return (int)GetValue(HangulClockFontSizeProperty); }
            set { SetValue(HangulClockFontSizeProperty, value); }
        }

        public bool ShowClockBold
        {
            get { return (bool)GetValue(HangulClockRealtimeClockProperty); }
            set { SetValue(HangulClockRealtimeClockProperty, value); }
        }

        private static DispatcherTimer timer = new DispatcherTimer();

        private static void AdjustControl(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            (source as HangulClock).UpdateFontChangeControls((int)e.NewValue);
        }

        private static void IsRealtimeUpdateControl(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            (source as HangulClock).UpdateRealtimeFlag((bool)e.NewValue);
            if((bool)e.NewValue)
            {
                timer.Start();
            }
            else
            {
                timer.Stop();
            }
        }

        private void UpdateRealtimeFlag(bool isShow)
        {
            resetOpacity();

            if (isShow)
            {
                var currentTime = DateTime.Now;

                if (currentTime.Hour == 0 || currentTime.Hour == 12)
                {
                    if (currentTime.Hour == 12 && currentTime.Minute == 0)
                    {
                        hangulClock_element_51.Opacity = 1;
                        hangulClock_element_61.Opacity = 1;
                    }
                    else if (currentTime.Hour == 0 && currentTime.Minute == 0)
                    {
                        hangulClock_element_41.Opacity = 1;
                        hangulClock_element_51.Opacity = 1;
                    }
                    else
                    {
                        hangulClock_element_33.Opacity = 1;
                        hangulClock_element_35.Opacity = 1;
                        hangulClock_element_36.Opacity = 1;
                    }
                }
                else if (currentTime.Hour == 1 || currentTime.Hour == 13)
                {
                    hangulClock_element_11.Opacity = 1;
                    hangulClock_element_36.Opacity = 1;
                }
                else if (currentTime.Hour == 2 || currentTime.Hour == 14)
                {
                    hangulClock_element_12.Opacity = 1;
                    hangulClock_element_36.Opacity = 1;
                }
                else if (currentTime.Hour == 3 || currentTime.Hour == 15)
                {
                    hangulClock_element_13.Opacity = 1;
                    hangulClock_element_36.Opacity = 1;
                }
                else if (currentTime.Hour == 4 || currentTime.Hour == 16)
                {
                    hangulClock_element_14.Opacity = 1;
                    hangulClock_element_36.Opacity = 1;
                }
                else if (currentTime.Hour == 5 || currentTime.Hour == 17)
                {
                    hangulClock_element_15.Opacity = 1;
                    hangulClock_element_16.Opacity = 1;
                    hangulClock_element_36.Opacity = 1;
                }
                else if (currentTime.Hour == 6 || currentTime.Hour == 18)
                {
                    hangulClock_element_21.Opacity = 1;
                    hangulClock_element_22.Opacity = 1;
                    hangulClock_element_36.Opacity = 1;
                }
                else if (currentTime.Hour == 7 || currentTime.Hour == 19)
                {
                    hangulClock_element_23.Opacity = 1;
                    hangulClock_element_24.Opacity = 1;
                    hangulClock_element_36.Opacity = 1;
                }
                else if (currentTime.Hour == 8 || currentTime.Hour == 20)
                {
                    hangulClock_element_25.Opacity = 1;
                    hangulClock_element_26.Opacity = 1;
                    hangulClock_element_36.Opacity = 1;
                }
                else if (currentTime.Hour == 9 || currentTime.Hour == 21)
                {
                    hangulClock_element_31.Opacity = 1;
                    hangulClock_element_32.Opacity = 1;
                    hangulClock_element_36.Opacity = 1;
                }
                else if (currentTime.Hour == 10 || currentTime.Hour == 22)
                {
                    hangulClock_element_33.Opacity = 1;
                    hangulClock_element_36.Opacity = 1;
                }
                else if (currentTime.Hour == 11 || currentTime.Hour == 23)
                {
                    hangulClock_element_33.Opacity = 1;
                    hangulClock_element_34.Opacity = 1;
                    hangulClock_element_36.Opacity = 1;
                }

                if(currentTime.Minute == 1)
                {
                    hangulClock_element_52.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 2)
                {
                    hangulClock_element_53.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 3)
                {
                    hangulClock_element_54.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 4)
                {
                    hangulClock_element_54.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 5)
                {
                    hangulClock_element_62.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 6)
                {
                    hangulClock_element_56.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 7)
                {
                    hangulClock_element_63.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 8)
                {
                    hangulClock_element_64.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 9)
                {
                    hangulClock_element_65.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 10)
                {
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 11)
                {
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_52.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 12)
                {
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_53.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 13)
                {
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_54.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 14)
                {
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_55.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 15)
                {
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_62.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 16)
                {
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_56.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 17)
                {
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_63.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 18)
                {
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_64.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 19)
                {
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_65.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 20)
                {
                    hangulClock_element_42.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 21)
                {
                    hangulClock_element_42.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_52.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 22)
                {
                    hangulClock_element_42.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_53.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 23)
                {
                    hangulClock_element_42.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_54.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 24)
                {
                    hangulClock_element_42.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_55.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 25)
                {
                    hangulClock_element_42.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_61.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 26)
                {
                    hangulClock_element_42.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_56.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 27)
                {
                    hangulClock_element_42.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_63.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 28)
                {
                    hangulClock_element_42.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_64.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 29)
                {
                    hangulClock_element_42.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_65.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 30)
                {
                    hangulClock_element_43.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 31)
                {
                    hangulClock_element_43.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_52.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 32)
                {
                    hangulClock_element_43.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_53.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 33)
                {
                    hangulClock_element_43.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_54.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 34)
                {
                    hangulClock_element_43.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_55.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 35)
                {
                    hangulClock_element_43.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_62.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 36)
                {
                    hangulClock_element_43.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_56.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 37)
                {
                    hangulClock_element_43.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_63.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 38)
                {
                    hangulClock_element_43.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_64.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 39)
                {
                    hangulClock_element_43.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_65.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 40)
                {
                    hangulClock_element_44.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 41)
                {
                    hangulClock_element_44.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_52.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 42)
                {
                    hangulClock_element_44.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_53.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 43)
                {
                    hangulClock_element_44.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_54.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 44)
                {
                    hangulClock_element_44.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_55.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 45)
                {
                    hangulClock_element_44.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_62.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 46)
                {
                    hangulClock_element_44.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_56.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 47)
                {
                    hangulClock_element_44.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_63.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 48)
                {
                    hangulClock_element_44.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_64.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 49)
                {
                    hangulClock_element_44.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_65.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 50)
                {
                    hangulClock_element_45.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 51)
                {
                    hangulClock_element_45.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_52.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 52)
                {
                    hangulClock_element_45.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_53.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 53)
                {
                    hangulClock_element_45.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_54.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 54)
                {
                    hangulClock_element_45.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_55.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 55)
                {
                    hangulClock_element_45.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_62.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 56)
                {
                    hangulClock_element_45.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_56.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 57)
                {
                    hangulClock_element_45.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_63.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 58)
                {
                    hangulClock_element_45.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_64.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
                else if (currentTime.Minute == 59)
                {
                    hangulClock_element_45.Opacity = 1;
                    hangulClock_element_46.Opacity = 1;
                    hangulClock_element_65.Opacity = 1;
                    hangulClock_element_66.Opacity = 1;
                }
            }
        }

        private void UpdateFontChangeControls(int fontSize)
        {
            hangulClock_element_11.FontSize = fontSize;
            hangulClock_element_12.FontSize = fontSize;
            hangulClock_element_13.FontSize = fontSize;
            hangulClock_element_14.FontSize = fontSize;
            hangulClock_element_15.FontSize = fontSize;
            hangulClock_element_16.FontSize = fontSize;

            hangulClock_element_21.FontSize = fontSize;
            hangulClock_element_22.FontSize = fontSize;
            hangulClock_element_23.FontSize = fontSize;
            hangulClock_element_24.FontSize = fontSize;
            hangulClock_element_25.FontSize = fontSize;
            hangulClock_element_26.FontSize = fontSize;

            hangulClock_element_31.FontSize = fontSize;
            hangulClock_element_32.FontSize = fontSize;
            hangulClock_element_33.FontSize = fontSize;
            hangulClock_element_34.FontSize = fontSize;
            hangulClock_element_35.FontSize = fontSize;
            hangulClock_element_36.FontSize = fontSize;

            hangulClock_element_41.FontSize = fontSize;
            hangulClock_element_42.FontSize = fontSize;
            hangulClock_element_43.FontSize = fontSize;
            hangulClock_element_44.FontSize = fontSize;
            hangulClock_element_45.FontSize = fontSize;
            hangulClock_element_46.FontSize = fontSize;

            hangulClock_element_51.FontSize = fontSize;
            hangulClock_element_52.FontSize = fontSize;
            hangulClock_element_53.FontSize = fontSize;
            hangulClock_element_54.FontSize = fontSize;
            hangulClock_element_55.FontSize = fontSize;
            hangulClock_element_56.FontSize = fontSize;

            hangulClock_element_61.FontSize = fontSize;
            hangulClock_element_62.FontSize = fontSize;
            hangulClock_element_63.FontSize = fontSize;
            hangulClock_element_64.FontSize = fontSize;
            hangulClock_element_65.FontSize = fontSize;
            hangulClock_element_66.FontSize = fontSize;
        }

        private void resetOpacity()
        {
            hangulClock_element_11.Opacity = 0.2;
            hangulClock_element_12.Opacity = 0.2;
            hangulClock_element_13.Opacity = 0.2;
            hangulClock_element_14.Opacity = 0.2;
            hangulClock_element_15.Opacity = 0.2;
            hangulClock_element_16.Opacity = 0.2;

            hangulClock_element_21.Opacity = 0.2;
            hangulClock_element_22.Opacity = 0.2;
            hangulClock_element_23.Opacity = 0.2;
            hangulClock_element_24.Opacity = 0.2;
            hangulClock_element_25.Opacity = 0.2;
            hangulClock_element_26.Opacity = 0.2;

            hangulClock_element_31.Opacity = 0.2;
            hangulClock_element_32.Opacity = 0.2;
            hangulClock_element_33.Opacity = 0.2;
            hangulClock_element_34.Opacity = 0.2;
            hangulClock_element_35.Opacity = 0.2;
            hangulClock_element_36.Opacity = 0.2;

            hangulClock_element_41.Opacity = 0.2;
            hangulClock_element_42.Opacity = 0.2;
            hangulClock_element_43.Opacity = 0.2;
            hangulClock_element_44.Opacity = 0.2;
            hangulClock_element_45.Opacity = 0.2;
            hangulClock_element_46.Opacity = 0.2;

            hangulClock_element_51.Opacity = 0.2;
            hangulClock_element_52.Opacity = 0.2;
            hangulClock_element_53.Opacity = 0.2;
            hangulClock_element_54.Opacity = 0.2;
            hangulClock_element_55.Opacity = 0.2;
            hangulClock_element_56.Opacity = 0.2;

            hangulClock_element_61.Opacity = 0.2;
            hangulClock_element_62.Opacity = 0.2;
            hangulClock_element_63.Opacity = 0.2;
            hangulClock_element_64.Opacity = 0.2;
            hangulClock_element_65.Opacity = 0.2;
            hangulClock_element_66.Opacity = 0.2;
        }
    }
}
