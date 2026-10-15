using GlucoseTray.Enums;
using GlucoseTray.Read.Dexcom;
using GlucoseTray.Read.Nightscout;
using GlucoseTray.Read.LibreLinkUp;
using Microsoft.Extensions.Options;

namespace GlucoseTray.Read;

public interface IReadStrategyFactory
{
    IReadStrategy Create();
}

public class ReadStrategyFactory(IOptionsMonitor<AppSettings> options, IExternalCommunicationAdapter communicator, IGlucoseReadingMapper mapper, ICredentialProtector protector, ILibreLinkUpClient libreLinkUpClient) : IReadStrategyFactory
{
    public IReadStrategy Create()
    {
        var settings = options.CurrentValue;
        return settings.DataSource switch
        {
            GlucoseSource.Dexcom => new DexcomReadStrategy(settings, communicator, mapper, protector),
            GlucoseSource.Nightscout => new NightscoutReadStrategy(settings, communicator, mapper, protector),
            GlucoseSource.LibreLinkUp => new LibreLinkUpReadStrategy(settings, libreLinkUpClient),
            _ => throw new InvalidOperationException("Unsupported glucose data source."),
        };
    }
}
