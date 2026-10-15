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
    public void ShouldAllowTheOriginalCompactMmolSeparator()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithMmolDisplay()
              .WithMmolSeparator(MmolSeparatorStyle.Apostrophe)
              .WithMmolValue(5.5f)
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithValue("5'5");
    }

    [Test]
    public void ShouldShowGreenTextForStandardMmolReadingInDarkMode()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithMmolDisplay()
              .WithMmolValue(5.5f)
              .WithDarkMode()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Green);
    }

    [Test]
    public void ShouldShowGreenTextForStandardMgReadingInDarkMode()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithMgValue(100)
              .WithDarkMode()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Green);
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
    public void ShouldShowGoldTextForMgReadingsBelowLow()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithLowValue()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Gold);
    }

    [Test]
    public void ShouldShowGoldTextForMmolReadingsBelowLow()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithMmolDisplay()
              .WithLowValue()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Gold);
    }

    [Test]
    public void ShouldShowYellowTextForMgReadingsBelowLowInDarkMode()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithLowValue()
              .WithDarkMode()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Yellow);
    }

    [Test]
    public void ShouldShowYellowTextForMmolReadingsBelowLowInDarkMode()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithMmolDisplay()
              .WithLowValue()
              .WithDarkMode()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Yellow);
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
    public void ShouldShowGoldTextForMgReadingsAboveHigh()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithHighValue()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Gold);
    }

    [Test]
    public void ShouldShowGoldTextForMmolReadingsAboveHigh()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithMmolDisplay()
              .WithHighValue()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Gold);
    }

    [Test]
    public void ShouldShowYellowTextForMgReadingsAboveHighInDarkMode()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithHighValue()
              .WithDarkMode()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Yellow);
    }

    [Test]
    public void ShouldShowYellowTextForMmolReadingsAboveHighInDarkMode()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithMmolDisplay()
              .WithHighValue()
              .WithDarkMode()
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithTextColor(IconTextColor.Yellow);
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

        Assert.That(result, Does.Contain("100 "));
        Assert.That(result, Does.Contain(":00:00 "));
        Assert.That(result, Does.Contain(" → \r\n60 minutes ago"));
    }

    [Test]
    public void ShouldNotShowNotificationWhenStale()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithMgValue(100)
              .WithStaleData()
              .When.RefreshingIcon()
              .Then.ShouldNotShowNotification();
    }

    [Test]
    public void ShouldNotShowNotificationIfReadingIs0()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithMgValue(0)
              .When.RefreshingIcon()
              .Then.ShouldNotShowNotification();
    }

    [Test]
    public void ShouldShowNotificationIfReadingIsAboveCriticalHighThreshold()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithCriticalHighValue()
              .When.RefreshingIcon()
              .Then.ShouldShowNotification("Critical High Glucose Alert");
    }

    [Test]
    public void ShouldShowNotificationIfReadingIsBelowCriticalLowThreshold()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithCriticalLowValue()
              .When.RefreshingIcon()
              .Then.ShouldShowNotification("Critical Low Glucose Alert");
    }

    [Test]
    public void ShouldShowNotificationIfReadingIsAboveHighThreshold()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithHighValue()
              .When.RefreshingIcon()
              .Then.ShouldShowNotification("High Glucose Alert");
    }

    [Test]
    public void ShouldShowNotificationIfReadingIsBelowLowThreshold()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithLowValue()
              .When.RefreshingIcon()
              .Then.ShouldShowNotification("Low Glucose Alert");
    }

    [Test]
    public void ShouldNotShowNotificationIfReadingIsWithinNormalRange()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithMgValue(100)
              .When.RefreshingIcon()
              .Then.ShouldNotShowNotification();
    }

    [Test]
    public void ShouldNotShowNotificationIfReadingIsWithinNormalRangeInMmol()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithMmolDisplay()
              .WithMmolValue(5.5f)
              .When.RefreshingIcon()
              .Then.ShouldNotShowNotification();
    }

    [Test]
    public void ShouldNotShowRepeatNotificationsForCriticalHigh()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithCriticalHighValue()
              .When.RefreshingIcon()
              .Then.ShouldShowNotification("Critical High Glucose Alert")
              .When.RefreshingIcon()
              .Then.ShouldNotShowNotification();
    }

    [Test]
    public void ShouldNotShowRepeatNotificationsForCriticalLow()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithCriticalLowValue()
              .When.RefreshingIcon()
              .Then.ShouldShowNotification("Critical Low Glucose Alert")
              .When.RefreshingIcon()
              .Then.ShouldNotShowNotification();
    }

    [Test]
    public void ShouldNotShowRepeatNotificationsForHigh()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithHighValue()
              .When.RefreshingIcon()
              .Then.ShouldShowNotification("High Glucose Alert")
              .When.RefreshingIcon()
              .Then.ShouldNotShowNotification();
    }

    [Test]
    public void ShouldNotShowRepeatNotificationsForLow()
    {
        var driver = new DisplayDriver();
        driver.GivenAGlucoseReading()
              .WithLowValue()
              .When.RefreshingIcon()
              .Then.ShouldShowNotification("Low Glucose Alert")
              .When.RefreshingIcon()
              .Then.ShouldNotShowNotification();
    }

    [TestCase("en-US", MmolSeparatorStyle.System, "5.5")]
    [TestCase("de-DE", MmolSeparatorStyle.System, "5,5")]
    [TestCase("de-CH", MmolSeparatorStyle.System, "5.5")]
    [TestCase("fr-CH", MmolSeparatorStyle.System, "5,5")]
    [TestCase("de-DE", MmolSeparatorStyle.DecimalPoint, "5.5")]
    [TestCase("en-US", MmolSeparatorStyle.Apostrophe, "5'5")]
    [TestCase("de-DE", MmolSeparatorStyle.Apostrophe, "5'5")]
    public void ShouldApplyTheSelectedMmolSeparator(string culture, MmolSeparatorStyle separator, string expected)
    {
        var original = Thread.CurrentThread.CurrentCulture;
        try
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(culture);

            var driver = new DisplayDriver();
            driver.GivenAGlucoseReading()
                  .WithMmolDisplay()
                  .WithMmolSeparator(separator)
                  .WithMmolValue(5.5f)
                  .When.RefreshingIcon()
                  .Then.ShouldBeRefreshedWithValue(expected);
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = original;
        }
    }

    [Test]
    public void DefaultSeparatorShouldRespectCustomNumberFormatsRatherThanUiLanguage()
    {
        var original = Thread.CurrentThread.CurrentCulture;
        var originalUi = Thread.CurrentThread.CurrentUICulture;
        try
        {
            var culture = (System.Globalization.CultureInfo)new System.Globalization.CultureInfo("en-US").Clone();
            culture.NumberFormat.NumberDecimalSeparator = ",";
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            new DisplayDriver().GivenAGlucoseReading()
                .WithMmolDisplay().WithMmolValue(8.1f)
                .When.RefreshingIcon().Then.ShouldBeRefreshedWithValue("8,1");
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = original;
            Thread.CurrentThread.CurrentUICulture = originalUi;
        }
    }

    [TestCase(MmolSeparatorStyle.System)]
    [TestCase(MmolSeparatorStyle.DecimalPoint)]
    [TestCase(MmolSeparatorStyle.Apostrophe)]
    public void MmolSeparatorShouldNotChangeMgDisplay(MmolSeparatorStyle separator)
    {
        new DisplayDriver().GivenAGlucoseReading()
            .WithMmolSeparator(separator).WithMgValue(100)
            .When.RefreshingIcon().Then.ShouldBeRefreshedWithValue("100");
    }
}
