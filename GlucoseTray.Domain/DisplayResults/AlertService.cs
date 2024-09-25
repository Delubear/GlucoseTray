using GlucoseTray.Domain.Enums;

namespace GlucoseTray.Domain.DisplayResults;

public class AlertService(ISettingsProxy options, IUiService uiService)
{
    private readonly ISettingsProxy _options = options;
    private readonly IUiService _uiService = uiService;
    private AlertLevel _currentAlertLevel = AlertLevel.None;

    public void AlertNotification(GlucoseResult currentGlucoseResult)
    {
        if (currentGlucoseResult.IsStale(_options.StaleResultsThreshold))
            return;

        var highAlertTriggered = _options.HighAlert && IsAlertTriggered(currentGlucoseResult.MgValue, currentGlucoseResult.MmolValue, _options.HighBg, UpDown.Down);
        var warningHighAlertTriggered = _options.WarningHighAlert && IsAlertTriggered(currentGlucoseResult.MgValue, currentGlucoseResult.MmolValue, _options.WarningHighBg, UpDown.Down);
        var warningLowAlertTriggered = _options.WarningLowAlert && IsAlertTriggered(currentGlucoseResult.MgValue, currentGlucoseResult.MmolValue, _options.WarningLowBg, UpDown.Up);
        var lowAlertTriggered = _options.LowAlert && IsAlertTriggered(currentGlucoseResult.MgValue, currentGlucoseResult.MmolValue, _options.LowBg, UpDown.Up);
        var criticalLowAlertTriggered = _options.CriticalLowAlert && IsAlertTriggered(currentGlucoseResult.MgValue, currentGlucoseResult.MmolValue, _options.CriticalLowBg, UpDown.Up);

        // Order matters. We need to show the most severe alert while avoid multiple alerts. (High > warning high.  Critical > low > warning low)
        if (highAlertTriggered)
        {
            if (_currentAlertLevel != AlertLevel.High)
                _uiService.ShowAlert("High Glucose Alert");
            _currentAlertLevel = AlertLevel.High;
            return;
        }
        if (warningHighAlertTriggered)
        {
            if (_currentAlertLevel != AlertLevel.High && _currentAlertLevel != AlertLevel.WarningHigh)
                _uiService.ShowAlert("Warning High Glucose Alert");
            _currentAlertLevel = AlertLevel.WarningHigh;
            return;
        }
        if (criticalLowAlertTriggered)
        {
            if (_currentAlertLevel != AlertLevel.CriticalLow)
                _uiService.ShowCriticalAlert("Critical Low Glucose Alert", "Critical Low Glucose Alert");
            _currentAlertLevel = AlertLevel.CriticalLow;
            return;
        }
        if (lowAlertTriggered)
        {
            if (_currentAlertLevel != AlertLevel.CriticalLow && _currentAlertLevel != AlertLevel.Low)
                _uiService.ShowAlert("Low Glucose Alert");
            _currentAlertLevel = AlertLevel.Low;
            return;
        }
        if (warningLowAlertTriggered)
        {
            if (_currentAlertLevel != AlertLevel.CriticalLow && _currentAlertLevel != AlertLevel.Low && _currentAlertLevel != AlertLevel.WarningLow)
                _uiService.ShowAlert("Warning Low Glucose Alert");
            _currentAlertLevel = AlertLevel.WarningLow;
            return;
        }
        _currentAlertLevel = AlertLevel.None;
    }

    private bool IsAlertTriggered(double glucoseValueMG, double glucoseValueMMOL, double alertThreshold, UpDown directionGlucoseShouldBeToNotAlert)
    {
        if (glucoseValueMMOL == 0) // If a null / default result is returned, do not trigger alerts.
            return false;
        if (_options.GlucoseUnit == GlucoseUnitType.MG)
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
