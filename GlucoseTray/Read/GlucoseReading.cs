using GlucoseTray.Enums;

namespace GlucoseTray.Read;

public class GlucoseReading
{
    public int MgValue { get; set; }
    public float MmolValue { get; set => field = MathF.Round(value, 1, MidpointRounding.AwayFromZero); }
    public DateTime TimestampUtc { get; set; }
    public Trend Trend { get; set; }
}
