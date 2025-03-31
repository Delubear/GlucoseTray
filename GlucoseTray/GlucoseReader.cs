
namespace GlucoseTray;

public interface IGlucoseReader
{
    Task<GlucoseResult> GetLatestGlucoseAsync();
}

internal class GlucoseReader : IGlucoseReader
{
    public async Task<GlucoseResult> GetLatestGlucoseAsync()
    {
        return new GlucoseResult { MgValue = 100 };
    }
}
