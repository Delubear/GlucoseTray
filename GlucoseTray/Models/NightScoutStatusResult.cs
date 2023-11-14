using System.Text.Json.Serialization;

namespace GlucoseTray.Models;

internal class NightScoutStatusResult
{
    [JsonPropertyName("settings")]
    internal NightScoutSettingsResult Settings { get; set; } = new();
}

internal class NightScoutSettingsResult
{
    [JsonPropertyName("units")]
    internal string Units { get; set; } = string.Empty;

    [JsonIgnore]
    internal GlucoseUnitType UnitType => Units == "mg/dl" ? GlucoseUnitType.MG : GlucoseUnitType.MMOL;
}
