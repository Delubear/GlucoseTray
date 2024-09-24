using GlucoseTray.Enums;
using GlucoseTray.Extensions;

namespace GlucoseTray.Tests
{
    public class StringExtensionsTests
    {
        [Test]
        [TestCase("TripleUp", TrendResult.TripleUp)]
        [TestCase("DoubleUp", TrendResult.DoubleUp)]
        [TestCase("SingleUp", TrendResult.SingleUp)]
        [TestCase("FortyFiveUp", TrendResult.FortyFiveUp)]
        [TestCase("Flat", TrendResult.Flat)]
        [TestCase("FortyFiveDown", TrendResult.FortFiveDown)]
        [TestCase("SingleDown", TrendResult.SingleDown)]
        [TestCase("DoubleDown", TrendResult.DoubleDown)]
        [TestCase("TripleDown", TrendResult.TripleDown)]
        [TestCase("Unknown", TrendResult.Unknown)]
        public void GetTrend_Should_ReturnCorrectValue(string direction, TrendResult expected)
        {
            // Act
            var result = direction.GetTrend();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(TrendResult.TripleUp, "⤊")]
        [TestCase(TrendResult.DoubleUp, "⮅")]
        [TestCase(TrendResult.SingleUp, "↑")]
        [TestCase(TrendResult.FortyFiveUp, "↗")]
        [TestCase(TrendResult.Flat, "→")]
        [TestCase(TrendResult.FortFiveDown, "↘")]
        [TestCase(TrendResult.SingleDown, "↓")]
        [TestCase(TrendResult.DoubleDown, "⮇")]
        [TestCase(TrendResult.TripleDown, "⤋")]
        [TestCase(TrendResult.Unknown, "Unknown")]
        public void GetTrendArrow_Should_ReturnCorrectValue(TrendResult input, string expected)
        {
            // Act
            var result = input.GetTrendArrow();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
