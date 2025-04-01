using NSubstitute;

namespace GlucoseTray.Tests.DSL.Display;

internal class DisplayBehaviorDriver(DisplayProvider provider, GlucoseReading glucoseResult)
{
    public DisplayBehaviorDriver RefreshingIcon()
    {
        provider.Reader.GetLatestGlucoseAsync().Returns(glucoseResult);
        provider.Runner.Process().Wait();
        return this;
    }

    public DisplayAssertionDriver Then => new(provider);
}
