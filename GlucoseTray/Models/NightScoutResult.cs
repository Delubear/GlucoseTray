using System.Text.Json.Serialization;

namespace GlucoseTray.Models;

/// <summary>
/// Class that maps to the JSON from NightScout queries.
/// </summary>
public class NightScoutResult
{
    [JsonPropertyName("_id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("sgv")]
    public double Sgv { get; set; }

    [JsonPropertyName("date")]
    public long Date { get; set; }

    [JsonPropertyName("dateString")]
    public string DateString { get; set; } = string.Empty;

    [JsonPropertyName("direction")]
    public string Direction { get; set; } = string.Empty;

    [JsonPropertyName("device")]
    public string Device { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("utcOffset")]
    public long UtcOffset { get; set; }

    [JsonPropertyName("sysTime")]
    public string SystemTime { get; set; } = string.Empty;
}
