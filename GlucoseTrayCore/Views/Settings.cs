using GlucoseTrayCore.Enums;
using GlucoseTrayCore.Services;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace GlucoseTrayCore.Views
{
    public partial class Settings : Form
    {
        private GlucoseUnitType SelectedGlucoseType = GlucoseUnitType.MG;

        public static readonly Dictionary<string, LogEventLevel> LogLevels = new Dictionary<string, LogEventLevel>
        {
            { "Verbose", LogEventLevel.Verbose },
            { "Debug", LogEventLevel.Debug },
            { "Information", LogEventLevel.Information },
            { "Warning", LogEventLevel.Warning },
            { "Error", LogEventLevel.Warning },
            { "Fatal", LogEventLevel.Fatal },
        };

        public Settings()
        {
            InitializeComponent();
            comboBox_log_level.DataSource = LogLevels.Select(x => x.Key).ToList();

            if (File.Exists(Program.SettingsFile))
            {
                try
                {
                    var model = FileService<GlucoseTraySettings>.ReadModelFromFile(Program.SettingsFile);

                    if (model is null)
                    {
                        MessageBox.Show("Unable to load existing settings due to a bad file.");
                        return;
                    }

                    if (model.FetchMethod == FetchMethod.DexcomShare)
                        radio_dexcom.Checked = true;
                    else
                        radio_nightscout.Checked = true;

                    if (model.GlucoseUnit == GlucoseUnitType.MG)
                    {
                        radio_glucose_unit_mg.Checked = true;
                        UpdateGlucoseNumericValues(GlucoseUnitType.MG);
                    }
                    else
                    {
                        radio_glucose_unit_mmol.Checked = true;
                        UpdateGlucoseNumericValues(GlucoseUnitType.MMOL);
                    }

                    textBox_dexcom_username.Text = string.IsNullOrWhiteSpace(model.DexcomUsername) ? string.Empty : StringEncryptionService.DecryptString(model.DexcomUsername, "i_can_probably_be_improved");
                    maskedText_dexcom_password.Text = string.IsNullOrWhiteSpace(model.DexcomPassword) ? string.Empty : StringEncryptionService.DecryptString(model.DexcomPassword, "i_can_probably_be_improved");

                    if (model.DexcomServer == DexcomServerLocation.DexcomShare1)
                        radio_dexcom_server_us_share1.Checked = true;
                    else if (model.DexcomServer == DexcomServerLocation.DexcomShare2)
                        radio_dexcom_server_us_share2.Checked = true;
                    else
                        radio_dexcom_server_international.Checked = true;

                    textBox_nightscout_token.Text = string.IsNullOrWhiteSpace(model.AccessToken) ? string.Empty : StringEncryptionService.DecryptString(model.AccessToken, "i_can_probably_be_improved");
                    textBox_nightscout_url.Text = model.NightscoutUrl;

                    numeric_glucose_critical.Value = (decimal)model.CriticalLowBg;
                    numeric_glucose_high.Value = (decimal)model.HighBg;
                    numeric_glucose_low.Value = (decimal)model.LowBg;
                    numeric_glucose_warning_high.Value = (decimal)model.WarningHighBg;
                    numeric_glucose_warning_low.Value = (decimal)model.WarningLowBg;

                    numeric_polling_threshold.Value = model.PollingThreshold;
                    numeric_stale_results.Value = model.StaleResultsThreshold;

                    comboBox_log_level.SelectedItem = model.LogLevel.ToString();
                    checkBox_debug_mode.Checked = model.EnableDebugMode;
                    textBox_db_location_result.Text = model.DatabaseLocation;

                }
                catch (Exception e) // Catch serialization errors due to a bad file
                {
                    MessageBox.Show("Unable to load existing settings due to a bad file.  " + e.Message + e.InnerException?.Message);
                }
            }
        }

        private void radio_dexcom_CheckedChanged(object sender, EventArgs e)
        {
            nightscout_grid.Visible = false;
            dexcom_settings_grid.Visible = true;
        }

        private void radio_nightscout_CheckedChanged(object sender, EventArgs e)
        {
            dexcom_settings_grid.Visible = false;
            nightscout_grid.Visible = true;
        }

        private void radio_glucose_unit_mg_CheckedChanged(object sender, EventArgs e)
        {
            UpdateGlucoseNumericValues(GlucoseUnitType.MG);
            glucose_values_grid.Enabled = true;
        }

        private void radio_glucose_unit_mmol_CheckedChanged(object sender, EventArgs e)
        {
            UpdateGlucoseNumericValues(GlucoseUnitType.MMOL);
            glucose_values_grid.Enabled = true;
        }

        private void UpdateGlucoseNumericValues(GlucoseUnitType setToUnitType)
        {
            var controls = new [] {
                numeric_glucose_high,
                numeric_glucose_warning_high,
                numeric_glucose_warning_low,
                numeric_glucose_low,
                numeric_glucose_critical
            };

            foreach (var control in controls)
            {
                if (setToUnitType == GlucoseUnitType.MG)
                {
                    if(SelectedGlucoseType == GlucoseUnitType.MMOL)
                        control.Value *= 18;
                    control.Increment = 1;
                    control.DecimalPlaces = 0;
                }
                else
                {
                    control.DecimalPlaces = 1;
                    control.Increment = 0.1m;
                    if (SelectedGlucoseType == GlucoseUnitType.MG)
                        control.Value /= 18;
                }
            }

            SelectedGlucoseType = setToUnitType == GlucoseUnitType.MG ? GlucoseUnitType.MG : GlucoseUnitType.MMOL;
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            var settingsModel = new GlucoseTraySettings
            {
                FetchMethod = radio_dexcom.Checked ? FetchMethod.DexcomShare : FetchMethod.NightscoutApi,
                AccessToken = textBox_nightscout_token.Text.Length > 0 ? StringEncryptionService.EncryptString(textBox_nightscout_token.Text, "i_can_probably_be_improved") : string.Empty,
                NightscoutUrl = textBox_nightscout_url.Text,
                DexcomUsername = textBox_dexcom_username.Text.Length > 0 ? StringEncryptionService.EncryptString(textBox_dexcom_username.Text, "i_can_probably_be_improved") : string.Empty,
                DexcomPassword = maskedText_dexcom_password.Text.Length > 0 ? StringEncryptionService.EncryptString(maskedText_dexcom_password.Text, "i_can_probably_be_improved") : string.Empty,
                DexcomServer = radio_dexcom_server_us_share1.Checked ? DexcomServerLocation.DexcomShare1 : radio_dexcom_server_us_share2.Checked ? DexcomServerLocation.DexcomShare2 : DexcomServerLocation.DexcomInternational,
                CriticalLowBg = (double) numeric_glucose_critical.Value,
                HighBg = (double) numeric_glucose_high.Value,
                LowBg = (double) numeric_glucose_low.Value,
                WarningLowBg = (double) numeric_glucose_warning_low.Value,
                WarningHighBg = (double) numeric_glucose_warning_high.Value,
                GlucoseUnit = radio_glucose_unit_mg.Checked ? GlucoseUnitType.MG : GlucoseUnitType.MMOL,
                DatabaseLocation = textBox_db_location_result.Text,
                EnableDebugMode = checkBox_debug_mode.Checked,
                LogLevel = LogLevels[(string)comboBox_log_level.SelectedValue],
                PollingThreshold = (int) numeric_polling_threshold.Value,
                StaleResultsThreshold = (int) numeric_stale_results.Value
            };

            var errors = ValidateSettings(settingsModel);

            if (!radio_dexcom.Checked && !radio_nightscout.Checked)
                errors.Add("Glucose Datasource is missing : You must select either Dexcom or Nightscout");

            if (radio_dexcom.Checked && (!radio_dexcom_server_us_share1.Checked && !radio_dexcom_server_us_share2.Checked && !radio_dexcom_server_international.Checked))
                errors.Add("Dexcom Server is missing");

            if (!radio_glucose_unit_mg.Checked && !radio_glucose_unit_mmol.Checked)
                errors.Add("Glucose Unit is missing : You must select either MG/DL or MMOL/L");

            if (errors.Any())
            {
                MessageBox.Show("Settings are not valid.  Please fix before continuing.\r\n\r\n" + String.Join("\r\n", errors));
                return;
            }            

            FileService<GlucoseTraySettings>.WriteModelToJsonFile(settingsModel, Program.SettingsFile);

            Close();
            DialogResult = DialogResult.OK;
        }

        /// <summary>
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
    }
}
