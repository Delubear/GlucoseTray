namespace GlucoseTray.Read;

public class GlucoseReading
{
    public int MgValue { get; set; }
    public float MmolValue { get; set; }
    public DateTime TimestampUtc { get; set; }
    public Trend Trend { get; set; }
}
