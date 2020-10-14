using GlucoseTrayCore.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace GlucoseTrayCore.Models
{
    public class GlucoseResult
    {
        [Key]
        public int Id { get; set; }

        public int MgValue { get; set; }

        public double MmolValue { get; set; }

        public DateTime DateTimeUTC { get; set; }

        public TrendResult Trend { get; set; }

        public bool WasError { get; set; }

        public FetchMethod Source { get; set; }
    }
}
