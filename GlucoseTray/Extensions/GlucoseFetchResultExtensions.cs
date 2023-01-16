using GlucoseTray.Enums;
using GlucoseTray.Models;

namespace GlucoseTray.Extensions
{
    public static class GlucoseFetchResultExtensions
    {
        public static string GetFormattedStringValue(this GlucoseResult fetchResult, GlucoseUnitType type) => type == GlucoseUnitType.MG ? fetchResult.MgValue.ToString() : fetchResult.MmolValue.ToString("0.0");
        public static bool IsStale(this GlucoseResult fetchResult, int minutes) => (System.DateTime.Now.ToUniversalTime() - fetchResult.DateTimeUTC).TotalMinutes > minutes;

        public static string StaleMessage(this GlucoseResult fetchResult, int minutes)
        {
            System.TimeSpan ts = System.DateTime.Now.ToUniversalTime() - fetchResult.DateTimeUTC;
            return ts.TotalMinutes > minutes ? $"\r\n{ts.TotalMinutes:#} minutes ago" : string.Empty;
        }
    }
}
