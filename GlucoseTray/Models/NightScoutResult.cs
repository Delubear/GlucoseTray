using System.Text.Json.Serialization;

namespace GlucoseTray.Models;

/// <summary>
/// Class that maps to the JSON from NightScout queries.
/// </summary>
internal class NightScoutResult
{
    [JsonPropertyName("_id")]
    internal string Id { get; set; } = string.Empty;

    [JsonPropertyName("sgv")]
    internal double Sgv { get; set; }

    [JsonPropertyName("date")]
    internal long Date { get; set; }

    [JsonPropertyName("dateString")]
    internal string DateString { get; set; } = string.Empty;

    [JsonPropertyName("direction")]
    internal string Direction { get; set; } = string.Empty;

    [JsonPropertyName("device")]
    internal string Device { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    internal string Type { get; set; } = string.Empty;

    [JsonPropertyName("utcOffset")]
    internal long UtcOffset { get; set; }

    [JsonPropertyName("sysTime")]
    internal string SystemTime { get; set; } = string.Empty;
}
