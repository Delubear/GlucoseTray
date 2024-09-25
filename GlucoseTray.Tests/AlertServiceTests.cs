
using GlucoseTray.Models;
using GlucoseTray.Enums;
using GlucoseTray.Services;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace GlucoseTray.Tests;

public class AlertServiceTests
{
    [Test]
    [TestCase(GlucoseUnitType.MG, 101, 1, 100)]
    [TestCase(GlucoseUnitType.MG, 100, 1, 100)]
    [TestCase(GlucoseUnitType.MMOL, 1, 10.1, 10)]
    [TestCase(GlucoseUnitType.MMOL, 1, 10, 10)]
    public void AlertNotification_WhenHighAlertTriggered_ShouldShowHighAlert(GlucoseUnitType unitType, int mgValue, double mmolValue, double alertValue)
    {
        // Arrange
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(new GlucoseTraySettings
        {
            HighAlert = true,
            HighBg = alertValue,
            GlucoseUnit = unitType,
            StaleResultsThreshold = 15,
        });
        var uiService = Substitute.For<IUiService>();
        var alertService = new AlertService(options, uiService);
        var glucoseResult = new GlucoseResult
        {
            MgValue = mgValue,
            MmolValue = mmolValue,
            DateTimeUTC = DateTime.UtcNow,
        };

        // Act
        alertService.AlertNotification(glucoseResult);

        // Assert
        uiService.Received().ShowAlert("High Glucose Alert");
    }

    [Test]
    [TestCase(GlucoseUnitType.MG, 101, 1, 100)]
    [TestCase(GlucoseUnitType.MG, 100, 1, 100)]
    [TestCase(GlucoseUnitType.MMOL, 1, 10.1, 10)]
    [TestCase(GlucoseUnitType.MMOL, 1, 10, 10)]
    public void AlertNotification_WhenWarningHighAlertTriggered_ShouldShowWarningHighAlert(GlucoseUnitType unitType, int mgValue, double mmolValue, double alertValue)
    {
        // Arrange
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(new GlucoseTraySettings
        {
            WarningHighAlert = true,
            WarningHighBg = alertValue,
            GlucoseUnit = unitType,
            StaleResultsThreshold = 15,
        });
        var uiService = Substitute.For<IUiService>();
        var alertService = new AlertService(options, uiService);
        var glucoseResult = new GlucoseResult
        {
            MgValue = mgValue,
            MmolValue = mmolValue,
            DateTimeUTC = DateTime.UtcNow,
        };

        // Act
        alertService.AlertNotification(glucoseResult);

        // Assert
        uiService.Received().ShowAlert("Warning High Glucose Alert");
    }

    [Test]
    [TestCase(GlucoseUnitType.MG, 55, 1, 55)]
    [TestCase(GlucoseUnitType.MG, 54, 1, 55)]
    [TestCase(GlucoseUnitType.MMOL, 1, 4, 4)]
    [TestCase(GlucoseUnitType.MMOL, 1, 3.9, 4)]
    public void AlertNotification_WhenCriticalLowAlertTriggered_ShouldShowCriticalLowAlert(GlucoseUnitType unitType, int mgValue, double mmolValue, double alertValue)
    {
        // Arrange
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(new GlucoseTraySettings
        {
            CriticallyLowAlert = true,
            CriticalLowBg = alertValue,
            GlucoseUnit = unitType,
            StaleResultsThreshold = 15,
        });
        var uiService = Substitute.For<IUiService>();
        var alertService = new AlertService(options, uiService);
        var glucoseResult = new GlucoseResult
        {
            MgValue = mgValue,
            MmolValue = mmolValue,
            DateTimeUTC = DateTime.UtcNow,
        };

        // Act
        alertService.AlertNotification(glucoseResult);

        // Assert
        uiService.Received().ShowCriticalAlert("Critical Low Glucose Alert", "Critical Low Glucose Alert");
    }

