
namespace GlucoseTray;

public interface IGlucoseReader
{
    Task<GlucoseReading> GetLatestGlucoseAsync();
}

internal class GlucoseReader : IGlucoseReader
{
    public async Task<GlucoseReading> GetLatestGlucoseAsync()
    {
        return new GlucoseReading { MgValue = 100 };
    }
}
