using GlucoseTray.Enums;
using GlucoseTray.Read.Dexcom;
using GlucoseTray.Read.Nightscout;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace GlucoseTray.Read;

public interface IGlucoseReadingMapper
{
    GlucoseReading Map(DexcomResult result);
    GlucoseReading Map(NightScoutResult result);
}

public class GlucoseReadingMapper(IOptionsMonitor<AppSettings> options) : IGlucoseReadingMapper
{
    public GlucoseReading Map(DexcomResult input)
    {
        var (MgValue, MmolValue) = GetValues((float)input.GlucoseValue);
        var unixTime = string.Join("", input.UnixTicks.Where(char.IsDigit));

        var result = new GlucoseReading
        {
            MgValue = MgValue,
            MmolValue = MmolValue,
            Trend = input.Trend.GetTrend(),
            TimestampUtc = !string.IsNullOrWhiteSpace(unixTime) ? DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(unixTime, CultureInfo.InvariantCulture)).UtcDateTime : DateTime.MinValue,
        };

        return result;
    }

    public GlucoseReading Map(NightScoutResult input)
    {
        var (MgValue, MmolValue) = GetValues((float)input.GlucoseValue);

        var result = new GlucoseReading
        {
            MgValue = MgValue,
            MmolValue = MmolValue,
            Trend = input.Trend.GetTrend(),
            TimestampUtc = GetNightscoutTimestamp(input),
        };

        return result;
    }

    private (int MgValue, float MmolValue) GetValues(float value)
    {
        if (value == 0)
            return (0, 0);
        if (options.CurrentValue.ServerUnitType == GlucoseUnitType.Mmol)
            return ((int)(value * 18.0182f), value);
        else
            return ((int)value, value / 18.0182f);
    }

    private static DateTime GetNightscoutTimestamp(NightScoutResult input)
    {
        if (!string.IsNullOrEmpty(input.DateString))
            return DateTime.Parse(input.DateString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind).ToUniversalTime();

        return input.UnixTicks > 0 ? DateTimeOffset.FromUnixTimeMilliseconds(input.UnixTicks).UtcDateTime : DateTime.MinValue;
    }
}
