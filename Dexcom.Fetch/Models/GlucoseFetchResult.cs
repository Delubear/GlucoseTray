using System;

namespace Dexcom.Fetch.Models
{
    public class GlucoseFetchResult
    {
        public int Value { get; set; }
        public string TrendIcon { get; set; }
        public DateTime Time { get; set; }
    }
}
