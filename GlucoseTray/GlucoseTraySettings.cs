using System.ComponentModel;
using System.Text.Json.Serialization;

namespace GlucoseTray;

public class GlucoseTraySettings : INotifyPropertyChanged
{
    [JsonIgnore]
    private const string EncryptionKey = "i_can_probably_be_improved";

    private FetchMethod fetchMethod;
    internal FetchMethod FetchMethod
    {
        get => fetchMethod; set { fetchMethod = value; OnPropertyChanged(nameof(FetchMethod)); }
    }

    private string nightscoutUrl = string.Empty;
    internal string NightscoutUrl
    {
        get => nightscoutUrl;
        set
        {
            nightscoutUrl = value;
            if (nightscoutUrl.EndsWith("/"))
                nightscoutUrl = nightscoutUrl.Remove(nightscoutUrl.Length - 1);
            OnPropertyChanged(nameof(NightscoutUrl));
        }
    }

    private DexcomServerLocation dexcomServer;
    internal DexcomServerLocation DexcomServer
    {
        get => dexcomServer; set { dexcomServer = value; OnPropertyChanged(nameof(DexcomServer)); }
    }

    private string dexcomUsername = string.Empty;
    internal string DexcomUsername
    {
        get => string.IsNullOrWhiteSpace(dexcomUsername) ? dexcomUsername : StringEncryptionService.DecryptString(dexcomUsername, EncryptionKey);
        set
        {
            dexcomUsername = string.IsNullOrWhiteSpace(value) ? string.Empty : StringEncryptionService.IsEncrypted(value, EncryptionKey) ? value : StringEncryptionService.EncryptString(value, EncryptionKey);
            OnPropertyChanged(nameof(DexcomUsername));
        }
    }

    private string dexcomPassword = string.Empty;
    internal string DexcomPassword
    {
        get => string.IsNullOrWhiteSpace(dexcomPassword) ? dexcomPassword : StringEncryptionService.DecryptString(dexcomPassword, EncryptionKey);
        set { dexcomPassword = string.IsNullOrWhiteSpace(value) ? string.Empty : StringEncryptionService.IsEncrypted(value, EncryptionKey) ? value : StringEncryptionService.EncryptString(value, EncryptionKey); OnPropertyChanged(nameof(DexcomPassword)); }
    }

    private string accessToken = string.Empty;
    internal string AccessToken
    {
        get => string.IsNullOrWhiteSpace(accessToken) ? accessToken : StringEncryptionService.DecryptString(accessToken, EncryptionKey);
        set { accessToken = string.IsNullOrWhiteSpace(value) ? string.Empty : StringEncryptionService.IsEncrypted(value, EncryptionKey) ? value : StringEncryptionService.EncryptString(value, EncryptionKey); OnPropertyChanged(nameof(AccessToken)); }
    }

    private GlucoseUnitType glucoseUnit;
    internal GlucoseUnitType GlucoseUnit
    {
        get => glucoseUnit; set { glucoseUnit = value; OnPropertyChanged(nameof(GlucoseUnit)); }
    }

    private double warningHighBg;
    internal double WarningHighBg
    {
        get => warningHighBg; set { warningHighBg = value; OnPropertyChanged(nameof(WarningHighBg)); }
    }

    private double highBg;
    internal double HighBg
    {
        get => highBg; set { highBg = value; OnPropertyChanged(nameof(HighBg)); }
    }

    private double warningLowBg;
    internal double WarningLowBg
    {
        get => warningLowBg; set { warningLowBg = value; OnPropertyChanged(nameof(WarningLowBg)); }
    }

    private double lowBg;
    internal double LowBg
    {
        get => lowBg; set { lowBg = value; OnPropertyChanged(nameof(LowBg)); }
    }

    private double criticalLowBg;
    internal double CriticalLowBg
    {
        get => criticalLowBg; set { criticalLowBg = value; OnPropertyChanged(nameof(CriticalLowBg)); }
    }

    private int pollingThreshold;
    internal int PollingThreshold
    {
        get => pollingThreshold; set { pollingThreshold = value; OnPropertyChanged(nameof(PollingThreshold)); }
    }

    [JsonIgnore]
    internal TimeSpan PollingThresholdTimeSpan => TimeSpan.FromSeconds(PollingThreshold);

    private int staleResultsThreshold;
    internal int StaleResultsThreshold
    {
        get => staleResultsThreshold; set { staleResultsThreshold = value; OnPropertyChanged(nameof(StaleResultsThreshold)); }
    }

    private bool highAlert;
    internal bool HighAlert
    {
        get => highAlert; set { highAlert = value; OnPropertyChanged(nameof(HighAlert)); }
    }

    private bool warningHighAlert;
    public bool WarningHighAlert
    {
        get => warningHighAlert; set { warningHighAlert = value; OnPropertyChanged(nameof(WarningHighAlert)); }
    }

    private bool warningLowAlert;
    internal bool WarningLowAlert
    {
        get => warningLowAlert; set { warningLowAlert = value; OnPropertyChanged(nameof(WarningLowAlert)); }
    }

    private bool lowAlert;
    internal bool LowAlert
    {
        get => lowAlert; set { lowAlert = value; OnPropertyChanged(nameof(LowAlert)); }
    }

    private bool criticallyLowAlert;
    internal bool CriticallyLowAlert
    {
        get => criticallyLowAlert; set { criticallyLowAlert = value; OnPropertyChanged(nameof(CriticallyLowAlert)); }
    }

    private bool isServerDataUnitTypeMmol;
    internal bool IsServerDataUnitTypeMmol
    {
        get => isServerDataUnitTypeMmol; set { isServerDataUnitTypeMmol = value; OnPropertyChanged(nameof(IsServerDataUnitTypeMmol)); }
    }


    private bool isDebugMode;
    internal bool IsDebugMode
    {
        get => isDebugMode; set { isDebugMode = value; OnPropertyChanged(nameof(IsDebugMode)); }
    }

    private bool isDarkMode;
    internal bool IsDarkMode
    {
        get => isDarkMode; set { isDarkMode = value; OnPropertyChanged(nameof(IsDarkMode)); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class AppSettings
{
    internal string Version { get; set; } = string.Empty;
    internal string Url { get; set; } = string.Empty;
}

public class AppSettingsContainer
{
    [JsonPropertyName("appsettings")]
    internal AppSettings AppSettings { get; set; } = new AppSettings();
}