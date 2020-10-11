using Dexcom.Fetch.Enums;
using System;

namespace Dexcom.Fetch.Models
{
    public class GlucoseFetchResult
    {
        /// <summary>
        /// MMOL Glucose value
        /// </summary>
        public double MmolValue { get; set; }

        /// <summary>
        /// MG Glucose value
        /// </summary>
        public int MgValue { get; set; }

        /// <summary>
        /// Time of result
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Was this result generated from an error or fallback process?
        /// </summary>
        public bool ErrorResult { get; set; }

        /// <summary>
        /// Where result was received from
        /// </summary>
        public FetchMethod Source { get; set; }

        /// <summary>
        /// The current BG trend
        /// </summary>
        public TrendResult Trend { get; set; }
    }
}