namespace GlucoseTray.Extensions;

internal static class GlucoseFetchResultExtensions
{
    internal static string GetFormattedStringValue(this GlucoseResult fetchResult, GlucoseUnitType type) => type == GlucoseUnitType.MG ? fetchResult.MgValue.ToString() : fetchResult.MmolValue.ToString("0.0");

    internal static bool IsStale(this GlucoseResult fetchResult, int minutes) => (DateTime.Now.ToUniversalTime() - fetchResult.DateTimeUTC).TotalMinutes > minutes;

    internal static string StaleMessage(this GlucoseResult fetchResult, int minutes)
    {
        var ts = DateTime.Now.ToUniversalTime() - fetchResult.DateTimeUTC;
        return ts.TotalMinutes > minutes ? $"\r\n{ts.TotalMinutes:#} minutes ago" : string.Empty;
    }
}
