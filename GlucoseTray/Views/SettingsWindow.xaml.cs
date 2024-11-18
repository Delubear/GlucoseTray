using System.Linq;
using System.Windows;
using GlucoseTray.Domain.DisplayResults;
using GlucoseTray.Domain.GlucoseSettings;
using GlucoseTray.GlucoseSettings;

namespace GlucoseTray.Views.Settings;

/// <summary>
/// Interaction logic for Settings.xaml
/// </summary>
public partial class SettingsWindow : Window
{
    private readonly ISettingsWindowService _service;
    private readonly IDialogService _dialogService;
    private bool HaveBypassedInitialModification;

    public SettingsWindow(ISettingsWindowService service, IDialogService dialogService)
    {
        _service = service;
        _dialogService = dialogService;

        InitializeComponent();

        var model = _service.GetSettingsFromFile() ?? _service.GetDefaultSettings();
        MapToViewModel(model);
    }

    private void MapToViewModel(GlucoseTraySettings settings)
    {
        var viewModel = GetViewModel();
        viewModel.DataSource = settings.DataSource;
        viewModel.UnitType = settings.GlucoseUnit;
        viewModel.NightscoutUrl = settings.NightscoutUrl;
        viewModel.DexcomServer = settings.DexcomServer;
        viewModel.DexcomUsername = settings.DexcomUsername;
        viewModel.WarningHighBg = settings.WarningHighBg;
        viewModel.HighBg = settings.HighBg;
        viewModel.WarningLowBg = settings.WarningLowBg;
        viewModel.LowBg = settings.LowBg;
        viewModel.CriticalLowBg = settings.CriticalLowBg;
        viewModel.PollingThreshold = settings.PollingThreshold;
        viewModel.StaleResultsThreshold = settings.StaleResultsThreshold;
        viewModel.HighAlert = settings.HighAlert;
        viewModel.WarningHighAlert = settings.WarningHighAlert;
        viewModel.WarningLowAlert = settings.WarningLowAlert;
        viewModel.LowAlert = settings.LowAlert;
        viewModel.CriticallyLowAlert = settings.CriticallyLowAlert;
        viewModel.IsServerDataUnitTypeMmol = settings.IsServerDataUnitTypeMmol;
        viewModel.IsDebugMode = settings.IsDebugMode;
        viewModel.IsDarkMode = settings.IsDarkMode;

        txt_dexcom_password.Password = settings.DexcomPassword;
        txt_nightscout_token.Password = settings.AccessToken;
    }

    private GlucoseTraySettingsViewModel GetViewModel() => Resources.Values.OfType<GlucoseTraySettingsViewModel>().First();

    private void UpdateValuesFromMMoLToMG(object sender, RoutedEventArgs e)
    {
        if (!HaveBypassedInitialModification)
        {
            HaveBypassedInitialModification = true;
            return;
        }

        var viewModel = GetViewModel();
        _service.UpdateValuesFromMMoLToMG(viewModel);
    }

    private void UpdateValuesFromMGToMMoL(object sender, RoutedEventArgs e)
    {
        if (!HaveBypassedInitialModification)
        {
            HaveBypassedInitialModification = true;
            return;
        }

        var viewModel = GetViewModel();
        _service.UpdateValuesFromMGToMMoL(viewModel);
    }

    private void Button_Save_Click(object sender, RoutedEventArgs e)
    {
        var viewModel = GetViewModel();
        var settings = MapToSettings(viewModel);
        var isValidResult = _service.IsValid(settings);
        if (isValidResult.IsValid == false)
        {
            _dialogService.ShowErrorAlert("Settings are not valid.  Please fix before continuing.", string.Join("\r\n", isValidResult.Errors));
            return;
        }

        _service.Save(settings);

        DialogResult = true;
        Close();
    }

    private void txt_dexcom_password_PasswordChanged(object sender, RoutedEventArgs e)
    {
        //var password = txt_dexcom_password.Password;
    }

    private void txt_nightscout_token_PasswordChanged(object sender, RoutedEventArgs e)
    {
        //var password = txt_nightscout_token.Password;
    }

    private GlucoseTraySettings MapToSettings(GlucoseTraySettingsViewModel viewModel)
    {
        var settings = new GlucoseTraySettings
        {
            DataSource = viewModel.DataSource,
            GlucoseUnit = viewModel.UnitType,
            NightscoutUrl = viewModel.NightscoutUrl,
            DexcomServer = viewModel.DexcomServer,
            DexcomUsername = viewModel.DexcomUsername,
            WarningHighBg = viewModel.WarningHighBg,
            HighBg = viewModel.HighBg,
            WarningLowBg = viewModel.WarningLowBg,
            LowBg = viewModel.LowBg,
            CriticalLowBg = viewModel.CriticalLowBg,
            PollingThreshold = viewModel.PollingThreshold,
            StaleResultsThreshold = viewModel.StaleResultsThreshold,
            HighAlert = viewModel.HighAlert,
            WarningHighAlert = viewModel.WarningHighAlert,
            WarningLowAlert = viewModel.WarningLowAlert,
            LowAlert = viewModel.LowAlert,
            CriticallyLowAlert = viewModel.CriticallyLowAlert,
            IsServerDataUnitTypeMmol = viewModel.IsServerDataUnitTypeMmol,
            IsDebugMode = viewModel.IsDebugMode,
            IsDarkMode = viewModel.IsDarkMode,
            DexcomPassword = txt_dexcom_password.Password,
            AccessToken = txt_nightscout_token.Password,
        };
        return settings;
    }
}
