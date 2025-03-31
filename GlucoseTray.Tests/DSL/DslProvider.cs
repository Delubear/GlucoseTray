
using NSubstitute;

namespace GlucoseTray.Tests.DSL;

internal class DslProvider
{
    public readonly AppRunner Runner;
    public readonly ITray Tray;
    public readonly ITrayIcon Icon = Substitute.For<ITrayIcon>();
    public readonly IGlucoseReader Reader = Substitute.For<IGlucoseReader>();

    public DslProvider()
    {
        Tray = new Tray(Icon);
        Runner = new AppRunner(Tray, Reader);
    }
}
