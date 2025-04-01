namespace GlucoseTray.Enums;

public enum Trend
{
    TripleUp = 0,
    DoubleUp = 1,
    SingleUp = 2,
    FortyFiveUp = 3,
    Flat = 4,
    FortFiveDown = 5,
    SingleDown = 6,
    DoubleDown = 7,
    TripleDown = 8,
    Unknown = 9
}

public static class TrendExtensions
{
    public static string GetTrendArrow(this Trend input)
    {
        return input switch
        {
            Trend.TripleUp => "⤊",
            Trend.DoubleUp => "⮅",
            Trend.SingleUp => "↑",
            Trend.FortyFiveUp => "↗",
            Trend.Flat => "→",
            Trend.FortFiveDown => "↘",
            Trend.SingleDown => "↓",
            Trend.DoubleDown => "⮇",
            Trend.TripleDown => "⤋",
            Trend.Unknown => "Unknown",
            _ => string.Empty,
        };
    }

    public static Trend GetTrend(this string direction)
    {
        // Values for Direction copied from https://github.com/nightscout/cgm-remote-monitor/blob/41ac93f7217b1b7023ec6ad6fc35d29dcf2e4f88/lib/plugins/direction.js
        return direction switch
        {
            "TripleUp" => Trend.TripleUp,
            "DoubleUp" => Trend.DoubleUp,
            "SingleUp" => Trend.SingleUp,
            "FortyFiveUp" => Trend.FortyFiveUp,
            "Flat" => Trend.Flat,
            "FortyFiveDown" => Trend.FortFiveDown,
            "SingleDown" => Trend.SingleDown,
            "DoubleDown" => Trend.DoubleDown,
            "TripleDown" => Trend.TripleDown,
            _ => Trend.Unknown,
        };
    }
}