using GlucoseTray.Domain.Enums;
using GlucoseTray.Domain.GlucoseSettings;

namespace GlucoseTray.Domain;

public class GlucoseResult
{
    public int MgValue { get; private set; }
    public double MmolValue { get; private set; }
    public DateTime DateTimeUTC { get; private set; }
    public TrendResult Trend { get; private set; }
    public bool IsCriticalLow { get; private set; }

    private readonly ISettingsProxy _settings;

    public GlucoseResult(ISettingsProxy settings) => _settings = settings;

    public void SetDefault()
    {
        SetTrend(TrendResult.Unknown);
        SetDateTimeUtc(DateTime.UtcNow);
        SetGlucoseValues(0);
    }

    public void SetTrend(TrendResult trend) => Trend = trend;

    public void SetDateTimeUtc(DateTime dateTimeUtc) => DateTimeUTC = dateTimeUtc;

    public void SetGlucoseValues(double value)
    {
        if (value == 0)
        {
            MmolValue = 0;
            MgValue = 0;
        }
        else if (_settings.IsServerDataUnitTypeMmol)
        {
            MmolValue = value;
            MgValue = Convert.ToInt32(value * 18);
        }
        else
        {
            MmolValue = value / 18;
            MgValue = Convert.ToInt32(value);
        }
        IsCriticalLow = IsCriticalLowCalculation(_settings);
    }

    private bool IsCriticalLowCalculation(ISettingsProxy currentSettings)
    {
        if (MmolValue == 0) // Don't treat a zero / null / default result as critical low.
            return false;
        return (currentSettings.GlucoseUnit == GlucoseUnitType.MMOL && MmolValue <= currentSettings.CriticalLowBg) || (currentSettings.GlucoseUnit == GlucoseUnitType.MG && MgValue <= currentSettings.CriticalLowBg);
    }
}
