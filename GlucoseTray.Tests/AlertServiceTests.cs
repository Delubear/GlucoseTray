
using NSubstitute;
using GlucoseTray.Domain.Enums;
using GlucoseTray.Domain;
using GlucoseTray.Domain.DisplayResults;

namespace GlucoseTray.Tests;

public class AlertServiceTests
{
    private GlucoseResult GetGlucoseResult(ISettingsProxy settingsProxy, double value, DateTime dateTimeUtc)
    {
        var glucoseResult = new GlucoseResult();
        glucoseResult.SetGlucoseValues(value, settingsProxy);
        glucoseResult.SetDateTimeUtc(dateTimeUtc);
        return glucoseResult;
    }

    [Test]
    [TestCase(GlucoseUnitType.MG, 101, 100)]
    [TestCase(GlucoseUnitType.MG, 100, 100)]
    [TestCase(GlucoseUnitType.MMOL, 10.1, 10)]
    [TestCase(GlucoseUnitType.MMOL, 10, 10)]
    public void AlertNotification_WhenHighAlertTriggered_ShouldShowHighAlert(GlucoseUnitType unitType, double value, double alertValue)
    {
        // Arrange
        var options = Substitute.For<ISettingsProxy>();
        options.HighAlert.Returns(true);
        options.HighBg.Returns(alertValue);
        options.GlucoseUnit.Returns(unitType);
        options.StaleResultsThreshold.Returns(15);
        options.IsServerDataUnitTypeMmol.Returns(unitType == GlucoseUnitType.MMOL);
        var dialogService = Substitute.For<IDialogService>();
        var iconService = Substitute.For<IIconService>();
        var alertService = new AlertService(options, iconService, dialogService);
        var glucoseResult = GetGlucoseResult(options, value, DateTime.UtcNow);

        // Act
        alertService.AlertNotification(glucoseResult);

        // Assert
        iconService.Received().ShowTrayNotification("High Glucose Alert");
    }

    [Test]
    [TestCase(GlucoseUnitType.MG, 101, 100)]
    [TestCase(GlucoseUnitType.MG, 100, 100)]
    [TestCase(GlucoseUnitType.MMOL, 10.1, 10)]
    [TestCase(GlucoseUnitType.MMOL, 10, 10)]
    public void AlertNotification_WhenWarningHighAlertTriggered_ShouldShowWarningHighAlert(GlucoseUnitType unitType, double value, double alertValue)
    {
        // Arrange
        var options = Substitute.For<ISettingsProxy>();
        options.WarningHighAlert.Returns(true);
        options.WarningHighBg.Returns(alertValue);
        options.GlucoseUnit.Returns(unitType);
        options.StaleResultsThreshold.Returns(15);
        options.IsServerDataUnitTypeMmol.Returns(unitType == GlucoseUnitType.MMOL);
        var dialogService = Substitute.For<IDialogService>();
        var iconService = Substitute.For<IIconService>();
        var alertService = new AlertService(options, iconService, dialogService);
        var glucoseResult = GetGlucoseResult(options, value, DateTime.UtcNow);

        // Act
        alertService.AlertNotification(glucoseResult);

        // Assert
        iconService.Received().ShowTrayNotification("Warning High Glucose Alert");
    }

    [Test]
    [TestCase(GlucoseUnitType.MG, 55, 55)]
    [TestCase(GlucoseUnitType.MG, 54, 55)]
    [TestCase(GlucoseUnitType.MMOL, 4, 4)]
    [TestCase(GlucoseUnitType.MMOL, 3.9, 4)]
    public void AlertNotification_WhenCriticalLowAlertTriggered_ShouldShowCriticalLowAlert(GlucoseUnitType unitType, double value, double alertValue)
    {
        // Arrange
        var options = Substitute.For<ISettingsProxy>();
        options.CriticalLowAlert.Returns(true);
        options.CriticalLowBg.Returns(alertValue);
        options.GlucoseUnit.Returns(unitType);
        options.StaleResultsThreshold.Returns(15);
        options.IsServerDataUnitTypeMmol.Returns(unitType == GlucoseUnitType.MMOL);
        var dialogService = Substitute.For<IDialogService>();
        var iconService = Substitute.For<IIconService>();
        var alertService = new AlertService(options, iconService, dialogService);
        var glucoseResult = GetGlucoseResult(options, value, DateTime.UtcNow);

        // Act
        alertService.AlertNotification(glucoseResult);

        // Assert
        dialogService.Received().ShowCriticalAlert("Critical Low Glucose Alert", "Critical Low Glucose Alert");
    }

