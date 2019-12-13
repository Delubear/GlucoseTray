using Dexcom.Fetch.Enums;
using System;

namespace Dexcom.Fetch.Models
{
    public class GlucoseFetchResult
    {
        /// <summary>
        /// Glucose value
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Arrow indicating direction
        /// </summary>
        public string TrendIcon { get; set; }

        /// <summary>
        /// Time of result
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Was this result generated from an error or fallback process?
        /// </summary>
        public bool ErrorResult { get; set; }

        public GlucoseUnitType UnitDisplayType { get; set; }
    }
}