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
                case TrendResult.DoubleUp       : return "⮅";
                case TrendResult.SingleUp       : return "↑";
                case TrendResult.FortyFiveUp    : return "↗";
                case TrendResult.Flat           : return "→";
                case TrendResult.FortFiveDown   : return "↘";
                case TrendResult.SingleDown     : return "↓";
                case TrendResult.DoubleDown     : return "⮇";
                default: return string.Empty;
            }
        }

        public static string GetTrendArrow(this string direction)
        {

            // Values for Direction copied from https://github.com/nightscout/cgm-remote-monitor/blob/41ac93f7217b1b7023ec6ad6fc35d29dcf2e4f88/lib/plugins/direction.js
            switch (direction)
            {
                case "TripleUp"     : return "⤊";
                case "DoubleUp"     : return "⮅";
                case "SingleUp"     : return "↑";
                case "FortyFiveUp"  : return "↗";
                case "Flat"         : return "→";
                case "FortyFiveDown": return "↘";
                case "SingleDown"   : return "↓";
                case "DoubleDown"   : return "⮇";
                case "TripleDown"   : return "⤋";
                default             : return direction;

            }
        }
    }
}
