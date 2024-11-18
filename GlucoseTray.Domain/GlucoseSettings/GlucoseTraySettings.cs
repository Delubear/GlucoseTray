using GlucoseTray.Domain.Enums;
using System.Text.Json.Serialization;

namespace GlucoseTray.Domain.GlucoseSettings;

public class GlucoseTraySettings
{
    [JsonIgnore]
    private const string EncryptionKey = "i_can_probably_be_improved";

    public DataSource DataSource { get; set; }

    private string nightscoutUrl = string.Empty;
    public string NightscoutUrl
    {
        get => nightscoutUrl;
        set
        {
            nightscoutUrl = value;
            if (nightscoutUrl.EndsWith('/'))
                nightscoutUrl = nightscoutUrl.Remove(nightscoutUrl.Length - 1);
        }
    }

    public DexcomServerLocation DexcomServer { get; set; }

    private string dexcomUsername = string.Empty;
    public string DexcomUsername
    {
        get => string.IsNullOrWhiteSpace(dexcomUsername) ? dexcomUsername : StringEncryptionService.DecryptString(dexcomUsername, EncryptionKey);
        set
        {
            dexcomUsername = string.IsNullOrWhiteSpace(value) ? string.Empty : StringEncryptionService.IsEncrypted(value, EncryptionKey) ? value : StringEncryptionService.EncryptString(value, EncryptionKey);
        }
    }

    private string dexcomPassword = string.Empty;
    public string DexcomPassword
    {
        get => string.IsNullOrWhiteSpace(dexcomPassword) ? dexcomPassword : StringEncryptionService.DecryptString(dexcomPassword, EncryptionKey);
        set { dexcomPassword = string.IsNullOrWhiteSpace(value) ? string.Empty : StringEncryptionService.IsEncrypted(value, EncryptionKey) ? value : StringEncryptionService.EncryptString(value, EncryptionKey); }
    }

    private string accessToken = string.Empty;
    public string AccessToken
    {
        get => string.IsNullOrWhiteSpace(accessToken) ? accessToken : StringEncryptionService.DecryptString(accessToken, EncryptionKey);
        set { accessToken = string.IsNullOrWhiteSpace(value) ? string.Empty : StringEncryptionService.IsEncrypted(value, EncryptionKey) ? value : StringEncryptionService.EncryptString(value, EncryptionKey); }
    }

    public GlucoseUnitType GlucoseUnit { get; set; }
    public double WarningHighBg { get; set; }
    public double HighBg { get; set; }
    public double WarningLowBg { get; set; }
    public double LowBg { get; set; }
    public double CriticalLowBg { get; set; }

    public int PollingThreshold { get; set; }

    [JsonIgnore]
    public TimeSpan PollingThresholdTimeSpan => TimeSpan.FromSeconds(PollingThreshold);

    public int StaleResultsThreshold { get; set; }
    public bool HighAlert { get; set; }
    public bool WarningHighAlert { get; set; }
    public bool WarningLowAlert { get; set; }
    public bool LowAlert { get; set; }
    public bool CriticallyLowAlert { get; set; }
    public bool IsServerDataUnitTypeMmol { get; set; }
    public bool IsDebugMode { get; set; }
    public bool IsDarkMode { get; set; }
}
