using GlucoseTrayCore.Enums;
using Serilog.Events;
using System;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace GlucoseTrayCore
{
    public class GlucoseTraySettings : INotifyPropertyChanged
    {
        private FetchMethod fetchMethod;
        public FetchMethod FetchMethod
        {
            get => fetchMethod; set { fetchMethod = value; OnPropertyChanged(nameof(FetchMethod)); }
        }

        private string nightscoutUrl;
        public string NightscoutUrl
        {
            get => nightscoutUrl; set { nightscoutUrl = value; OnPropertyChanged(nameof(NightscoutUrl)); }
        }

        private DexcomServerLocation dexcomServer;
        public DexcomServerLocation DexcomServer
        {
            get => dexcomServer; set { dexcomServer = value; OnPropertyChanged(nameof(DexcomServer)); }
        }

        private string dexcomUsername;
        public string DexcomUsername
        {
            get => dexcomUsername; set { dexcomUsername = value; OnPropertyChanged(nameof(DexcomUsername)); }
        }

        private string dexcomPassword;
        public string DexcomPassword
        {
            get => dexcomPassword; set { dexcomPassword = value; OnPropertyChanged(nameof(DexcomPassword)); }
        }

        private string accessToken;
        public string AccessToken
        {
            get => accessToken; set { accessToken = value; OnPropertyChanged(nameof(AccessToken)); }
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}