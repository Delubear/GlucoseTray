using GlucoseTray.Domain.Enums;

namespace GlucoseTray.Domain.FetchResults;

public static class GlucoseMath
{
    private static bool IsCriticalLow(GlucoseResult result, ISettingsProxy currentSettings)
    {
        if (result.MmolValue == 0) // Don't treat a zero / null / default result as critical low.
            return false;
        return currentSettings.GlucoseUnit == GlucoseUnitType.MMOL && result.MmolValue <= currentSettings.CriticalLowBg
            || currentSettings.GlucoseUnit == GlucoseUnitType.MG && result.MgValue <= currentSettings.CriticalLowBg;
    }

    public static void CalculateValues(GlucoseResult result, double value, ISettingsProxy currentSettings)
    {
        if (value == 0)
        {
            result.MmolValue = value;
            result.MgValue = Convert.ToInt32(value);
        }
        else if (currentSettings.IsServerDataUnitTypeMmol)
        {
            result.MmolValue = value;
            result.MgValue = Convert.ToInt32(value * 18);
        }
        else
        {
            result.MmolValue = value / 18;
            result.MgValue = Convert.ToInt32(value);
        }
        result.IsCriticalLow = IsCriticalLow(result, currentSettings);
    }
}