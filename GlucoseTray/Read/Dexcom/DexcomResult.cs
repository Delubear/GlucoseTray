using System.Text.Json.Serialization;

namespace GlucoseTray.Read.Dexcom;

/// <summary>
/// Class that maps to the JSON received from DexCom queries.
/// </summary>
public class DexcomResult
{
    [JsonPropertyName("ST")]
    public string UnixTicks { get; set; } = string.Empty;
    [JsonPropertyName("Trend")]
    public string Trend { get; set; } = string.Empty;
    [JsonPropertyName("Value")]
    public float GlucoseValue { get; set; }
}
