namespace GlucoseTray.Read;

public interface IReadStrategy
{
    Task<GlucoseReading> GetLatestGlucoseAsync();
}
