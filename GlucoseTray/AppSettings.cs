
namespace GlucoseTray;

public class AppSettings
{
    public bool IsDarkMode { get; set; } = false;
    public int MinutesUntilStale { get; set; } = 15;

    public GlucoseUnitType DisplayUnitType { get; set; }
    public GlucoseUnitType ServerUnitType { get; set; }

    public int CriticalLowMgThreshold { get; set; } = 55;
    public int LowMgThreshold { get; set; } = 70;
    public int HighMgThreshold { get; set; } = 250;
    public int CriticalHighMgThreshold { get; set; } = 300;


    public float CriticalLowMmolThreshold { get; set; } = 3.0f;
    public float LowMmolThreshold { get; set; } = 3.8f;
    public float HighMmolThreshold { get; set; } = 13.8f;
    public float CriticalHighMmolThreshold { get; set; } = 16.6f;
}

public enum GlucoseUnitType
{
    Mg,
    Mmol
}
