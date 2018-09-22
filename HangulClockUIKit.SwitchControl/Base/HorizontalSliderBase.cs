using System.Windows;
using System.Windows.Input;

namespace HangulClockUIKit.Controls.Base
{
    public class HorizontalSliderBase: SliderBase
    {
        static HorizontalSliderBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HorizontalSliderBase), new FrameworkPropertyMetadata(typeof(HorizontalSliderBase)));
        }

        protected override void UpdatePosition()
        {
            if (this.sliderCanvas == null) return;

            this.Position = Mouse.GetPosition(this.sliderCanvas).X;

            if (this.Position > this.sliderCanvas.ActualWidth)
            {
                this.Position = this.sliderCanvas.ActualWidth;
            }

            if (this.Position < 0.0)
            {
                this.Position = 0.0;
            }
        }

        protected override void CalculatePosition()
        {
            if (this.sliderCanvas == null) return;

            if (!this.isCalculating)
            {
                this.isCalculating = true;

                if ((this.Maximum - this.Minimum) > 0 && this.sliderCanvas.ActualWidth > 0)
                {
                    this.Position = ((this.Value - this.Minimum) / (this.Maximum - this.Minimum)) * this.sliderCanvas.ActualWidth;
                }
                else
                {
                    this.Position = 0;
                }

                this.isCalculating = false;
            }
        }

        protected override void CalculateValue()
        {
            if (this.sliderCanvas == null) return;

            if (!this.isCalculating)
            {
                this.isCalculating = true;

                if (this.sliderCanvas.ActualWidth > 0)
                {
                    this.Value = ((this.Position * (this.Maximum - this.Minimum)) / this.sliderCanvas.ActualWidth) + this.Minimum;
                }
                else
                {
                    this.Value = 0;
                }

                this.isCalculating = false;
            }

        }
    }
}
