using System;
using System.Collections.Generic;
using System.Text;
using Dexcom.Fetch.Enums;

namespace Dexcom.Fetch.Models
{
    public class GlucoseFetchConfiguration
    {
        public FetchMethod FetchMethod { get; set; }
        public string NightscoutUrl { get; set; }
        public string NightscoutAccessToken { get; set; }
        public string DexcomUsername { get; set; }
        public string DexcomPassword { get; set; }

    }
}
