
using Microsoft.Extensions.Options;

namespace GlucoseTray.Read;

public interface IGlucoseReadingMapper
{
    GlucoseReading Map(DexcomResult result);
    GlucoseReading Map(NightScoutResult result);
}

internal class GlucoseReadingMapper(IOptionsMonitor<AppSettings> options) : IGlucoseReadingMapper
{
    public GlucoseReading Map(DexcomResult input)
    {
        var (MgValue, MmolValue) = GetValues((float)input.Value);
        var unixTime = string.Join("", input.ST.Where(char.IsDigit));

        var result = new GlucoseReading
        {
            MgValue = MgValue,
            MmolValue = MmolValue,
            Trend = input.Trend.GetTrend(),
            TimestampUtc = !string.IsNullOrWhiteSpace(unixTime) ? DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(unixTime)).UtcDateTime : DateTime.MinValue,
        };

        return result;
    }

    public GlucoseReading Map(NightScoutResult input)
    {
        var (MgValue, MmolValue) = GetValues((float)input.Sgv);

        var result = new GlucoseReading
        {
            MgValue = MgValue,
            MmolValue = MmolValue,
            Trend = input.Direction.GetTrend(),
            TimestampUtc = !string.IsNullOrEmpty(input.DateString) ? DateTime.Parse(input.DateString).ToUniversalTime() : DateTimeOffset.FromUnixTimeMilliseconds(input.Date).UtcDateTime,
        };

        return result;
    }

    private (int MgValue, float MmolValue) GetValues(float value)
    {
        if (value == 0)
            return (0, 0);
        if (options.CurrentValue.ServerUnitType == GlucoseUnitType.Mmol)
            return ((int)(value * 18.0182), value);
        else
            return ((int)value, value / 18.0182f);
    }
}
