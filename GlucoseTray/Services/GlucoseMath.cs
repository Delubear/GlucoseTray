using GlucoseTray.Enums;
using GlucoseTray.Models;
using System;

namespace GlucoseTray.Services
{
    internal static class GlucoseMath
    {
        private static bool IsCriticalLow(GlucoseResult result, GlucoseTraySettings currentSettings) =>
            (currentSettings.GlucoseUnit == GlucoseUnitType.MMOL && result.MmolValue <= currentSettings.CriticalLowBg)
            || (currentSettings.GlucoseUnit == GlucoseUnitType.MG && result.MgValue <= currentSettings.CriticalLowBg);

        internal static void CalculateValues(GlucoseResult result, double value, GlucoseTraySettings currentSettings)
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
}
