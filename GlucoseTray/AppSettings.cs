
using GlucoseTray.Enums;
using System.Text.Json.Serialization;

namespace GlucoseTray;

public class AppSettings
{
    [JsonPropertyOrder(1)]
    public bool IsDarkMode { get; set; } = false;
    [JsonPropertyOrder(2)]
    public int MinutesUntilStale { get; set; } = 15;
    [JsonPropertyOrder(3)]
    public int RefreshIntervalInMinutes { get; set; } = 5;

    [JsonPropertyOrder(4)]
    public string DATA_SOURCE_OPTIONS { get; private set; } = "Dexcom,Nightscout";
    [JsonPropertyOrder(5)]
    public GlucoseSource DataSource { get; set; } = GlucoseSource.Dexcom;
    [JsonPropertyOrder(6)]
    public string DEXCOM_SERVER_OPTIONS { get; private set; } = "DexcomShare1,DexcomShare2,DexcomInternational";
    [JsonPropertyOrder(7)]
    public DexcomServer DexcomServer { get; set; } = DexcomServer.DexcomShare1;

    [JsonPropertyOrder(8)]
    public string DexcomUsername { get; set; } = string.Empty;
    [JsonPropertyOrder(9)]
    public string DexcomPassword { get; set; } = string.Empty;

    [JsonPropertyOrder(10)]
    public string NightscoutUrl { get; set; } = string.Empty;
    [JsonPropertyOrder(11)]
    public string NightscoutToken { get; set; } = string.Empty;

    [JsonPropertyOrder(12)]
    public string DISPLAY_UNIT_TYPE_OPTIONS { get; private set; } = "Mg,Mmol";
    [JsonPropertyOrder(13)]
    public GlucoseUnitType DisplayUnitType { get; set; }
    [JsonPropertyOrder(14)]
    public string SERVER_UNIT_TYPE_OPTIONS { get; private set; } = "Mg,Mmol";
    [JsonPropertyOrder(15)]
    public GlucoseUnitType ServerUnitType { get; set; }

    [JsonPropertyOrder(16)]
    public int CriticalLowMgThreshold { get; set; } = 55;
    [JsonPropertyOrder(17)]
    public int LowMgThreshold { get; set; } = 70;
    [JsonPropertyOrder(18)]
    public int HighMgThreshold { get; set; } = 250;
    [JsonPropertyOrder(19)]
    public int CriticalHighMgThreshold { get; set; } = 300;


    [JsonPropertyOrder(20)]
    public float CriticalLowMmolThreshold { get; set; } = 3.0f;
    [JsonPropertyOrder(21)]
    public float LowMmolThreshold { get; set; } = 3.8f;
    [JsonPropertyOrder(22)]
    public float HighMmolThreshold { get; set; } = 13.8f;
    [JsonPropertyOrder(23)]
    public float CriticalHighMmolThreshold { get; set; } = 16.6f;

    [JsonPropertyOrder(24)]
    public bool EnableAlerts { get; set; }
}
