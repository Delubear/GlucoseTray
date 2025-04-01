
namespace GlucoseTray.Read;

/// <summary>
/// Class that maps to the JSON received from DexCom queries.
/// </summary>
public class DexcomResult
{
    public string ST { get; set; } = string.Empty;
    public string DT { get; set; } = string.Empty;
    public string Trend { get; set; } = string.Empty;
    public double Value { get; set; }
    public string WT { get; set; } = string.Empty;
}
