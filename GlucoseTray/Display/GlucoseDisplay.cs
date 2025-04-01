namespace GlucoseTray.Display;

public class GlucoseDisplay
{
    public string DisplayValue { get; set; } = string.Empty;
    public bool IsStale { get; set; }
    public DateTime TimestampUtc { get; set; }
    public Trend Trend { get; set; }
    public int FontSize { get; set; }
    public IconTextColor Color { get; set; } = IconTextColor.White;

    public string GetDisplayMessage(DateTime utcNow)
    {
        var staleMessage = IsStale ? $"\r\n{Math.Abs((utcNow - TimestampUtc).TotalMinutes):#} minutes ago" : string.Empty;
        return $"{DisplayValue} {TimestampUtc.ToLocalTime().ToLongTimeString()} {Trend.GetTrendArrow()} {staleMessage}".Trim();
    }
}

public enum IconTextColor
{
    White,
    Black,
    Yellow,
    Gold,
    Red,
}
