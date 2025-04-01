
using GlucoseTray.Enums;

namespace GlucoseTray;

public class AppSettings
{
    public bool IsDarkMode { get; set; } = false;
    public int MinutesUntilStale { get; set; } = 15;
    public int RefreshIntervalInMinutes { get; set; } = 5;

    public string DATA_SOURCE_OPTIONS { get; private set; } = "Dexcom,Nightscout";
    public GlucoseSource DataSource { get; set; } = GlucoseSource.Dexcom;
    public string DEXCOM_SERVER_OPTIONS { get; private set; } = "DexcomShare1,DexcomShare2,DexcomInternational";
    public DexcomServer DexcomServer { get; set; } = DexcomServer.DexcomShare1;

    public string DexcomUsername { get; set; } = string.Empty;
    public string DexcomPassword { get; set; } = string.Empty;

    public string NightscoutUrl { get; set; } = string.Empty;
    public string NightscoutToken { get; set; } = string.Empty;

    public string DISPLAY_UNIT_TYPE_OPTIONS { get; private set; } = "Mg,Mmol";
    public GlucoseUnitType DisplayUnitType { get; set; }
    public string SERVER_UNIT_TYPE_OPTIONS { get; private set; } = "Mg,Mmol";
    public GlucoseUnitType ServerUnitType { get; set; }

    public int CriticalLowMgThreshold { get; set; } = 55;
    public int LowMgThreshold { get; set; } = 70;
    public int HighMgThreshold { get; set; } = 250;
    public int CriticalHighMgThreshold { get; set; } = 300;


    public float CriticalLowMmolThreshold { get; set; } = 3.0f;
    public float LowMmolThreshold { get; set; } = 3.8f;
    public float HighMmolThreshold { get; set; } = 13.8f;
    public float CriticalHighMmolThreshold { get; set; } = 16.6f;

    public bool EnableAlerts { get; set; }
}
