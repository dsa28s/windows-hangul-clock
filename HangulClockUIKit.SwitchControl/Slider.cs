using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HangulClockUIKit.Controls.Base;
using System.Windows;
using System.Windows.Media;

namespace HangulClockUIKit.Controls
{
    public class Slider:HorizontalSliderBase
    {
        public const int UWPSliderBaseUnit = 8;
        public const int UWPSliderCanvasLengthOffset = -2 * UWPSliderBaseUnit;

        public double BarFillPosition
        {
            get { return Convert.ToDouble(BarFillPositionProperty); }
            set { SetValue(BarFillPositionProperty, value); }
        }

        public static readonly DependencyProperty BarFillPositionProperty =
            DependencyProperty.Register(nameof(BarFillPosition), typeof(double), typeof(Slider), new PropertyMetadata(0.0));

        public Brush ButtonInnerBackground
        {
            get { return (Brush)GetValue(ButtonInnerBackgroundProperty); }
            set { SetValue(ButtonInnerBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ButtonInnerBackgroundProperty =
          DependencyProperty.Register(nameof(ButtonInnerBackground), typeof(Brush), typeof(Slider), new PropertyMetadata(null));

        static Slider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Slider), new FrameworkPropertyMetadata(typeof(Slider)));
        }

        protected override void UpdatePosition()
        {
            base.UpdatePosition();
            this.CalculateVisibleLengths();
        }

        protected override void CalculatePosition()
        {
            base.CalculatePosition();
            this.CalculateVisibleLengths();
        }

        protected virtual void CalculateVisibleLengths()
        {
            if (this.sliderCanvas != null && this.sliderCanvas.ActualWidth != 0 && this.sliderButton != null)
            {
                this.BarFillPosition = this.Position * (this.sliderCanvas.ActualWidth - UWPSliderCanvasLengthOffset) /
                                       this.sliderCanvas.ActualWidth;
            }
        }
    }
}
