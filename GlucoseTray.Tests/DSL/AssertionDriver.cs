
using GlucoseTray.Display;
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

    public AssertionDriver ShouldBeRefreshedWithTextColor(IconTextColor color)
    {
        _provider.Icon.Received().RefreshIcon(Arg.Is<GlucoseDisplay>(x => x.Color == color));
        return this;
    }

    public AssertionDriver ShouldBeMarkedStale()
    {
        _provider.Icon.Received().RefreshIcon(Arg.Is<GlucoseDisplay>(x => x.IsStale));
        return this;
    }

    public AssertionDriver ShouldBeRefreshedWithFontSize(int fontSize)
    {
        _provider.Icon.Received().RefreshIcon(Arg.Is<GlucoseDisplay>(x => x.FontSize == fontSize));
        return this;
    }
}
