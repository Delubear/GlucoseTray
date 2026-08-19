using GlucoseTray.Read;
using GlucoseTray.Read.Nightscout;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace GlucoseTray.Tests;

public class GlucoseReadingMapperTests
{
    private static GlucoseReadingMapper CreateMapper()
    {
        var options = Substitute.For<IOptionsMonitor<AppSettings>>();
        options.CurrentValue.Returns(new AppSettings { ServerUnitType = Enums.GlucoseUnitType.Mg });
        return new GlucoseReadingMapper(options);
    }

    [Test]
    public void ShouldUseDateStringWhenPresentForNightscout()
    {
        var mapper = CreateMapper();
        var input = new NightScoutResult { GlucoseValue = 100, DateString = "2024-01-15T10:30:00.000Z" };

        var result = mapper.Map(input);

        Assert.That(result.TimestampUtc, Is.EqualTo(new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc)));
    }

    [Test]
    public void ShouldFallBackToUnixTicksWhenNightscoutDateStringIsMissing()
    {
        var mapper = CreateMapper();
        var expected = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        var input = new NightScoutResult { GlucoseValue = 100, DateString = string.Empty, UnixTicks = new DateTimeOffset(expected).ToUnixTimeMilliseconds() };

        var result = mapper.Map(input);

        Assert.That(result.TimestampUtc, Is.EqualTo(expected));
    }

    [Test]
    public void ShouldFallBackToMinValueWhenNightscoutHasNoTimestampData()
    {
        var mapper = CreateMapper();
        var input = new NightScoutResult { GlucoseValue = 100, DateString = string.Empty, UnixTicks = 0 };

        var result = mapper.Map(input);

        Assert.That(result.TimestampUtc, Is.EqualTo(DateTime.MinValue));
    }
}
