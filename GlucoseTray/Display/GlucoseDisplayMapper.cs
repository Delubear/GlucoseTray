using Microsoft.Extensions.Options;

namespace GlucoseTray.Display;

public interface IGlucoseDisplayMapper
{
    GlucoseDisplay Map(GlucoseReading reading);
}

public class GlucoseDisplayMapper(IOptionsMonitor<AppSettings> options) : IGlucoseDisplayMapper
{
    public GlucoseDisplay Map(GlucoseReading reading)
    {
        return new GlucoseDisplay
        {
            DisplayValue = GetDisplayValue(reading),
            Color = options.CurrentValue.DisplayUnitType == GlucoseUnitType.Mg ? GetColor(reading.MgValue) : GetColor(reading.MmolValue),
            FontSize = GetFontSize(reading),
            TimestampUtc = reading.TimestampUtc,
            Trend = reading.Trend,
            IsStale = reading.TimestampUtc < DateTime.Now.AddMinutes(-options.CurrentValue.MinutesUntilStale),
        };
    }

    private int GetFontSize(GlucoseReading reading)
    {
        var defaultFontSize = 40;
        var smallerFontSize = 38;

        return options.CurrentValue.DisplayUnitType == GlucoseUnitType.Mmol && reading.MmolValue >= 10 ? smallerFontSize : defaultFontSize;
    }

    private string GetDisplayValue(GlucoseReading reading)
    {
        var displayUnitType = options.CurrentValue.DisplayUnitType;

        if (displayUnitType == GlucoseUnitType.Mg)
        {
            if (reading.MgValue == 0)
                return "NUL";
            if (IsCriticalLow(reading.MgValue))
                return "DAN";
            return reading.MgValue.ToString();
        }
        else
        {
            if (reading.MmolValue == 0)
                return "NUL";
            if (IsCriticalLow(reading.MmolValue))
                return "DAN";
            return reading.MmolValue.ToString("0.0").Replace('.', '\''); // ' uses less space than .
        }
    }

    private bool IsCriticalLow(int mgValue) => mgValue <= options.CurrentValue.CriticalLowMgThreshold;
    private bool IsCriticalLow(float mmolValue) => mmolValue <= options.CurrentValue.CriticalLowMmolThreshold;

    private IconTextColor GetColor(int mgValue)
    {
        var settings = options.CurrentValue;
        return GetColor(mgValue, settings.CriticalLowMgThreshold, settings.LowMgThreshold, settings.HighMgThreshold, settings.CriticalHighMgThreshold);
    }

    private IconTextColor GetColor(float mmolValue)
    {
        var settings = options.CurrentValue;
        return GetColor(mmolValue, settings.CriticalLowMmolThreshold, settings.LowMmolThreshold, settings.HighMmolThreshold, settings.CriticalHighMmolThreshold);
    }

    private IconTextColor GetColor(float value, float criticalLow, float low, float high, float criticalHigh)
    {
        if (value <= criticalLow)
            return IconTextColor.Red;
        else if (value <= low)
            return options.CurrentValue.IsDarkMode ? IconTextColor.Gold : IconTextColor.Yellow;
        else if (value >= criticalHigh)
            return IconTextColor.Red;
        else if (value >= high)
            return options.CurrentValue.IsDarkMode ? IconTextColor.Gold : IconTextColor.Yellow;
        else
            return options.CurrentValue.IsDarkMode ? IconTextColor.White : IconTextColor.Black;
    }
}
