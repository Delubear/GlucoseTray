using System;

namespace Dexcom.Fetch.Models
{
    public class GlucoseFetchResult
    {
        /// <summary>
        /// Glucose value
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Arrow indicating direction
        /// </summary>
        public string TrendIcon { get; set; }

        /// <summary>
        /// Time of result
        /// </summary>
        public DateTime Time { get; set; }
    }
}
