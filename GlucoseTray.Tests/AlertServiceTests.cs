
using GlucoseTray.Models;
using GlucoseTray.Enums;
using GlucoseTray.Services;
using NSubstitute;
using GlucoseTray.Settings;

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
        var options = Substitute.For<ISettingsProxy>();
        options.HighAlert.Returns(true);
        options.HighBg.Returns(alertValue);
        options.GlucoseUnit.Returns(unitType);
        options.StaleResultsThreshold.Returns(15);
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
        var options = Substitute.For<ISettingsProxy>();
        options.WarningHighAlert.Returns(true);
        options.WarningHighBg.Returns(alertValue);
        options.GlucoseUnit.Returns(unitType);
        options.StaleResultsThreshold.Returns(15);
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
        var options = Substitute.For<ISettingsProxy>();
        options.CriticalLowAlert.Returns(true);
        options.CriticalLowBg.Returns(alertValue);
        options.GlucoseUnit.Returns(unitType);
        options.StaleResultsThreshold.Returns(15);
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
        var options = Substitute.For<ISettingsProxy>();
        options.LowAlert.Returns(true);
        options.LowBg.Returns(alertValue);
        options.GlucoseUnit.Returns(unitType);
        options.StaleResultsThreshold.Returns(15);

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
        var options = Substitute.For<ISettingsProxy>();
        options.WarningLowAlert.Returns(true);
        options.WarningLowBg.Returns(alertValue);
        options.GlucoseUnit.Returns(unitType);
        options.StaleResultsThreshold.Returns(15);
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
        var options = Substitute.For<ISettingsProxy>();
        options.HighAlert.Returns(true);
        options.HighBg.Returns(200);
        options.GlucoseUnit.Returns(GlucoseUnitType.MG);
        options.StaleResultsThreshold.Returns(15);
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
        var options = Substitute.For<ISettingsProxy>();
        options.HighAlert.Returns(true);
        options.HighBg.Returns(200);
        options.GlucoseUnit.Returns(GlucoseUnitType.MG);
        options.StaleResultsThreshold.Returns(15);
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
