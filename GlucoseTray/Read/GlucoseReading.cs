using GlucoseTray.Enums;

namespace GlucoseTray.Read;

public class GlucoseReading
{
    public string? SubjectId { get; set; }
    public bool SubjectChanged { get; set; }
    public string? ConnectionError { get; set; }
    public int MgValue { get; set; }
    public float MmolValue { get; set; }
    public DateTime TimestampUtc { get; set; }
    public Trend Trend { get; set; }
}
