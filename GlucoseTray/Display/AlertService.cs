
using GlucoseTray.Enums;
using Microsoft.Extensions.Options;

namespace GlucoseTray.Display;

public interface IAlertService
{
    string GetAlertMessage(int mgValue, float mmolValue, bool isStale);
}

public class AlertService(IOptionsMonitor<AppSettings> options) : IAlertService
{
    private AlertLevel _currentAlertLevel = AlertLevel.None;

    public string GetAlertMessage(int mgValue, float mmolValue, bool isStale)
    {
        if (isStale)
            return string.Empty;

        if (mgValue == 0 || mmolValue == 0)
            return string.Empty;

        var reading = options.CurrentValue.DisplayUnitType == GlucoseUnitType.Mg ? mgValue : mmolValue;

        var settings = options.CurrentValue;
        var criticalHigh = settings.DisplayUnitType == GlucoseUnitType.Mg ? settings.CriticalHighMgThreshold : settings.CriticalHighMmolThreshold;
        var high = settings.DisplayUnitType == GlucoseUnitType.Mg ? settings.HighMgThreshold : settings.HighMmolThreshold;
        var low = settings.DisplayUnitType == GlucoseUnitType.Mg ? settings.LowMgThreshold : settings.LowMmolThreshold;
        var criticalLow = settings.DisplayUnitType == GlucoseUnitType.Mg ? settings.CriticalLowMgThreshold : settings.CriticalLowMmolThreshold;

        if (reading >= criticalHigh)
        {
            if (_currentAlertLevel != AlertLevel.CriticalHigh)
            {
                _currentAlertLevel = AlertLevel.CriticalHigh;
                return "Critical High Glucose Alert";
            }
        }
        else if (reading >= high)
        {
            if (_currentAlertLevel != AlertLevel.CriticalHigh && _currentAlertLevel != AlertLevel.High)
            {
                _currentAlertLevel = AlertLevel.High;
                return "High Glucose Alert";
            }
        }
        else if (reading <= criticalLow)
        {
            if (_currentAlertLevel != AlertLevel.CriticalLow)
            {
                _currentAlertLevel = AlertLevel.CriticalLow;
                return "Critical Low Glucose Alert";
            }
        }
        else if (reading <= low)
        {
            if (_currentAlertLevel != AlertLevel.CriticalLow && _currentAlertLevel != AlertLevel.Low)
            {
                _currentAlertLevel = AlertLevel.Low;
                return "Low Glucose Alert";
            }
        }

        return string.Empty;
    }
}
