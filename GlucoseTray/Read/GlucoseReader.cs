using GlucoseTray.Enums;
using GlucoseTray.Read.Dexcom;
using GlucoseTray.Read.Nightscout;
using Microsoft.Extensions.Options;

namespace GlucoseTray.Read;

public interface IGlucoseReader
{
    Task<GlucoseReading> GetLatestGlucoseAsync();
}

public class GlucoseReader(IOptionsMonitor<AppSettings> options, IExternalCommunicationAdapter communicator, IGlucoseReadingMapper mapper) : IGlucoseReader
{
    private GlucoseReading? _latestReading;

    public async Task<GlucoseReading> GetLatestGlucoseAsync()
    {
        IReadStrategy strategy = GetReadStrategy();

        try
        {
            _latestReading = await strategy.GetLatestGlucoseAsync();
            return _latestReading;
        }
        catch
        {
            return _latestReading ?? new GlucoseReading() { TimestampUtc = DateTime.UtcNow, Trend = Trend.Unknown };
        }
    }

    private IReadStrategy GetReadStrategy()
    {
        if (options.CurrentValue.DataSource == GlucoseSource.Dexcom)
            return new DexcomReadStrategy(options.CurrentValue, communicator, mapper);
        else
            return new NightscoutReadStrategy(options.CurrentValue, communicator, mapper);
    }
}
