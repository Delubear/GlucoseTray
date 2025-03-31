
using NSubstitute;

namespace GlucoseTray.Tests.DSL;

internal class AssertionDriver
{
    private readonly DslProvider _provider;

    public AssertionDriver(DslProvider provider)
    {
        _provider = provider;
    }

    public AssertionDriver ShouldBeRefreshedWithValue(string displayValue)
    {
        _provider.Icon.Received().RefreshIcon(Arg.Is<GlucoseDisplay>(x => x.DisplayValue == displayValue));
        return this;
    }
}
