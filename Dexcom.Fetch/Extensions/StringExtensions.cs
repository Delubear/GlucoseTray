using System;
using System.Text.RegularExpressions;
using Dexcom.Fetch.Enums;

namespace Dexcom.Fetch.Extensions
{
    internal static class StringExtensions
    {
        public static string GetTrendArrow(this int input)
        {
            switch ((TrendResult)input)
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
    }
}
