using GlucoseTray.Display;
using GlucoseTray.Tests.DSL;

namespace GlucoseTray.Tests;

public class DisplayTests
{
    [Test]
    public void ShouldMapMgToDisplay()
    {
        var driver = new AppDriver();
        driver.GivenAGlucoseReading()
              .WithMgValue(100)
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithValue("100");
    }

    [Test]
    public void ShouldMapMmolToDisplay()
    {
        var driver = new AppDriver();
        driver.GivenAGlucoseReading()
              .WithMmolDisplay()
              .WithMmolValue(5.5f)
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithValue("5'5");
    }

    [Test]
    public void ShouldShowWhiteTextForStandardMmolReadingInDarkMode()
    {
        var driver = new AppDriver();
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
        var driver = new AppDriver();
        driver.GivenAGlucoseReading()
              .WithMgValue(100)
              .WithDarkMode()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.White);
    }

    [Test]
    public void ShouldRecognizeWhenDataIsStale()
    {
        var driver = new AppDriver();
        driver.GivenAGlucoseReading()
              .WithMgValue(100)
              .WithStaleData()
              .When.RefreshingIcon()
              .Then.ShouldBeMarkedStale();
    }

    [Test]
    public void ShouldShowYellowTextForMgReadingsBelowLow()
    {
        var driver = new AppDriver();
        driver.GivenAGlucoseReading()
              .WithLowValue()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Yellow);
    }

    [Test]
    public void ShouldShowYellowTextForMmolReadingsBelowLow()
    {
        var driver = new AppDriver();
        driver.GivenAGlucoseReading()
              .WithMmolDisplay()
              .WithLowValue()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Yellow);
    }

    [Test]
    public void ShouldShowGoldTextForMgReadingsBelowLowInDarkMode()
    {
        var driver = new AppDriver();
        driver.GivenAGlucoseReading()
              .WithLowValue()
              .WithDarkMode()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Gold);
    }

    [Test]
    public void ShouldShowGoldTextForMmolReadingsBelowLowInDarkMode()
    {
        var driver = new AppDriver();
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
        var driver = new AppDriver();
        driver.GivenAGlucoseReading()
              .WithCriticalLowValue()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Red);
    }

    [Test]
    public void ShouldShowRedTextForMmolReadingsBelowCriticalLow()
    {
        var driver = new AppDriver();
        driver.GivenAGlucoseReading()
              .WithMmolDisplay()
              .WithCriticalLowValue()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Red);
    }

    [Test]
    public void ShouldShowRedTextForMgReadingsAboveCriticalHigh()
    {
        var driver = new AppDriver();
        driver.GivenAGlucoseReading()
              .WithCriticalHighValue()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Red);
    }

    [Test]
    public void ShouldShowRedTextForMmolReadingsAboveCriticalHigh()
    {
        var driver = new AppDriver();
        driver.GivenAGlucoseReading()
              .WithMmolDisplay()
              .WithCriticalHighValue()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Red);
    }

    [Test]
    public void ShouldShowYellowTextForMgReadingsAboveHigh()
    {
        var driver = new AppDriver();
        driver.GivenAGlucoseReading()
              .WithHighValue()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Yellow);
    }

    [Test]
    public void ShouldShowYellowTextForMmolReadingsAboveHigh()
    {
        var driver = new AppDriver();
        driver.GivenAGlucoseReading()
              .WithMmolDisplay()
              .WithHighValue()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Yellow);
    }

    [Test]
    public void ShouldShowGoldTextForMgReadingsAboveHighInDarkMode()
    {
        var driver = new AppDriver();
        driver.GivenAGlucoseReading()
              .WithHighValue()
              .WithDarkMode()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Gold);
    }

    [Test]
    public void ShouldShowGoldTextForMmolReadingsAboveHighInDarkMode()
    {
        var driver = new AppDriver();
        driver.GivenAGlucoseReading()
              .WithMmolDisplay()
              .WithHighValue()
              .WithDarkMode()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Gold);
    }
}
