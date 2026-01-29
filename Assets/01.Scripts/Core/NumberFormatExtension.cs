namespace JunkyardClicker.Core
{
    public static class NumberFormatExtension
    {
        private static readonly string[] s_suffixes =
        {
            "", "K", "M", "B", "T",
            "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj",
            "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at",
            "au", "av", "aw", "ax", "ay", "az",
            "ba", "bb", "bc", "bd", "be", "bf", "bg", "bh", "bi", "bj",
            "bk", "bl", "bm", "bn", "bo", "bp", "bq", "br", "bs", "bt",
            "bu", "bv", "bw", "bx", "by", "bz"
        };

        public static string ToFormattedString(this double number)
        {
            if (number < 1000)
            {
                return number.ToString("N0");
            }

            int suffixIndex = 0;
            double value = number;

            while (value >= 1000 && suffixIndex < s_suffixes.Length - 1)
            {
                value /= 1000;
                suffixIndex++;
            }

            if (value >= 100)
            {
                return $"{value:F0}{s_suffixes[suffixIndex]}";
            }

            if (value >= 10)
            {
                return $"{value:F1}{s_suffixes[suffixIndex]}";
            }

            return $"{value:F2}{s_suffixes[suffixIndex]}";
        }

        public static string ToFormattedString(this int number)
        {
            return ((double)number).ToFormattedString();
        }
    }
}
