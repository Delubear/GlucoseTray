using GlucoseTray.Domain.Enums;
using System.ComponentModel;

namespace GlucoseTray.GlucoseSettings;

public class GlucoseTraySettingsViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    private void UpdateAllDataSourceFields()
    {
        OnPropertyChanged(nameof(ShowDexcomFields));
        OnPropertyChanged(nameof(ShowNightscoutFields));
        OnPropertyChanged(nameof(DataSource));
        OnPropertyChanged(nameof(IsDexcomDataSource));
        OnPropertyChanged(nameof(IsNightscoutDataSource));
        OnPropertyChanged(nameof(NightscoutUrl));
        OnPropertyChanged(nameof(DexcomServer));
        OnPropertyChanged(nameof(DexcomUsername));
    }
    private void UpdateAllUnitTypeFields()
    {
        OnPropertyChanged(nameof(UnitType));
        OnPropertyChanged(nameof(IsMgUnitType));
    }

    private DataSource dataSource;
    public DataSource DataSource
    {
        get => dataSource; set { dataSource = value; UpdateAllDataSourceFields(); }
    }
    public bool IsDexcomDataSource
    {
        get => DataSource == DataSource.DexcomShare;
        set
        {
            if (value)
                DataSource = DataSource.DexcomShare;
            UpdateAllDataSourceFields();
        }
    }
    public bool IsNightscoutDataSource
    {
        get => DataSource == DataSource.NightscoutApi;
        set
        {
            if (value)
                DataSource = DataSource.NightscoutApi;
            UpdateAllDataSourceFields();
        }
    }

    public bool ShowDexcomFields => DataSource == DataSource.DexcomShare;
    public bool ShowNightscoutFields => DataSource == DataSource.NightscoutApi;

    private GlucoseUnitType unitType;
    public GlucoseUnitType UnitType
    {
        get => unitType; set { unitType = value; OnPropertyChanged(nameof(UnitType)); }
    }
    public bool IsMgUnitType
    {
        get => UnitType == GlucoseUnitType.MG;
        set
        {
            if (value)
                UnitType = GlucoseUnitType.MG;
            UpdateAllUnitTypeFields();
        }
    }
    public bool IsMmolUnitType
    {
        get => UnitType == GlucoseUnitType.MMOL;
        set
        {
            if (value)
                UnitType = GlucoseUnitType.MMOL;
            UpdateAllUnitTypeFields();
        }
    }

    private string nightscoutUrl = string.Empty;
    public string NightscoutUrl
    {
        get => nightscoutUrl;
        set
        {
            nightscoutUrl = value;
            if (nightscoutUrl.EndsWith('/'))
                nightscoutUrl = nightscoutUrl.Remove(nightscoutUrl.Length - 1);
            UpdateAllDataSourceFields();
        }
    }

    private DexcomServerLocation dexcomServer;
    public DexcomServerLocation DexcomServer
    {
        get => dexcomServer; set { dexcomServer = value; UpdateAllDataSourceFields(); }
    }
    public DexcomServerLocation[] DexcomServerLocations => Enum.GetValues<DexcomServerLocation>();

    private string dexcomUsername = string.Empty;
    public string DexcomUsername
    {
        get => dexcomUsername;
        set
        {
            dexcomUsername = value;
            UpdateAllDataSourceFields();
        }
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

    private bool isDebugMode;
    public bool IsDebugMode
    {
        get => isDebugMode; set { isDebugMode = value; OnPropertyChanged(nameof(IsDebugMode)); }
    }

    private bool isDarkMode;
    public bool IsDarkMode
    {
        get => isDarkMode; set { isDarkMode = value; OnPropertyChanged(nameof(IsDarkMode)); }
    }
}
