using GlucoseTray.Domain.Enums;

namespace GlucoseTray.Domain.GlucoseSettings
{
    public interface ISettingsProxy
    {
        string AccessToken { get; set; }
        bool CriticalLowAlert { get; set; }
        double CriticalLowBg { get; set; }
        string DexcomPassword { get; set; }
        DexcomServerLocation DexcomServer { get; set; }
        string DexcomUsername { get; set; }
        DataSource FetchMethod { get; set; }
        GlucoseUnitType GlucoseUnit { get; set; }
        bool HighAlert { get; set; }
        double HighBg { get; set; }
        bool IsDarkMode { get; set; }
        bool IsDebugMode { get; set; }
        bool IsServerDataUnitTypeMmol { get; set; }
        bool LowAlert { get; set; }
        double LowBg { get; set; }
        string NightscoutUrl { get; set; }
        int PollingThreshold { get; set; }
        TimeSpan PollingThresholdTimeSpan { get; }
        int StaleResultsThreshold { get; set; }
        bool WarningHighAlert { get; set; }
        double WarningHighBg { get; set; }
        bool WarningLowAlert { get; set; }
        double WarningLowBg { get; set; }
    }
}