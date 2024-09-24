using Microsoft.Extensions.Options;
using System.Windows.Forms;

namespace GlucoseTray.Services;

public class AlertService(IOptionsMonitor<GlucoseTraySettings> options, IUiService uiService)
{
    private readonly IOptionsMonitor<GlucoseTraySettings> _options = options;
    private readonly IUiService _uiService = uiService;
    private AlertLevel _currentAlertLevel = AlertLevel.None;

    public void AlertNotification(GlucoseResult? currentGlucoseResult)
    {
        if (currentGlucoseResult?.IsStale(_options.CurrentValue.StaleResultsThreshold) != false)
            return;

        var highAlertTriggered = _options.CurrentValue.HighAlert && IsAlertTriggered(currentGlucoseResult.MgValue, currentGlucoseResult.MmolValue, _options.CurrentValue.HighBg, UpDown.Down);
        var warningHighAlertTriggered = _options.CurrentValue.WarningHighAlert && IsAlertTriggered(currentGlucoseResult.MgValue, currentGlucoseResult.MmolValue, _options.CurrentValue.WarningHighBg, UpDown.Down);
        var warningLowAlertTriggered = _options.CurrentValue.WarningLowAlert && IsAlertTriggered(currentGlucoseResult.MgValue, currentGlucoseResult.MmolValue, _options.CurrentValue.WarningLowBg, UpDown.Up);
        var lowAlertTriggered = _options.CurrentValue.LowAlert && IsAlertTriggered(currentGlucoseResult.MgValue, currentGlucoseResult.MmolValue, _options.CurrentValue.LowBg, UpDown.Up);
        var criticalLowAlertTriggered = _options.CurrentValue.CriticallyLowAlert && IsAlertTriggered(currentGlucoseResult.MgValue, currentGlucoseResult.MmolValue, _options.CurrentValue.CriticalLowBg, UpDown.Up);

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
                MessageBox.Show("Critical Low Glucose Alert", "Critical Low Glucose Alert", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        if (_options.CurrentValue.GlucoseUnit == GlucoseUnitType.MG)
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
