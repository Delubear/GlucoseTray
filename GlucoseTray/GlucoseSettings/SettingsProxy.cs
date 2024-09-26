using GlucoseTray.Domain;
using GlucoseTray.Domain.Enums;
using Microsoft.Extensions.Options;

namespace GlucoseTray.GlucoseSettings;

public class SettingsProxy(IOptionsMonitor<GlucoseTraySettings> options) : ISettingsProxy
{
    private readonly IOptionsMonitor<GlucoseTraySettings> _options = options;

    public FetchMethod FetchMethod
    {
        get => _options.CurrentValue.FetchMethod;
        set => _options.CurrentValue.FetchMethod = value;
    }

    public string NightscoutUrl
    {
        get => _options.CurrentValue.NightscoutUrl;
        set => _options.CurrentValue.NightscoutUrl = value;
    }

    public DexcomServerLocation DexcomServer
    {
        get => _options.CurrentValue.DexcomServer;
        set => _options.CurrentValue.DexcomServer = value;
    }

    public string DexcomUsername
    {
        get => _options.CurrentValue.DexcomUsername;
        set => _options.CurrentValue.DexcomUsername = value;
    }

    public string DexcomPassword
    {
        get => _options.CurrentValue.DexcomPassword;
        set => _options.CurrentValue.DexcomPassword = value;
    }

    public string AccessToken
    {
        get => _options.CurrentValue.AccessToken;
        set => _options.CurrentValue.AccessToken = value;
    }

    public GlucoseUnitType GlucoseUnit
    {
        get => _options.CurrentValue.GlucoseUnit;
        set => _options.CurrentValue.GlucoseUnit = value;
    }

    public double WarningHighBg
    {
        get => _options.CurrentValue.WarningLowBg;
        set => _options.CurrentValue.WarningLowBg = value;
    }

    public double HighBg
    {
        get => _options.CurrentValue.LowBg;
        set => _options.CurrentValue.LowBg = value;
    }

    public double WarningLowBg
    {
        get => _options.CurrentValue.CriticalLowBg;
        set => _options.CurrentValue.CriticalLowBg = value;
    }

    public double LowBg
    {
        get => _options.CurrentValue.LowBg;
        set => _options.CurrentValue.LowBg = value;
    }

    public double CriticalLowBg
    {
        get => _options.CurrentValue.CriticalLowBg;
        set => _options.CurrentValue.CriticalLowBg = value;
    }

    public int PollingThreshold
    {
        get => _options.CurrentValue.PollingThreshold;
        set => _options.CurrentValue.PollingThreshold = value;
    }

    public TimeSpan PollingThresholdTimeSpan => _options.CurrentValue.PollingThresholdTimeSpan;

    public int StaleResultsThreshold
    {
        get => _options.CurrentValue.StaleResultsThreshold;
        set => _options.CurrentValue.StaleResultsThreshold = value;
    }

    public bool HighAlert
    {
        get => _options.CurrentValue.HighAlert;
        set => _options.CurrentValue.HighAlert = value;
    }

    public bool WarningHighAlert
    {
        get => _options.CurrentValue.WarningHighAlert;
        set => _options.CurrentValue.WarningHighAlert = value;
    }

    public bool WarningLowAlert
    {
        get => _options.CurrentValue.WarningLowAlert;
        set => _options.CurrentValue.WarningLowAlert = value;
    }

    public bool LowAlert
    {
        get => _options.CurrentValue.LowAlert;
        set => _options.CurrentValue.LowAlert = value;
    }

    public bool CriticalLowAlert
    {
        get => _options.CurrentValue.CriticallyLowAlert;
        set => _options.CurrentValue.CriticallyLowAlert = value;
    }

    public bool IsServerDataUnitTypeMmol
    {
        get => _options.CurrentValue.IsServerDataUnitTypeMmol;
        set => _options.CurrentValue.IsServerDataUnitTypeMmol = value;
    }

    public bool IsDebugMode
    {
        get => _options.CurrentValue.IsDebugMode;
        set => _options.CurrentValue.IsDebugMode = value;
    }

    public bool IsDarkMode
    {
        get => _options.CurrentValue.IsDarkMode;
        set => _options.CurrentValue.IsDarkMode = value;
    }
}
