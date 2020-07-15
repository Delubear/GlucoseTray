using Dexcom.Fetch.Models;

namespace Dexcom.Fetch.Extensions
{
    public static class GlucoseFetchResultExtensions
    {
        public static string GetFormattedStringValue(this GlucoseFetchResult fetchResult)
        {
            var value = fetchResult.Value.ToString();
            if (value.Contains("."))
                value = fetchResult.Value.ToString("0.0");
            return value;
        }

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
