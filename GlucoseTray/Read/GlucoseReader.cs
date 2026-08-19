using GlucoseTray.Enums;
using Microsoft.Extensions.Logging;

namespace GlucoseTray.Read;

public interface IGlucoseReader
{
    Task<GlucoseReading> GetLatestGlucoseAsync();
}

public class GlucoseReader(IReadStrategyFactory strategyFactory, ILogger<GlucoseReader> logger) : IGlucoseReader
{
    private GlucoseReading? _latestReading;

    public async Task<GlucoseReading> GetLatestGlucoseAsync()
    {
        IReadStrategy strategy = strategyFactory.Create();

        try
        {
            _latestReading = await strategy.GetLatestGlucoseAsync();
            return _latestReading;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to read latest glucose value. Falling back to the last known reading.");
            return _latestReading ?? new GlucoseReading() { TimestampUtc = DateTime.UtcNow, Trend = Trend.Unknown };
        }
    }
}
