using GlucoseTray.Domain;
using GlucoseTray.Domain.Enums;
using GlucoseTray.Domain.DisplayResults;
using NSubstitute;
using GlucoseTray.Domain.GlucoseSettings;

namespace GlucoseTray.Tests;

public class GlucoseFetchResultExtensionTests
{
    private GlucoseResult GetGlucoseResult(ISettingsProxy settingsProxy, double value, DateTime dateTimeUtc)
    {
        var glucoseResult = new GlucoseResult(settingsProxy);
        glucoseResult.SetGlucoseValues(value);
        glucoseResult.SetDateTimeUtc(dateTimeUtc);
        return glucoseResult;
    }

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
        var settings = Substitute.For<ISettingsProxy>();
        settings.IsServerDataUnitTypeMmol.Returns(unitType == GlucoseUnitType.MMOL);
        var fetchResult = GetGlucoseResult(settings, value, DateTime.UtcNow);

        // Act
        var result = fetchResult.GetFormattedStringValue(unitType);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void IsStale_Should_ReturnTrueWhenTooOld()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var fetchResult = GetGlucoseResult(Substitute.For<ISettingsProxy>(), 100, now.AddMinutes(-30));

        // Act
        var result = fetchResult.IsStale(15);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsStale_Should_ReturnFalseWhenNotTooOld()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var fetchResult = GetGlucoseResult(Substitute.For<ISettingsProxy>(), 100, now.AddMinutes(-10));

        // Act
        var result = fetchResult.IsStale(15);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void StaleMessage_Should_ReturnCorrectMessageWhenTooOld()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var fetchResult = GetGlucoseResult(Substitute.For<ISettingsProxy>(), 100, now.AddMinutes(-30));

        // Act
        var result = fetchResult.StaleMessage(15);

        // Assert
        Assert.That(result, Is.EqualTo($"\r\n30 minutes ago"));
    }

    [Test]
    public void StaleMessage_Should_ReturnEmptyStringWhenNotTooOld()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var fetchResult = GetGlucoseResult(Substitute.For<ISettingsProxy>(), 100, now.AddMinutes(-10));

        // Act
        var result = fetchResult.StaleMessage(15);

        // Assert
        Assert.That(result, Is.EqualTo(string.Empty));
    }
}