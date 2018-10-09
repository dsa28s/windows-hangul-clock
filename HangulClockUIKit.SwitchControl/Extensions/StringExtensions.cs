namespace HangulClockUIKit.Controls.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNumeric(this string str)
        {
            float output;
            return float.TryParse(str, out output);
        }
    }
}
