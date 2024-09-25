using GlucoseTray.Domain.Enums;

namespace GlucoseTray.Domain.DisplayResults;

public static class StringExtensions
{
    public static string GetTrendArrow(this TrendResult input)
    {
        return input switch
        {
            TrendResult.TripleUp => "⤊",
            TrendResult.DoubleUp => "⮅",
            TrendResult.SingleUp => "↑",
            TrendResult.FortyFiveUp => "↗",
            TrendResult.Flat => "→",
            TrendResult.FortFiveDown => "↘",
            TrendResult.SingleDown => "↓",
            TrendResult.DoubleDown => "⮇",
            TrendResult.TripleDown => "⤋",
            TrendResult.Unknown => "Unknown",
            _ => string.Empty,
        };
    }

    public static TrendResult GetTrend(this string direction)
    {
        // Values for Direction copied from https://github.com/nightscout/cgm-remote-monitor/blob/41ac93f7217b1b7023ec6ad6fc35d29dcf2e4f88/lib/plugins/direction.js
        return direction switch
        {
            "TripleUp" => TrendResult.TripleUp,
            "DoubleUp" => TrendResult.DoubleUp,
            "SingleUp" => TrendResult.SingleUp,
            "FortyFiveUp" => TrendResult.FortyFiveUp,
            "Flat" => TrendResult.Flat,
            "FortyFiveDown" => TrendResult.FortFiveDown,
            "SingleDown" => TrendResult.SingleDown,
            "DoubleDown" => TrendResult.DoubleDown,
            "TripleDown" => TrendResult.TripleDown,
            _ => TrendResult.Unknown,
        };
    }
}
