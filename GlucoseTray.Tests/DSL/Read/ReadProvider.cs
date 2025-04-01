using GlucoseTray.Display;
using GlucoseTray.Read;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace GlucoseTray.Tests.DSL.Read;

internal class ReadProvider
{
    public readonly AppRunner Runner;
    public readonly ITray Tray = Substitute.For<ITray>();
    public readonly IGlucoseReadingMapper GlucoseReadingMapper;
    public readonly ITrayIcon Icon = Substitute.For<ITrayIcon>();
    public readonly IGlucoseReader Reader;
    public readonly IOptionsMonitor<AppSettings> Options = Substitute.For<IOptionsMonitor<AppSettings>>();
    public readonly IExternalCommunicationAdapter ExternalCommunicationAdapter = Substitute.For<IExternalCommunicationAdapter>();

    public ReadProvider()
    {
        GlucoseReadingMapper = new GlucoseReadingMapper(Options);
        Reader = new GlucoseReader(Options, ExternalCommunicationAdapter, GlucoseReadingMapper);
        Runner = new AppRunner(Tray, Reader);
    }
}
