namespace GlucoseTray.Models;

internal class GlucoseResult
{
    internal int Id { get; set; }
    internal int MgValue { get; set; }
    internal double MmolValue { get; set; }
    internal DateTime DateTimeUTC { get; set; }
    internal TrendResult Trend { get; set; }
    internal bool WasError { get; set; }
    internal FetchMethod Source { get; set; }
    internal bool IsCriticalLow { get; set; }
}
