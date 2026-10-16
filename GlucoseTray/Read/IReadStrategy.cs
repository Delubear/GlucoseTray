namespace GlucoseTray.Read;

public interface IReadStrategy
{
    object? CacheIdentity => null;
    Task<GlucoseReading> GetLatestGlucoseAsync();
}
