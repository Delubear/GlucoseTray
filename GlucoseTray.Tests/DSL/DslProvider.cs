
using GlucoseTray.Display;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace GlucoseTray.Tests.DSL;

internal class DslProvider
{
    public readonly AppRunner Runner;
    public readonly ITray Tray;
    public readonly IGlucoseDisplayMapper GlucoseDisplayMapper;
    public readonly ITrayIcon Icon = Substitute.For<ITrayIcon>();
    public readonly IGlucoseReader Reader = Substitute.For<IGlucoseReader>();
    public readonly IOptionsMonitor<AppSettings> Options = Substitute.For<IOptionsMonitor<AppSettings>>();

    public DslProvider()
    {
        GlucoseDisplayMapper = new GlucoseDisplayMapper(Options);
        Tray = new Tray(Icon, GlucoseDisplayMapper);
        Runner = new AppRunner(Options, Tray, Reader);
    }
}