    [Test]
    [TestCase(GlucoseUnitType.MG, 55, 55)]
    [TestCase(GlucoseUnitType.MG, 54, 55)]
    [TestCase(GlucoseUnitType.MMOL, 4, 4)]
    [TestCase(GlucoseUnitType.MMOL, 3.9, 4)]
    public void AlertNotification_WhenLowAlertTriggered_ShouldShowLowAlert(GlucoseUnitType unitType, double value, double alertValue)
    {
        // Arrange
        var options = Substitute.For<ISettingsProxy>();
        options.LowAlert.Returns(true);
        options.LowBg.Returns(alertValue);
        options.GlucoseUnit.Returns(unitType);
        options.StaleResultsThreshold.Returns(15);
        options.IsServerDataUnitTypeMmol.Returns(unitType == GlucoseUnitType.MMOL);
        var dialogService = Substitute.For<IDialogService>();
        var iconService = Substitute.For<IIconService>();
        var alertService = new AlertService(options, iconService, dialogService);
        var glucoseResult = GetGlucoseResult(options, value, DateTime.UtcNow);

        // Act
        alertService.AlertNotification(glucoseResult);

        // Assert
        iconService.Received().ShowTrayNotification("Low Glucose Alert");
    }

    [Test]
    [TestCase(GlucoseUnitType.MG, 55, 55)]
    [TestCase(GlucoseUnitType.MG, 54, 55)]
    [TestCase(GlucoseUnitType.MMOL, 4, 4)]
    [TestCase(GlucoseUnitType.MMOL, 3.9, 4)]
    public void AlertNotification_WhenWarningLowAlertTriggered_ShouldShowWarningLowAlert(GlucoseUnitType unitType, double value, double alertValue)
    {
        // Arrange
        var options = Substitute.For<ISettingsProxy>();
        options.WarningLowAlert.Returns(true);
        options.WarningLowBg.Returns(alertValue);
        options.GlucoseUnit.Returns(unitType);
        options.StaleResultsThreshold.Returns(15);
        options.IsServerDataUnitTypeMmol.Returns(unitType == GlucoseUnitType.MMOL);
        var dialogService = Substitute.For<IDialogService>();
        var iconService = Substitute.For<IIconService>();
        var alertService = new AlertService(options, iconService, dialogService);
        var glucoseResult = GetGlucoseResult(options, value, DateTime.UtcNow);

        // Act
        alertService.AlertNotification(glucoseResult);

        // Assert
        iconService.Received().ShowTrayNotification("Warning Low Glucose Alert");
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
        options.IsServerDataUnitTypeMmol.Returns(false);
        var dialogService = Substitute.For<IDialogService>();
        var iconService = Substitute.For<IIconService>();
        var alertService = new AlertService(options, iconService, dialogService);
        var glucoseResult = GetGlucoseResult(options, 100, DateTime.UtcNow);

        // Act
        alertService.AlertNotification(glucoseResult);

        // Assert
        iconService.DidNotReceive().ShowTrayNotification(Arg.Any<string>());
        dialogService.DidNotReceive().ShowCriticalAlert(Arg.Any<string>(), Arg.Any<string>());
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
        options.IsServerDataUnitTypeMmol.Returns(false);
        var dialogService = Substitute.For<IDialogService>();
        var iconService = Substitute.For<IIconService>();
        var alertService = new AlertService(options, iconService, dialogService);
        var glucoseResult = GetGlucoseResult(options, 500, DateTime.UtcNow.AddMinutes(-30));

        // Act
        alertService.AlertNotification(glucoseResult);

        // Assert
        iconService.DidNotReceive().ShowTrayNotification(Arg.Any<string>());
        dialogService.DidNotReceive().ShowCriticalAlert(Arg.Any<string>(), Arg.Any<string>());
    }
}
