using System.Text.Json.Serialization;

namespace GlucoseTray.Models;

/// <summary>
/// Class that maps to the JSON from NightScout status.
/// </summary>
/// <remarks>
/// Currently only maps the status value.
/// 
/// Would it be possible to read the units and alarm thresholds from nightscout?
/// </remarks>
public class NightScoutStatus
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}
