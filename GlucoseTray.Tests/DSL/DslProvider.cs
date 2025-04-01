
using GlucoseTray.Display;
using GlucoseTray.Read;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace GlucoseTray.Tests.DSL;

internal class DslProvider
{
    public readonly AppRunner Runner;
    public readonly ITray Tray;
    public readonly IGlucoseDisplayMapper GlucoseDisplayMapper;
    public readonly IGlucoseReadingMapper GlucoseReadingMapper;
    public readonly ITrayIcon Icon = Substitute.For<ITrayIcon>();
    public readonly IGlucoseReader Reader;
    public readonly IOptionsMonitor<AppSettings> Options = Substitute.For<IOptionsMonitor<AppSettings>>();
    public readonly IExternalCommunicationAdapter ExternalCommunicationAdapter = Substitute.For<IExternalCommunicationAdapter>();

    public DslProvider()
    {
        GlucoseDisplayMapper = new GlucoseDisplayMapper(Options);
        GlucoseReadingMapper = new GlucoseReadingMapper(Options);
        Reader = new GlucoseReader(Options, ExternalCommunicationAdapter, GlucoseReadingMapper);
        Tray = new Tray(Icon, GlucoseDisplayMapper);
        Runner = new AppRunner(Tray, Reader);
    }
}
