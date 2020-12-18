using GlucoseTrayCore.Enums;
using GlucoseTrayCore.Services;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;

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
                        model.DexcomUsername = string.IsNullOrWhiteSpace(model.DexcomUsername) ? string.Empty : StringEncryptionService.DecryptString(model.DexcomUsername, "i_can_probably_be_improved");
                        model.DexcomPassword = string.IsNullOrWhiteSpace(model.DexcomPassword) ? string.Empty : StringEncryptionService.DecryptString(model.DexcomPassword, "i_can_probably_be_improved");
                        txt_dexcom_password.Password = model.DexcomPassword;
                        model.AccessToken = string.IsNullOrWhiteSpace(model.AccessToken) ? string.Empty : StringEncryptionService.DecryptString(model.AccessToken, "i_can_probably_be_improved");
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

        private bool HaveBypassedInitialModification;
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
                {
                    errors.Add("File is Invalid");
                    return errors;
                }
            }

            if (model.FetchMethod == FetchMethod.DexcomShare)
            {
                if (string.IsNullOrWhiteSpace(model.DexcomUsername))
                    errors.Add("Dexcom Username is missing");
                if (string.IsNullOrWhiteSpace(model.DexcomPassword))
                    errors.Add("Dexcom Password is missing");
            }
            else if (string.IsNullOrWhiteSpace(model.NightscoutUrl))
            {
                errors.Add("Nightscout Url is missing");
            }

            if (string.IsNullOrWhiteSpace(model.DatabaseLocation))
                errors.Add("Database Location is missing");

            if (!(model.HighBg > model.WarningHighBg && model.WarningHighBg > model.WarningLowBg && model.WarningLowBg > model.LowBg && model.LowBg > model.CriticalLowBg))
                errors.Add("Thresholds overlap ");

            return errors;
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            Settings.FetchMethod = radio_source_dexcom.IsChecked == true ? FetchMethod.DexcomShare : FetchMethod.NightscoutApi;
            Settings.GlucoseUnit = radio_unit_mg.IsChecked == true ? GlucoseUnitType.MG : GlucoseUnitType.MMOL;
            Settings.LogLevel = (LogEventLevel) combobox_loglevel.SelectedIndex;
            Settings.DexcomServer = (DexcomServerLocation) combobox_dexcom_server.SelectedIndex;
            Settings.DexcomUsername = txt_dexcom_username.Text.Length > 0 ? StringEncryptionService.EncryptString(txt_dexcom_username.Text, "i_can_probably_be_improved") : string.Empty;
            Settings.DexcomPassword = txt_dexcom_password.Password.Length > 0 ? StringEncryptionService.EncryptString(txt_dexcom_password.Password, "i_can_probably_be_improved") : string.Empty;
            Settings.AccessToken = txt_nightscout_token.Password.Length > 0 ? StringEncryptionService.EncryptString(txt_nightscout_token.Password, "i_can_probably_be_improved") : string.Empty;

            var errors = ValidateSettings(Settings);
            if (ValidateSettings(Settings).Any())
            {
                MessageBox.Show("Settings are not valid.  Please fix before continuing.\r\n\r\n" + string.Join("\r\n", errors));
                return;
            }

            FileService<GlucoseTraySettings>.WriteModelToJsonFile(Settings, Program.SettingsFile);

            DialogResult = true;
            Close();
        }
    }
}
