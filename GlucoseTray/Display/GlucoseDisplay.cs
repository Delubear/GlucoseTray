namespace GlucoseTray.Display;

public class GlucoseDisplay
{
    public string DisplayValue { get; set; } = string.Empty;
    public bool IsStale { get; set; }
    public int FontSize { get; set; }
    public IconTextColor Color { get; set; } = IconTextColor.White;
}

public enum IconTextColor
{
    White,
    Black,
    Yellow,
    Gold,
    Red,
}
