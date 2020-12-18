using GlucoseTrayCore.Enums;

namespace GlucoseTrayCore.Extensions
{
    public static class StringExtensions
    {
        public static string GetTrendArrow(this TrendResult input)
        {
            switch (input)
            {
                case TrendResult.TripleUp:
                    return "⤊";
                case TrendResult.DoubleUp:
                    return "⮅";
                case TrendResult.SingleUp:
                    return "↑";
                case TrendResult.FortyFiveUp:
                    return "↗";
                case TrendResult.Flat:
                    return "→";
                case TrendResult.FortFiveDown:
                    return "↘";
                case TrendResult.SingleDown:
                    return "↓";
                case TrendResult.DoubleDown:
                    return "⮇";
                case TrendResult.TripleDown:
                    return "⤋";
                case TrendResult.Unknown:
                    return "Unknown";
                default:
                    return string.Empty;
            }
        }

        internal static TrendResult GetTrend(this string direction)
        {
            // Values for Direction copied from https://github.com/nightscout/cgm-remote-monitor/blob/41ac93f7217b1b7023ec6ad6fc35d29dcf2e4f88/lib/plugins/direction.js
            switch (direction)
            {
                case "TripleUp":
                    return TrendResult.TripleUp;
                case "DoubleUp":
                    return TrendResult.DoubleUp;
                case "SingleUp":
                    return TrendResult.SingleUp;
                case "FortyFiveUp":
                    return TrendResult.FortyFiveUp;
                case "Flat":
                    return TrendResult.Flat;
                case "FortyFiveDown":
                    return TrendResult.FortFiveDown;
                case "SingleDown":
                    return TrendResult.SingleDown;
                case "DoubleDown":
                    return TrendResult.DoubleDown;
                case "TripleDown":
                    return TrendResult.TripleDown;
                default:
                    return TrendResult.Unknown;
            }
        }
    }
}
