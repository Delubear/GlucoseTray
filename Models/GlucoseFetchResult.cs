using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlucoseTray.Models
{
    public class GlucoseFetchResult
    {
        public int Value { get; set; }
        public string Trend { get; set; }
        public string TrendIcon { get; set; }
        public DateTime Time { get; set; }
    }
}
