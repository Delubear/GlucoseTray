using GlucoseTray.Enums;
using System.Globalization;

namespace GlucoseTray.Display;

public class GlucoseDisplay
{
    public string? ConnectionError { get; set; }
    public string DisplayValue { get; set; } = string.Empty;
    public bool IsStale { get; set; }
    public DateTime TimestampUtc { get; set; }
    public Trend Trend { get; set; }
    public int FontSize { get; set; }
    public IconTextColor Color { get; set; } = IconTextColor.White;

    public string GetDisplayMessage(DateTime utcNow)
    {
        if (TimestampUtc == DateTime.MinValue)
            return ConnectionError ?? "No glucose reading available yet.";
        var staleMessage = IsStale ? $"\r\n{Math.Abs((utcNow - TimestampUtc).TotalMinutes).ToString("#", CultureInfo.InvariantCulture)} minutes ago" : string.Empty;
        var message = $"{DisplayValue} {TimestampUtc.ToLocalTime().ToLongTimeString()} {Trend.GetTrendArrow()} {staleMessage}".Trim();
        return ConnectionError is null ? message : $"{ConnectionError}\r\nLast reading: {message}";
    }
}
