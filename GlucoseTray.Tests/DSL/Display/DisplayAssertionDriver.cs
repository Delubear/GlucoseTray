using GlucoseTray.Display;
using GlucoseTray.Enums;
using NSubstitute;

namespace GlucoseTray.Tests.DSL.Display;

internal class DisplayAssertionDriver(DisplayProvider provider, DisplayBehaviorDriver behaviorDriver)
{
    public DisplayBehaviorDriver When => behaviorDriver;

    public DisplayAssertionDriver ShouldBeRefreshedWithValue(string displayValue)
    {
        provider.Icon.Received().RefreshIcon(Arg.Is<GlucoseDisplay>(x => x.DisplayValue == displayValue));
        return this;
    }

    public DisplayAssertionDriver ShouldBeRefreshedWithTextColor(IconTextColor color)
    {
        provider.Icon.Received().RefreshIcon(Arg.Is<GlucoseDisplay>(x => x.Color == color));
        return this;
    }

    public DisplayAssertionDriver ShouldBeMarkedStale()
    {
        provider.Icon.Received().RefreshIcon(Arg.Is<GlucoseDisplay>(x => x.IsStale));
        return this;
    }

    public DisplayAssertionDriver ShouldBeRefreshedWithFontSize(int fontSize)
    {
        provider.Icon.Received().RefreshIcon(Arg.Is<GlucoseDisplay>(x => x.FontSize == fontSize));
        return this;
    }

    public DisplayAssertionDriver ShouldNotShowNotification()
    {
        provider.Icon.DidNotReceive().ShowNotification(Arg.Any<string>());
        return this;
    }

    public DisplayAssertionDriver ShouldShowNotification(string message)
    {
        provider.Icon.Received().ShowNotification(message);
        provider.Icon.ClearReceivedCalls();
        return this;
    }
}
