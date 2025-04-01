using GlucoseTray.Read.Dexcom;
using GlucoseTray.Read.Nightscout;
using Microsoft.Extensions.Options;

namespace GlucoseTray.Read;

public interface IGlucoseReader
{
    Task<GlucoseReading> GetLatestGlucoseAsync();
}

internal class GlucoseReader(IOptionsMonitor<AppSettings> options, IExternalCommunicationAdapter communicator, IGlucoseReadingMapper mapper) : IGlucoseReader
{
    public async Task<GlucoseReading> GetLatestGlucoseAsync()
    {
        IReadStrategy strategy;

        if (options.CurrentValue.DataSource == GlucoseSource.Dexcom)
            strategy = new DexcomReadStrategy(options.CurrentValue, communicator, mapper);
        else
            strategy = new NightscoutReadStrategy(options.CurrentValue, communicator, mapper);

        try
        {
            var result = await strategy.GetLatestGlucoseAsync();
            return result;
        }
        catch
        {
            return new GlucoseReading();
        }
    }
}
