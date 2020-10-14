using Dexcom.Fetch.Enums;
using Dexcom.Fetch.Models;

namespace Dexcom.Fetch.Extensions
{
    public static class GlucoseFetchResultExtensions
    {
        public static string GetFormattedStringValue(this GlucoseFetchResult fetchResult, GlucoseUnitType type) => type == GlucoseUnitType.MG ? fetchResult.MgValue.ToString() : fetchResult.MmolValue.ToString("0.0");

        public static bool IsStale(this GlucoseFetchResult fetchResult, int minutes)
        {
            var ts = System.DateTime.Now - fetchResult.Time;
            return ts.TotalMinutes > minutes;
        }

        public static string StaleMessage(this GlucoseFetchResult fetchResult, int minutes)
        {
            var ts = System.DateTime.Now - fetchResult.Time;
            return ts.TotalMinutes > minutes ? $"\r\n{ts.TotalMinutes:#} minutes ago" : "";
        }
    }
}
