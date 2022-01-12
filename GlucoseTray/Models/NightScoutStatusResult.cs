using GlucoseTrayCore.Enums;
using System.Text.Json.Serialization;

namespace GlucoseTrayCore.Models
{
    public class NightScoutStatusResult
    {
        public NightScoutSettingsResult settings { get; set; }
    }

    public class NightScoutSettingsResult
    {
        public string units { get; set; }

        [JsonIgnore]
        public GlucoseUnitType UnitType => units == "mg/dl" ? GlucoseUnitType.MG : GlucoseUnitType.MMOL;
    }
}
