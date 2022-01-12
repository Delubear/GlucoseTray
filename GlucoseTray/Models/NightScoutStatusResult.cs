using GlucoseTrayCore.Enums;
using System.Text.Json.Serialization;

namespace GlucoseTrayCore.Models
{
    public class NightScoutStatusResult
    {
        [JsonPropertyName("settings")]
        public NightScoutSettingsResult Settings { get; set; }
    }

    public class NightScoutSettingsResult
    {
        [JsonPropertyName("units")]
        public string Units { get; set; }

        [JsonIgnore]
        public GlucoseUnitType UnitType => Units == "mg/dl" ? GlucoseUnitType.MG : GlucoseUnitType.MMOL;
    }
}
