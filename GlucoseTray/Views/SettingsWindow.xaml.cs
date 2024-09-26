using System.Windows;
using GlucoseTray.Domain.Enums;
using GlucoseTray.GlucoseSettings;

namespace GlucoseTray.Views.Settings;

/// <summary>
/// Interaction logic for Settings.xaml
/// </summary>
public partial class SettingsWindow : Window
{
    private readonly ISettingsWindowService _service;
    private GlucoseTraySettings _settings;
    private bool HaveBypassedInitialModification;

    public SettingsWindow(ISettingsWindowService service)
    {
        _service = service;
        _settings = _service.GetDefaultSettings();

        InitializeComponent();

        combobox_dexcom_server.ItemsSource = _service.GetDexComServerLocationDescriptions();

        var model = _service.GetSettingsFromFile();
        if (model is not null)
        {
            txt_dexcom_password.Password = model.DexcomPassword;
            txt_nightscout_token.Password = model.AccessToken;
            if (model.FetchMethod == FetchMethod.DexcomShare)
                radio_source_dexcom.IsChecked = true;
            else
                radio_source_nightscout.IsChecked = true;

            if (model.GlucoseUnit == GlucoseUnitType.MG)
                radio_unit_mg.IsChecked = true;
            else
                radio_unit_mmol.IsChecked = true;

            combobox_dexcom_server.SelectedIndex = (int)model.DexcomServer;
            _settings = model;
        }

        DataContext = _settings;
    }

    private void UpdateValuesFromMMoLToMG(object sender, RoutedEventArgs e)
    {
        if (!HaveBypassedInitialModification)
        {
            HaveBypassedInitialModification = true;
            return;
        }

        _service.UpdateValuesFromMMoLToMG(_settings);
    }

    private void UpdateValuesFromMGToMMoL(object sender, RoutedEventArgs e)
    {
        if (!HaveBypassedInitialModification)
        {
            HaveBypassedInitialModification = true;
            return;
        }

        _service.UpdateValuesFromMGToMMoL(_settings);
    }

    private void ShowNightscoutBlock(object sender, RoutedEventArgs e)
    {
        if (label_dexcom_username == null)
            return;
        label_dexcom_username.Visibility = Visibility.Hidden;
        label_dexcom_password.Visibility = Visibility.Hidden;
        txt_dexcom_password.Visibility = Visibility.Hidden;
        txt_dexcom_username.Visibility = Visibility.Hidden;
        label_dexcom_server.Visibility = Visibility.Hidden;
        combobox_dexcom_server.Visibility = Visibility.Hidden;

        label_nightscoutUrl.Visibility = Visibility.Visible;
        label_nightscout_token.Visibility = Visibility.Visible;
        txt_nightscout_token.Visibility = Visibility.Visible;
        txt_nightscoutUrl.Visibility = Visibility.Visible;
    }

    private void ShowDexcomBlock(object sender, RoutedEventArgs e)
    {
        if (label_nightscoutUrl == null)
            return;
        label_nightscoutUrl.Visibility = Visibility.Hidden;
        label_nightscout_token.Visibility = Visibility.Hidden;
        txt_nightscout_token.Visibility = Visibility.Hidden;
        txt_nightscoutUrl.Visibility = Visibility.Hidden;

        label_dexcom_username.Visibility = Visibility.Visible;
        label_dexcom_password.Visibility = Visibility.Visible;
        txt_dexcom_password.Visibility = Visibility.Visible;
        txt_dexcom_username.Visibility = Visibility.Visible;
        label_dexcom_server.Visibility = Visibility.Visible;
        combobox_dexcom_server.Visibility = Visibility.Visible;
    }

    private void Button_Save_Click(object sender, RoutedEventArgs e)
    {
        var fetchMethod = radio_source_dexcom.IsChecked == true ? FetchMethod.DexcomShare : FetchMethod.NightscoutApi;
        var glucoseUnit = radio_unit_mg.IsChecked == true ? GlucoseUnitType.MG : GlucoseUnitType.MMOL;
        var dexcomServer = (DexcomServerLocation)combobox_dexcom_server.SelectedIndex;
        var dexcomUsername = txt_dexcom_username.Text;
        var dexcomPassword = txt_dexcom_password.Password;
        var nightscoutAccessToken = txt_nightscout_token.Password;
        _service.UpdateServerDetails(_settings, fetchMethod, glucoseUnit, dexcomServer, dexcomUsername, dexcomPassword, nightscoutAccessToken);
        var isValidResult = _service.IsValid(_settings);
        if (isValidResult.IsValid == false)
        {
            MessageBox.Show("Settings are not valid.  Please fix before continuing.\r\n\r\n" + string.Join("\r\n", isValidResult.Errors));
            return;
        }

        _service.Save(_settings);

        DialogResult = true;
        Close();
    }
}
