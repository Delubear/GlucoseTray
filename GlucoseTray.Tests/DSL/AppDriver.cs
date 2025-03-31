
namespace GlucoseTray.Tests.DSL;

internal class AppDriver
{
    private readonly DslProvider _provider = new();
    private GlucoseResult _glucoseResult;

    public AppDriver GivenAGlucoseResult()
    {
        _glucoseResult = new GlucoseResult();
        return this;
    }

    public AppDriver WithMgValue(int value)
    {
        _glucoseResult.MgValue = value;
        return this;
    }

    public BehaviorDriver When => new(_provider, _glucoseResult);
}
