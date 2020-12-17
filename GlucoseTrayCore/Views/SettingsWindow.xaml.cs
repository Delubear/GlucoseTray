using GlucoseTrayCore.Enums;
using GlucoseTrayCore.Services;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GlucoseTrayCore.Views.Settings
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public GlucoseTraySettings Settings { get; set; } = new GlucoseTraySettings
        {
            HighBg = 250,
            WarningHighBg = 200,
            WarningLowBg = 80,
            LowBg = 70,
            CriticalLowBg = 55,
            EnableDebugMode = false,
            PollingThreshold = 15,
            StaleResultsThreshold = 15,
            DatabaseLocation = @"C:\Temp\glucosetray.db",
            DexcomUsername = "",
            NightscoutUrl = "",
            // Appears we will need to manually bind radios, dropdowns, and password fields for now.
        };

        public SettingsWindow()
        {
            InitializeComponent();

            combobox_loglevel.ItemsSource = Enum.GetNames<LogEventLevel>();
            combobox_dexcom_server.ItemsSource = typeof(DexcomServerLocation).GetFields().Select(x => (DescriptionAttribute[])x.GetCustomAttributes(typeof(DescriptionAttribute), false)).SelectMany(x => x).Select(x => x.Description);

            if (File.Exists(Program.SettingsFile))
            {
                try
                {
                    var model = FileService<GlucoseTraySettings>.ReadModelFromFile(Program.SettingsFile);

                    if (model is null)
                    {
                        MessageBox.Show("Unable to load existing settings due to a bad file.");
                    }
                    else
                    {
                        model.DexcomUsername = StringEncryptionService.DecryptString(model.DexcomUsername, "i_can_probably_be_improved");
                        model.DexcomPassword = StringEncryptionService.DecryptString(model.DexcomPassword, "i_can_probably_be_improved");
                        txt_dexcom_password.Password = model.DexcomPassword;
                        model.AccessToken = StringEncryptionService.DecryptString(model.AccessToken, "i_can_probably_be_improved");
                        txt_nightscout_token.Password = model.AccessToken;
                        if (model.FetchMethod == FetchMethod.DexcomShare)
                            radio_source_dexcom.IsChecked = true;
                        else
                            radio_source_nightscout.IsChecked = true;

                        combobox_loglevel.SelectedIndex = (int)model.LogLevel;
                        combobox_dexcom_server.SelectedIndex = (int)model.DexcomServer;

                        Settings = model;
                    }
                }
                catch (Exception e) // Catch serialization errors due to a bad file
                {
                    MessageBox.Show("Unable to load existing settings due to a bad file.  " + e.Message + e.InnerException?.Message);
                }
            }

            DataContext = Settings;
        }

        private bool HaveBypassedInitialModification = false;
        private void UpdateValuesFromMMoLToMG(object sender, RoutedEventArgs e)
        {
            if (!HaveBypassedInitialModification)
            {
                HaveBypassedInitialModification = true;
                return;
            }
            Settings.HighBg = Math.Round(Settings.HighBg *= 18);
            Settings.WarningHighBg = Math.Round(Settings.WarningHighBg *= 18);
            Settings.WarningLowBg = Math.Round(Settings.WarningLowBg *= 18);
            Settings.LowBg = Math.Round(Settings.LowBg *= 18);
            Settings.CriticalLowBg = Math.Round(Settings.CriticalLowBg *= 18);
        }

        private void UpdateValuesFromMGToMMoL(object sender, RoutedEventArgs e)
        {
            if (!HaveBypassedInitialModification)
            {
                HaveBypassedInitialModification = true;
                return;
            }
            Settings.HighBg = Math.Round(Settings.HighBg /= 18, 1);
            Settings.WarningHighBg = Math.Round(Settings.WarningHighBg /= 18, 1);
            Settings.WarningLowBg = Math.Round(Settings.WarningLowBg /= 18, 1);
            Settings.LowBg = Math.Round(Settings.LowBg /= 18, 1);
            Settings.CriticalLowBg = Math.Round(Settings.CriticalLowBg /= 18, 1);
        }

        // TODO: Expand me
        /// <summary>
        /// TODO: This should not live in the window since it is used in multiple places
        /// If model is null, will validate from stored settings file.
        /// </summary>
        /// <param name="model"></param>
        public List<string> ValidateSettings(GlucoseTraySettings model = null)
        {
            var errors = new List<string>();

            if (model is null)
            {
                model = FileService<GlucoseTraySettings>.ReadModelFromFile(Program.SettingsFile);
                if (model is null)
                    errors.Add("File is Invalid");
            }

            if (model.FetchMethod == FetchMethod.DexcomShare)
            {
                if (string.IsNullOrWhiteSpace(model.DexcomUsername))
                    errors.Add("DexcomUsername is missing");
                if (string.IsNullOrWhiteSpace(model.DexcomPassword))
                    errors.Add("DexcomPassword is missing");
            }
            else if (string.IsNullOrWhiteSpace(model.NightscoutUrl))
            {
                errors.Add("NightscoutUrl is missing");
            }

            if (string.IsNullOrWhiteSpace(model.DatabaseLocation))
                errors.Add("DatabaseLocation is missing");

            if (!(model.HighBg > model.WarningHighBg && model.WarningHighBg > model.WarningLowBg && model.WarningLowBg > model.LowBg && model.LowBg > model.CriticalLowBg))
                errors.Add("Thresholds overlap ");

            return errors;
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            //var settingsModel = new GlucoseTraySettings
            //{
            //    FetchMethod = radio_source_dexcom.IsChecked == true ? FetchMethod.DexcomShare : FetchMethod.NightscoutApi,
            //    AccessToken = textBox_nightscout_token.Text.Length > 0 ? StringEncryptionService.EncryptString(textBox_nightscout_token.Text, "i_can_probably_be_improved") : string.Empty,
            //    NightscoutUrl = textBox_nightscout_url.Text,
            //    DexcomUsername = textBox_dexcom_username.Text.Length > 0 ? StringEncryptionService.EncryptString(textBox_dexcom_username.Text, "i_can_probably_be_improved") : string.Empty,
            //    DexcomPassword = maskedText_dexcom_password.Text.Length > 0 ? StringEncryptionService.EncryptString(maskedText_dexcom_password.Text, "i_can_probably_be_improved") : string.Empty,
            //    DexcomServer = radio_dexcom_server_us_share1.Checked ? DexcomServerLocation.DexcomShare1 : radio_dexcom_server_us_share2.Checked ? DexcomServerLocation.DexcomShare2 : DexcomServerLocation.DexcomInternational,
            //    CriticalLowBg = (double) txt_critical.Text,
            //    HighBg = (double) txt_high.Text,
            //    LowBg = (double) txt_low.Text,
            //    WarningLowBg = (double) txt_warn_low.Text,
            //    WarningHighBg = (double) txt_warn_high.Text,
            //    GlucoseUnit = radio_glucose_unit_mg.Checked ? GlucoseUnitType.MG : GlucoseUnitType.MMOL,
            //    DatabaseLocation = textBox_db_location_result.Text,
            //    EnableDebugMode = checkBox_debug_mode.Checked,
            //    LogLevel = LogLevels[(string)comboBox_log_level.SelectedValue],
            //    PollingThreshold = (int) numeric_polling_threshold.Value,
            //    StaleResultsThreshold = (int) numeric_stale_results.Value
            //};

            //var errors = ValidateSettings(settingsModel);

            //if (!radio_dexcom.Checked && !radio_nightscout.Checked)
            //    errors.Add("Glucose Datasource is missing : You must select either Dexcom or Nightscout");

            //if (radio_dexcom.Checked && (!radio_dexcom_server_us_share1.Checked && !radio_dexcom_server_us_share2.Checked && !radio_dexcom_server_international.Checked))
            //    errors.Add("Dexcom Server is missing");

            //if (!radio_glucose_unit_mg.Checked && !radio_glucose_unit_mmol.Checked)
            //    errors.Add("Glucose Unit is missing : You must select either MG/DL or MMOL/L");

            //if (errors.Any())
            //{
            //    MessageBox.Show("Settings are not valid.  Please fix before continuing.\r\n\r\n" + String.Join("\r\n", errors));
            //    return;
            //}

            //FileService<GlucoseTraySettings>.WriteModelToJsonFile(settingsModel, Program.SettingsFile);

            Close();
            // DialogResult = DialogResult.OK;
        }
    }
}
