using System.Text.Json.Serialization;

namespace GlucoseTray.Read.Nightscout;

/// <summary>
/// Class that maps to the JSON from NightScout queries.
/// </summary>
public class NightScoutResult
{
    [JsonPropertyName("sgv")]
    public float GlucoseValue { get; set; }

    [JsonPropertyName("date")]
    public long UnixTicks { get; set; }

    [JsonPropertyName("dateString")]
    public string DateString { get; set; } = string.Empty;

    [JsonPropertyName("direction")]
    public string Trend { get; set; } = string.Empty;
}
