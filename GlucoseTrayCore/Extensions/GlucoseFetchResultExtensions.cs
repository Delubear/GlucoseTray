using Dexcom.Fetch.Enums;
using Dexcom.Fetch.Models;
using GlucoseTrayCore.Data;

namespace Dexcom.Fetch.Extensions
{
    public static class GlucoseFetchResultExtensions
    {
        public static string GetFormattedStringValue(this GlucoseResult fetchResult, GlucoseUnitType type) => type == GlucoseUnitType.MG ? fetchResult.MgValue.ToString() : fetchResult.MmolValue.ToString("0.0");

        public static bool IsStale(this GlucoseResult fetchResult, int minutes)
        {
            var ts = System.DateTime.Now.ToUniversalTime() - fetchResult.DateTimeUTC;
            return ts.TotalMinutes > minutes;
        }

        public static string StaleMessage(this GlucoseResult fetchResult, int minutes)
        {
            var ts = System.DateTime.Now.ToUniversalTime() - fetchResult.DateTimeUTC;
            return ts.TotalMinutes > minutes ? $"\r\n{ts.TotalMinutes:#} minutes ago" : "";
        }
    }
}
