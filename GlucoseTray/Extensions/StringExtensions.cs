using System;
using System.Text.RegularExpressions;
using GlucoseTray.Enums;

namespace GlucoseTray.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveUnnecessaryCharacters(this string input)
        {
            return input.Replace("[", "").Replace("]", "").Replace("\\", "").Replace("/", "").Replace("\"", "").Replace("{", "").Replace("}", "").Replace("(", "").Replace(")", "");
        }

        public static string GetTrendArrowFromDexcom(this string input)
        {
            if (int.TryParse(input, out var trendInt))
            {
                switch ((TrendResult)trendInt)
                {
                    case TrendResult.DoubleUp: return "⮅";
                    case TrendResult.SingleUp: return "↑";
                    case TrendResult.FortyFiveUp: return "↗";
                    case TrendResult.Flat: return "→";
                    case TrendResult.FortFiveDown: return "↘";
                    case TrendResult.SingleDown: return "↓";
                    case TrendResult.DoubleDown: return "⮇";
                    default: return string.Empty;
                }
            }
            else
                return string.Empty;
        }

        public static string GetTrendArrowFromNightscout(this string input)
        {
            switch (input)
            {
                case nameof(TrendResult.DoubleUp): return "⮅";
                case nameof(TrendResult.SingleUp): return "↑";
                case nameof(TrendResult.FortyFiveUp): return "↗";
                case nameof(TrendResult.Flat): return "→";
                case nameof(TrendResult.FortFiveDown): return "↘";
                case nameof(TrendResult.SingleDown): return "↓";
                case nameof(TrendResult.DoubleDown): return "⮇";
                default: return string.Empty;
            }
        }

        public static DateTime ParseNightscoutDate(this string input)
        {
            Regex rgx = new Regex("[^0-9]");

            var ymd = input.Split('T')[0].Split('-');
            var hms = input.Split('T')[1].Split('.')[0].Split(':');

            var year = int.Parse(rgx.Replace(ymd[0], ""));
            var month = int.Parse(rgx.Replace(ymd[1], ""));
            var day = int.Parse(rgx.Replace(ymd[2], ""));

            var hour = int.Parse(rgx.Replace(hms[0], ""));
            var minute = int.Parse(rgx.Replace(hms[1], ""));
            var second = int.Parse(rgx.Replace(hms[2], ""));

            var dateTime = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);

            return dateTime.ToLocalTime();
        }
    }
}
