namespace GlucoseTray.Models;

public class GlucoseResult
{
    public int MgValue { get; set; }
    public double MmolValue { get; set; }
    public DateTime DateTimeUTC { get; set; }
    public TrendResult Trend { get; set; }
    public bool WasError { get; set; }
    public FetchMethod Source { get; set; }
    public bool IsCriticalLow { get; set; }

    public static GlucoseResult Default => new()
    {
        MmolValue = 0,
        MgValue = 0,
        DateTimeUTC = DateTime.Now.ToUniversalTime(),
        Trend = TrendResult.Unknown,
        WasError = true
    };
}
