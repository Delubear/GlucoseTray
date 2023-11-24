using System.Text.Json.Serialization;

namespace GlucoseTray.Models;

public class NightScoutStatusResult
{
    [JsonPropertyName("settings")]
    public NightScoutSettingsResult Settings { get; set; } = new();
}

public class NightScoutSettingsResult
{
    [JsonPropertyName("units")]
    public string Units { get; set; } = string.Empty;

    [JsonIgnore]
    public GlucoseUnitType UnitType => Units == "mg/dl" ? GlucoseUnitType.MG : GlucoseUnitType.MMOL;
}
