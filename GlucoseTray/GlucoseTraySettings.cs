using GlucoseTrayCore.Enums;
using GlucoseTrayCore.Services;
using Serilog.Events;
using System;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace GlucoseTrayCore
{
    public class GlucoseTraySettings : INotifyPropertyChanged
    {
        [JsonIgnore]
        private const string EncryptionKey = "i_can_probably_be_improved";

        private FetchMethod fetchMethod;
        public FetchMethod FetchMethod
        {
            get => fetchMethod; set { fetchMethod = value; OnPropertyChanged(nameof(FetchMethod)); }
        }

        private string nightscoutUrl;
        public string NightscoutUrl
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
        public DexcomServerLocation DexcomServer
        {
            get => dexcomServer; set { dexcomServer = value; OnPropertyChanged(nameof(DexcomServer)); }
        }

        private string dexcomUsername;
        public string DexcomUsername
        {
            get => string.IsNullOrWhiteSpace(dexcomUsername) ? dexcomUsername : StringEncryptionService.DecryptString(dexcomUsername, EncryptionKey);
            set
            {
                dexcomUsername = string.IsNullOrWhiteSpace(value) ? string.Empty : StringEncryptionService.IsEncrypted(value, EncryptionKey) ? value : StringEncryptionService.EncryptString(value, EncryptionKey);
                OnPropertyChanged(nameof(DexcomUsername));
            }
        }

        private string dexcomPassword;
        public string DexcomPassword
        {
            get => string.IsNullOrWhiteSpace(dexcomPassword) ? dexcomPassword : StringEncryptionService.DecryptString(dexcomPassword, EncryptionKey);
            set { dexcomPassword = string.IsNullOrWhiteSpace(value) ? string.Empty : StringEncryptionService.IsEncrypted(value, EncryptionKey) ? value : StringEncryptionService.EncryptString(value, EncryptionKey); OnPropertyChanged(nameof(DexcomPassword)); }
        }

        private string accessToken;
        public string AccessToken
        {
            get => string.IsNullOrWhiteSpace(accessToken) ? accessToken : StringEncryptionService.DecryptString(accessToken, EncryptionKey);
            set { accessToken = string.IsNullOrWhiteSpace(value) ? string.Empty : StringEncryptionService.IsEncrypted(value, EncryptionKey) ? value : StringEncryptionService.EncryptString(value, EncryptionKey); OnPropertyChanged(nameof(AccessToken)); }
        }

        private GlucoseUnitType glucoseUnit;
        public GlucoseUnitType GlucoseUnit
        {
            get => glucoseUnit; set { glucoseUnit = value; OnPropertyChanged(nameof(GlucoseUnit)); }
        }

        private double warningHighBg;
        public double WarningHighBg
        {
            get => warningHighBg; set { warningHighBg = value; OnPropertyChanged(nameof(WarningHighBg)); }
        }

        private double highBg;
        public double HighBg
        {
            get => highBg; set { highBg = value; OnPropertyChanged(nameof(HighBg)); }
        }

        private double warningLowBg;
        public double WarningLowBg
        {
            get => warningLowBg; set { warningLowBg = value; OnPropertyChanged(nameof(WarningLowBg)); }
        }

        private double lowBg;
        public double LowBg
        {
            get => lowBg; set { lowBg = value; OnPropertyChanged(nameof(LowBg)); }
        }

        private double criticalLowBg;
        public double CriticalLowBg
        {
            get => criticalLowBg; set { criticalLowBg = value; OnPropertyChanged(nameof(CriticalLowBg)); }
        }

        private int pollingThreshold;
        public int PollingThreshold
        {
            get => pollingThreshold; set { pollingThreshold = value; OnPropertyChanged(nameof(PollingThreshold)); }
        }

        [JsonIgnore]
        public TimeSpan PollingThresholdTimeSpan => TimeSpan.FromSeconds(PollingThreshold);

        private string databaseLocation;
        public string DatabaseLocation
        {
            get => databaseLocation; set { databaseLocation = value; OnPropertyChanged(nameof(DatabaseLocation)); }
        }

        private bool enableDebugMode;
        public bool EnableDebugMode
        {
            get => enableDebugMode; set { enableDebugMode = value; OnPropertyChanged(nameof(EnableDebugMode)); }
        }

        private LogEventLevel logLevel;
        public LogEventLevel LogLevel
        {
            get => logLevel; set { logLevel = value; OnPropertyChanged(nameof(LogLevel)); }
        }

        private int staleResultsThreshold;
        public int StaleResultsThreshold
        {
            get => staleResultsThreshold; set { staleResultsThreshold = value; OnPropertyChanged(nameof(StaleResultsThreshold)); }
        }

        private bool highAlert;
        public bool HighAlert
        {
            get => highAlert; set { highAlert = value; OnPropertyChanged(nameof(HighAlert)); }
        }

        private bool warningHighAlert;
        public bool WarningHighAlert
        {
            get => warningHighAlert; set { warningHighAlert = value; OnPropertyChanged(nameof(WarningHighAlert)); }
        }

        private bool warningLowAlert;
        public bool WarningLowAlert
        {
            get => warningLowAlert; set { warningLowAlert = value; OnPropertyChanged(nameof(WarningLowAlert)); }
        }

        private bool lowAlert;
        public bool LowAlert
        {
            get => lowAlert; set { lowAlert = value; OnPropertyChanged(nameof(LowAlert)); }
        }

        private bool criticallyLowAlert;
        public bool CriticallyLowAlert
        {
            get => criticallyLowAlert; set { criticallyLowAlert = value; OnPropertyChanged(nameof(CriticallyLowAlert)); }
        }

        private bool isServerDataUnitTypeMmol;
        public bool IsServerDataUnitTypeMmol
        {
            get => isServerDataUnitTypeMmol; set { isServerDataUnitTypeMmol = value; OnPropertyChanged(nameof(IsServerDataUnitTypeMmol)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}