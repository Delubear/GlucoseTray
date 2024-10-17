using GlucoseTray.Domain.Enums;
using GlucoseTray.Domain.GlucoseSettings;

namespace GlucoseTray.Domain.DisplayResults;

public class AlertService(ISettingsProxy options, IIconService iconService, IDialogService dialogService, GlucoseResult glucoseResult)
{
    private AlertLevel _currentAlertLevel = AlertLevel.None;

    public void AlertNotification()
    {
        if (glucoseResult.IsStale(options.StaleResultsThreshold))
            return;

        var highAlertTriggered = options.HighAlert && IsAlertTriggered(glucoseResult.MgValue, glucoseResult.MmolValue, options.HighBg, UpDown.Down);
        var warningHighAlertTriggered = options.WarningHighAlert && IsAlertTriggered(glucoseResult.MgValue, glucoseResult.MmolValue, options.WarningHighBg, UpDown.Down);
        var warningLowAlertTriggered = options.WarningLowAlert && IsAlertTriggered(glucoseResult.MgValue, glucoseResult.MmolValue, options.WarningLowBg, UpDown.Up);
        var lowAlertTriggered = options.LowAlert && IsAlertTriggered(glucoseResult.MgValue, glucoseResult.MmolValue, options.LowBg, UpDown.Up);
        var criticalLowAlertTriggered = options.CriticalLowAlert && IsAlertTriggered(glucoseResult.MgValue, glucoseResult.MmolValue, options.CriticalLowBg, UpDown.Up);

        // Order matters. We need to show the most severe alert while avoid multiple alerts. (High > warning high.  Critical > low > warning low)
        if (highAlertTriggered)
        {
            if (_currentAlertLevel != AlertLevel.High)
                iconService.ShowTrayNotification("High Glucose Alert");
            _currentAlertLevel = AlertLevel.High;
            return;
        }
        if (warningHighAlertTriggered)
        {
            if (_currentAlertLevel != AlertLevel.High && _currentAlertLevel != AlertLevel.WarningHigh)
                iconService.ShowTrayNotification("Warning High Glucose Alert");
            _currentAlertLevel = AlertLevel.WarningHigh;
            return;
        }
        if (criticalLowAlertTriggered)
        {
            if (_currentAlertLevel != AlertLevel.CriticalLow)
                dialogService.ShowCriticalAlert("Critical Low Glucose Alert", "Critical Low Glucose Alert");
            _currentAlertLevel = AlertLevel.CriticalLow;
            return;
        }
        if (lowAlertTriggered)
        {
            if (_currentAlertLevel != AlertLevel.CriticalLow && _currentAlertLevel != AlertLevel.Low)
                iconService.ShowTrayNotification("Low Glucose Alert");
            _currentAlertLevel = AlertLevel.Low;
            return;
        }
        if (warningLowAlertTriggered)
        {
            if (_currentAlertLevel != AlertLevel.CriticalLow && _currentAlertLevel != AlertLevel.Low && _currentAlertLevel != AlertLevel.WarningLow)
                iconService.ShowTrayNotification("Warning Low Glucose Alert");
            _currentAlertLevel = AlertLevel.WarningLow;
            return;
        }
        _currentAlertLevel = AlertLevel.None;
    }

    // TODO: We shouldn't be needing both MG and MMOL here.  We should be able to use one or the other.
    private bool IsAlertTriggered(double glucoseValueMG, double glucoseValueMMOL, double alertThreshold, UpDown directionGlucoseShouldBeToNotAlert)
    {
        if (glucoseValueMMOL == 0) // If a null / default result is returned, do not trigger alerts.
            return false;
        if (options.GlucoseUnit == GlucoseUnitType.MG)
        {
            if (directionGlucoseShouldBeToNotAlert == UpDown.Down)
                return glucoseValueMG >= alertThreshold;
            else
                return glucoseValueMG <= alertThreshold;
        }
        else
        {
            if (directionGlucoseShouldBeToNotAlert == UpDown.Down)
                return glucoseValueMMOL >= alertThreshold;
            else
                return glucoseValueMMOL <= alertThreshold;
        }
    }
}
