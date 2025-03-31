
using NSubstitute;

namespace GlucoseTray.Tests.DSL;

internal class BehaviorDriver
{
    private readonly DslProvider _provider;
    private readonly GlucoseResult _glucoseResult;

    public BehaviorDriver(DslProvider provider, GlucoseResult glucoseResult)
    {
        _provider = provider;
        _glucoseResult = glucoseResult;
    }

    public BehaviorDriver RefreshingIcon()
    {
        _provider.Reader.GetLatestGlucoseAsync().Returns(_glucoseResult);
        _provider.Runner.Process().Wait();
        return this;
    }

    public AssertionDriver Then => new(_provider);
}
