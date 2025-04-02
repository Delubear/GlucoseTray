using GlucoseTray.Read;
using NSubstitute;

namespace GlucoseTray.Tests.DSL.Read;

internal class ReadAssertionDriver(ReadProvider provider, ReadBehaviorDriver behaviorDriver)
{
    public ReadBehaviorDriver When => behaviorDriver;

    public ReadAssertionDriver ShouldHaveMgValueOf(int value)
    {
        provider.Tray.Received().Refresh(Arg.Is<GlucoseReading>(x => x.MgValue == value));
        return this;
    }

    public ReadAssertionDriver ShouldHaveMmolValueOf(float value)
    {
        provider.Tray.Received().Refresh(Arg.Is<GlucoseReading>(x => x.MmolValue == value));
        return this;
    }
}
