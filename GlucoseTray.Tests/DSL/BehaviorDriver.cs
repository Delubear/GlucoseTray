
using NSubstitute;

namespace GlucoseTray.Tests.DSL;

internal class BehaviorDriver
{
    private readonly DslProvider _provider;
    private readonly GlucoseReading _reading;

    public BehaviorDriver(DslProvider provider, GlucoseReading glucoseResult)
    {
        _provider = provider;
        _reading = glucoseResult;
    }

    public BehaviorDriver RefreshingIcon()
    {
        _provider.Reader.GetLatestGlucoseAsync().Returns(_reading);
        _provider.Runner.Process().Wait();
        return this;
    }

    public AssertionDriver Then => new(_provider);
}