    [Test]
    [TestCase(GlucoseUnitType.MG, 55, 1, 55)]
    [TestCase(GlucoseUnitType.MG, 54, 1, 55)]
    [TestCase(GlucoseUnitType.MMOL, 1, 4, 4)]
    [TestCase(GlucoseUnitType.MMOL, 1, 3.9, 4)]
    public void AlertNotification_WhenLowAlertTriggered_ShouldShowLowAlert(GlucoseUnitType unitType, int mgValue, double mmolValue, double alertValue)
    {
        // Arrange
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(new GlucoseTraySettings
        {
            LowAlert = true,
            LowBg = alertValue,
            GlucoseUnit = unitType,
            StaleResultsThreshold = 15,
        });
        var uiService = Substitute.For<IUiService>();
        var alertService = new AlertService(options, uiService);
        var glucoseResult = new GlucoseResult
        {
            MgValue = mgValue,
            MmolValue = mmolValue,
            DateTimeUTC = DateTime.UtcNow,
        };

        // Act
        alertService.AlertNotification(glucoseResult);

        // Assert
        uiService.Received().ShowAlert("Low Glucose Alert");
    }

    [Test]
    [TestCase(GlucoseUnitType.MG, 55, 1, 55)]
    [TestCase(GlucoseUnitType.MG, 54, 1, 55)]
    [TestCase(GlucoseUnitType.MMOL, 1, 4, 4)]
    [TestCase(GlucoseUnitType.MMOL, 1, 3.9, 4)]
    public void AlertNotification_WhenWarningLowAlertTriggered_ShouldShowWarningLowAlert(GlucoseUnitType unitType, int mgValue, double mmolValue, double alertValue)
    {
        // Arrange
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(new GlucoseTraySettings
        {
            WarningLowAlert = true,
            WarningLowBg = alertValue,
            GlucoseUnit = unitType,
            StaleResultsThreshold = 15,
        });
        var uiService = Substitute.For<IUiService>();
        var alertService = new AlertService(options, uiService);
        var glucoseResult = new GlucoseResult
        {
            MgValue = mgValue,
            MmolValue = mmolValue,
            DateTimeUTC = DateTime.UtcNow,
        };

        // Act
        alertService.AlertNotification(glucoseResult);

        // Assert
        uiService.Received().ShowAlert("Warning Low Glucose Alert");
    }

    [Test]
    public void AlertNotification_WhenNoAlertTriggered_ShouldNotShowAlert()
    {
        // Arrange
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(new GlucoseTraySettings
        {
            HighAlert = true,
            HighBg = 200,
            GlucoseUnit = GlucoseUnitType.MG,
            StaleResultsThreshold = 15,
        });
        var uiService = Substitute.For<IUiService>();
        var alertService = new AlertService(options, uiService);
        var glucoseResult = new GlucoseResult
        {
            MgValue = 100,
            MmolValue = 5,
            DateTimeUTC = DateTime.UtcNow,
        };

        // Act
        alertService.AlertNotification(glucoseResult);

        // Assert
        uiService.DidNotReceive().ShowAlert(Arg.Any<string>());
        uiService.DidNotReceive().ShowCriticalAlert(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public void AlertNotification_WhenStale_ShouldNotShowAlert()
    {
        // Arrange
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(new GlucoseTraySettings
        {
            HighAlert = true,
            HighBg = 200,
            GlucoseUnit = GlucoseUnitType.MG,
            StaleResultsThreshold = 15,
        });
        var uiService = Substitute.For<IUiService>();
        var alertService = new AlertService(options, uiService);
        var glucoseResult = new GlucoseResult
        {
            MgValue = 400,
            MmolValue = 40,
            DateTimeUTC = DateTime.UtcNow.AddMinutes(-30),
        };

        // Act
        alertService.AlertNotification(glucoseResult);

        // Assert
        uiService.DidNotReceive().ShowAlert(Arg.Any<string>());
        uiService.DidNotReceive().ShowCriticalAlert(Arg.Any<string>(), Arg.Any<string>());
    }
}
