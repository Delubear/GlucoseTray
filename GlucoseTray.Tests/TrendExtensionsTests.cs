using GlucoseTray.Enums;

namespace GlucoseTray.Tests;

public class TrendExtensionsTests
{
    [TestCase("TripleUp", Trend.TripleUp)]
    [TestCase("DoubleUp", Trend.DoubleUp)]
    [TestCase("SingleUp", Trend.SingleUp)]
    [TestCase("FortyFiveUp", Trend.FortyFiveUp)]
    [TestCase("Flat", Trend.Flat)]
    [TestCase("FortyFiveDown", Trend.FortyFiveDown)]
    [TestCase("SingleDown", Trend.SingleDown)]
    [TestCase("DoubleDown", Trend.DoubleDown)]
    [TestCase("TripleDown", Trend.TripleDown)]
    public void ShouldMapDirectionStringToTrend(string direction, Trend expected) => Assert.That(direction.GetTrend(), Is.EqualTo(expected));

    [TestCase("")]
    [TestCase("NOT_A_REAL_DIRECTION")]
    [TestCase("flat")]
    public void ShouldMapUnknownDirectionToUnknownTrend(string direction) => Assert.That(direction.GetTrend(), Is.EqualTo(Trend.Unknown));

    [TestCase(Trend.SingleUp, "\u2191")]
    [TestCase(Trend.Flat, "\u2192")]
    [TestCase(Trend.SingleDown, "\u2193")]
    [TestCase(Trend.Unknown, "Unknown")]
    public void ShouldMapTrendToArrow(Trend trend, string expected) => Assert.That(trend.GetTrendArrow(), Is.EqualTo(expected));
}
