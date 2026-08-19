using GlucoseTray.Enums;
using GlucoseTray.Read.Dexcom;
using GlucoseTray.Read.Nightscout;
using Microsoft.Extensions.Options;

namespace GlucoseTray.Read;

public interface IReadStrategyFactory
{
    IReadStrategy Create();
}

public class ReadStrategyFactory(IOptionsMonitor<AppSettings> options, IExternalCommunicationAdapter communicator, IGlucoseReadingMapper mapper, ICredentialProtector protector) : IReadStrategyFactory
{
    public IReadStrategy Create()
    {
        var settings = options.CurrentValue;
        return settings.DataSource == GlucoseSource.Dexcom
            ? new DexcomReadStrategy(settings, communicator, mapper, protector)
            : new NightscoutReadStrategy(settings, communicator, mapper, protector);
    }
}
