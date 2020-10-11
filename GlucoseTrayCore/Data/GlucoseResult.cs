using Dexcom.Fetch.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GlucoseTrayCore.Data
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
