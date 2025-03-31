
namespace GlucoseTray;

public class GlucoseDisplay
{
    public string DisplayValue { get; set; } = string.Empty;
    public bool IsStale { get; set; }
    public int FontSize { get; set; }
    public GlucoseColor Color { get; set; } = GlucoseColor.White;
}

public enum GlucoseColor
{
    White,
    Black,
    Yellow,
    Gold,
    Red,
}
