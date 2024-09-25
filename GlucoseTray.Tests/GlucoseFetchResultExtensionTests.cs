using GlucoseTray.Domain;
using GlucoseTray.Domain.Enums;
using GlucoseTray.Domain.FetchResults;
using GlucoseTray.Domain.DisplayResults;

namespace GlucoseTray.Tests;

public class GlucoseFetchResultExtensionTests
{
    [Test]
    [TestCase(GlucoseUnitType.MG, 1, "1")]
    [TestCase(GlucoseUnitType.MG, 100, "100")]
    [TestCase(GlucoseUnitType.MMOL, 1, "1.0")]
    [TestCase(GlucoseUnitType.MMOL, 10, "10.0")]
    [TestCase(GlucoseUnitType.MMOL, 5.5, "5.5")]
    [TestCase(GlucoseUnitType.MMOL, 15.5, "15.5")]
    public void GetFormattedStringValue_Should_ReturnCorrectValue(GlucoseUnitType unitType, double value, string expected)
    {
        // Arrange
        var fetchResult = new GlucoseResult
        {
            MgValue = (int)value,
            MmolValue = value,
        };

        // Act
        var result = fetchResult.GetFormattedStringValue(unitType);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void IsStale_Should_ReturnTrueWhenTooOld()
    {
        var now = DateTime.UtcNow;

        // Arrange
        var fetchResult = new GlucoseResult
        {
            DateTimeUTC = now.AddMinutes(-30)
        };

        // Act
        var result = fetchResult.IsStale(15);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsStale_Should_ReturnFalseWhenNotTooOld()
    {
        var now = DateTime.UtcNow;

        // Arrange
        var fetchResult = new GlucoseResult
        {
            DateTimeUTC = now.AddMinutes(-10)
        };

        // Act
        var result = fetchResult.IsStale(15);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void StaleMessage_Should_ReturnCorrectMessageWhenTooOld()
    {
        var now = DateTime.UtcNow;

        // Arrange
        var fetchResult = new GlucoseResult
        {
            DateTimeUTC = now.AddMinutes(-30)
        };

        // Act
        var result = fetchResult.StaleMessage(15);

        // Assert
        Assert.That(result, Is.EqualTo($"\r\n30 minutes ago"));
    }

    [Test]
    public void StaleMessage_Should_ReturnEmptyStringWhenNotTooOld()
    {
        var now = DateTime.UtcNow;

        // Arrange
        var fetchResult = new GlucoseResult
        {
            DateTimeUTC = now.AddMinutes(-10)
        };

        // Act
        var result = fetchResult.StaleMessage(15);

        // Assert
        Assert.That(result, Is.EqualTo(string.Empty));
    }
}