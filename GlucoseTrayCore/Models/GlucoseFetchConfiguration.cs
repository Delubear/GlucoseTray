using Dexcom.Fetch.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dexcom.Fetch.Models
{
    public class GlucoseFetchConfiguration
    {
        /// <summary>
        /// Either getting data directly from Dexcom or by using Nightscout
        /// </summary>
        public FetchMethod FetchMethod { get; set; }

        /// <summary>
        /// Base URL of Herokuapp site.  i.e. https://accountname.herokuapp.com
        /// </summary>
        public string NightscoutUrl { get; set; }

        /// <summary>
        /// If using AUTH_DEFAULT_ROLES = denied in Nightscout configuration, create an access key with at least 'readable' role and enter the token here. Otherwise, leave blank.
        /// http://www.nightscout.info/wiki/welcome/website-features/0-9-features/authentication-roles
        /// </summary>
        public string NightscoutAccessToken { get; set; }

        /// <summary>
        /// Username for Dexcom if using Dexcom method
        /// </summary>
        public string DexcomUsername { get; set; }

        /// <summary>
        /// Password for Dexcom if using Dexcom method
        /// </summary>
        public string DexcomPassword { get; set; }

        public DexcomServerLocation DexcomServer { get; set; }

        /// <summary>
        /// MG/DL or MMOL/L
        /// </summary>
        public GlucoseUnitType UnitDisplayType { get; set; }
    }
}