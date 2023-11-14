namespace GlucoseTray.Models;

/// <summary>
/// Class that maps to the JSON received from DexCom queries.
/// </summary>
internal class DexcomResult
{
    internal string ST { get; set; } = string.Empty;
    internal string DT { get; set; } = string.Empty;
    internal string Trend { get; set; } = string.Empty;
    internal double Value { get; set; }
    internal string WT { get; set; } = string.Empty;
}
