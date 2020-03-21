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
    }
}
