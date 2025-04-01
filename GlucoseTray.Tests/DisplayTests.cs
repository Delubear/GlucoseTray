using GlucoseTray.Display;
using GlucoseTray.Enums;
using GlucoseTray.Tests.DSL.Display;

namespace GlucoseTray.Tests;

public class DisplayTests
{
    [Test]
    public void ShouldMapMgToDisplay()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithMgValue(100)
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithValue("100");
    }

    [Test]
    public void ShouldMapMmolToDisplay()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithMmolDisplay()
              .WithMmolValue(5.5f)
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithValue("5'5");
    }

    [Test]
    public void ShouldShowWhiteTextForStandardMmolReadingInDarkMode()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithMmolDisplay()
              .WithMmolValue(5.5f)
              .WithDarkMode()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.White);
    }

    [Test]
    public void ShouldShowWhiteTextForStandardMgReadingInDarkMode()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithMgValue(100)
              .WithDarkMode()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.White);
    }

    [Test]
    public void ShouldRecognizeWhenDataIsStale()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithMgValue(100)
              .WithStaleData()
              .When.RefreshingIcon()
              .Then.ShouldBeMarkedStale();
    }

    [Test]
    public void ShouldShowYellowTextForMgReadingsBelowLow()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithLowValue()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Yellow);
    }

    [Test]
    public void ShouldShowYellowTextForMmolReadingsBelowLow()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithMmolDisplay()
              .WithLowValue()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Yellow);
    }

    [Test]
    public void ShouldShowGoldTextForMgReadingsBelowLowInDarkMode()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithLowValue()
              .WithDarkMode()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Gold);
    }

    [Test]
    public void ShouldShowGoldTextForMmolReadingsBelowLowInDarkMode()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithMmolDisplay()
              .WithLowValue()
              .WithDarkMode()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Gold);
    }

    [Test]
    public void ShouldShowRedTextForMgReadingsBelowCriticalLow()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithCriticalLowValue()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Red);
    }

    [Test]
    public void ShouldShowRedTextForMmolReadingsBelowCriticalLow()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithMmolDisplay()
              .WithCriticalLowValue()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Red);
    }

    [Test]
    public void ShouldShowRedTextForMgReadingsAboveCriticalHigh()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithCriticalHighValue()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Red);
    }

    [Test]
    public void ShouldShowRedTextForMmolReadingsAboveCriticalHigh()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithMmolDisplay()
              .WithCriticalHighValue()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Red);
    }

    [Test]
    public void ShouldShowYellowTextForMgReadingsAboveHigh()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithHighValue()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Yellow);
    }

    [Test]
    public void ShouldShowYellowTextForMmolReadingsAboveHigh()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithMmolDisplay()
              .WithHighValue()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Yellow);
    }

    [Test]
    public void ShouldShowGoldTextForMgReadingsAboveHighInDarkMode()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithHighValue()
              .WithDarkMode()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Gold);
    }

    [Test]
    public void ShouldShowGoldTextForMmolReadingsAboveHighInDarkMode()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithMmolDisplay()
              .WithHighValue()
              .WithDarkMode()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Gold);
    }

    [Test]
    public void ShouldUseDefaultFontSizeForAllMgReadings()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithMgValue(100)
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithFontSize(40);
    }

    [Test]
    public void ShouldUseDefaultFontSizeForMmolReadingsBelow10()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithMmolDisplay()
              .WithMmolValue(5.5f)
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithFontSize(40);
    }

    [Test]
    public void ShouldUseSmallerFontSizeForMmolReadings10AndAbove()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithMmolDisplay()
              .WithMmolValue(15.5f)
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithFontSize(38);
    }

    [Test]
    public void ShouldHaveTheCorrectDisplayMessage()
    {
        var display = new GlucoseDisplay
        {
            TimestampUtc = new DateTime(2000, 5, 5, 11, 0, 0, DateTimeKind.Utc),
            DisplayValue = "100",
            IsStale = true,
            Trend = Trend.Flat,
        };

        var result = display.GetDisplayMessage(new DateTime(2000, 5, 5, 10, 0, 0, DateTimeKind.Utc));

        Assert.That(result, Is.EqualTo($"100 6:00:00 AM → \r\n60 minutes ago"));
    }
}
