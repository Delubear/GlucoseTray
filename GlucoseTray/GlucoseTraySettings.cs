using GlucoseTray.Enums;
using GlucoseTray.Services;
using System;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace GlucoseTray
{
    public class GlucoseTraySettings : INotifyPropertyChanged
    {
        [JsonIgnore]
        const string EncryptionKey = "i_can_probably_be_improved";

        FetchMethod fetchMethod;
        public FetchMethod FetchMethod
        {
            get => fetchMethod; set { fetchMethod = value; OnPropertyChanged(nameof(FetchMethod)); }
        }

        string nightscoutUrl;
        public string NightscoutUrl
        {
            get => nightscoutUrl;
            set
            {
                nightscoutUrl = value;
                if (nightscoutUrl.EndsWith("/")) nightscoutUrl = nightscoutUrl.Remove(nightscoutUrl.Length - 1);
                OnPropertyChanged(nameof(NightscoutUrl));
            }
        }

        DexcomServerLocation dexcomServer;
        public DexcomServerLocation DexcomServer
        {
            get => dexcomServer; set { dexcomServer = value; OnPropertyChanged(nameof(DexcomServer)); }
        }

        string dexcomUsername;
        public string DexcomUsername
        {
            get => string.IsNullOrWhiteSpace(dexcomUsername) ? dexcomUsername : StringEncryptionService.DecryptString(dexcomUsername, EncryptionKey);
            set
            {
                dexcomUsername = string.IsNullOrWhiteSpace(value) ? string.Empty : StringEncryptionService.IsEncrypted(value, EncryptionKey) ? value : StringEncryptionService.EncryptString(value, EncryptionKey);
                OnPropertyChanged(nameof(DexcomUsername));
            }
        }

        string dexcomPassword;
        public string DexcomPassword
        {
            get => string.IsNullOrWhiteSpace(dexcomPassword) ? dexcomPassword : StringEncryptionService.DecryptString(dexcomPassword, EncryptionKey);
            set { dexcomPassword = string.IsNullOrWhiteSpace(value) ? string.Empty : StringEncryptionService.IsEncrypted(value, EncryptionKey) ? value : StringEncryptionService.EncryptString(value, EncryptionKey); OnPropertyChanged(nameof(DexcomPassword)); }
        }

        string accessToken;
        public string AccessToken
        {
            get => string.IsNullOrWhiteSpace(accessToken) ? accessToken : StringEncryptionService.DecryptString(accessToken, EncryptionKey);
            set { accessToken = string.IsNullOrWhiteSpace(value) ? string.Empty : StringEncryptionService.IsEncrypted(value, EncryptionKey) ? value : StringEncryptionService.EncryptString(value, EncryptionKey); OnPropertyChanged(nameof(AccessToken)); }
        }

        GlucoseUnitType glucoseUnit;
        public GlucoseUnitType GlucoseUnit
        {
            get => glucoseUnit; set { glucoseUnit = value; OnPropertyChanged(nameof(GlucoseUnit)); }
        }

        double warningHighBg;
        public double WarningHighBg
        {
            get => warningHighBg; set { warningHighBg = value; OnPropertyChanged(nameof(WarningHighBg)); }
        }

        double highBg;
        public double HighBg
        {
            get => highBg; set { highBg = value; OnPropertyChanged(nameof(HighBg)); }
        }

        double warningLowBg;
        public double WarningLowBg
        {
            get => warningLowBg; set { warningLowBg = value; OnPropertyChanged(nameof(WarningLowBg)); }
        }

        double lowBg;
        public double LowBg
        {
            get => lowBg; set { lowBg = value; OnPropertyChanged(nameof(LowBg)); }
        }

        double criticalLowBg;
        public double CriticalLowBg
        {
            get => criticalLowBg; set { criticalLowBg = value; OnPropertyChanged(nameof(CriticalLowBg)); }
        }

        int pollingThreshold;
        public int PollingThreshold
        {
            get => pollingThreshold; set { pollingThreshold = value; OnPropertyChanged(nameof(PollingThreshold)); }
        }

        [JsonIgnore]
        public TimeSpan PollingThresholdTimeSpan => TimeSpan.FromSeconds(PollingThreshold);

        int staleResultsThreshold;
        public int StaleResultsThreshold
        {
            get => staleResultsThreshold; set { staleResultsThreshold = value; OnPropertyChanged(nameof(StaleResultsThreshold)); }
        }

        bool highAlert;
        public bool HighAlert
        {
            get => highAlert; set { highAlert = value; OnPropertyChanged(nameof(HighAlert)); }
        }

        bool warningHighAlert;
        public bool WarningHighAlert
        {
            get => warningHighAlert; set { warningHighAlert = value; OnPropertyChanged(nameof(WarningHighAlert)); }
        }

        bool warningLowAlert;
        public bool WarningLowAlert
        {
            get => warningLowAlert; set { warningLowAlert = value; OnPropertyChanged(nameof(WarningLowAlert)); }
        }

        bool lowAlert;
        public bool LowAlert
        {
            get => lowAlert; set { lowAlert = value; OnPropertyChanged(nameof(LowAlert)); }
        }

        bool criticallyLowAlert;
        public bool CriticallyLowAlert
        {
            get => criticallyLowAlert; set { criticallyLowAlert = value; OnPropertyChanged(nameof(CriticallyLowAlert)); }
        }

        bool isServerDataUnitTypeMmol;
        public bool IsServerDataUnitTypeMmol
        {
            get => isServerDataUnitTypeMmol; set { isServerDataUnitTypeMmol = value; OnPropertyChanged(nameof(IsServerDataUnitTypeMmol)); }
        }


        bool isDebugMode;
        public bool IsDebugMode
        {
            get => isDebugMode; set { isDebugMode = value; OnPropertyChanged(nameof(IsDebugMode)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class AppSettings
    {
        public string Version { get; set; }
        public string Url { get; set; }
    }

    public class AppSettingsContainer
    {
        [JsonPropertyName("appsettings")]
        public AppSettings AppSettings { get; set; }
    }
}