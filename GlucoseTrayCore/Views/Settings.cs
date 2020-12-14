using GlucoseTrayCore.Enums;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GlucoseTrayCore.Views
{
    public partial class Settings : Form
    {
        private bool? SelectedGlucoseTypeIsMG;

        public static readonly Dictionary<string, LogEventLevel> LogLevels = new Dictionary<string, LogEventLevel>
        {
            { "Verbose", LogEventLevel.Verbose },
            { "Debug", LogEventLevel.Debug },
            { "Informational", LogEventLevel.Information },
            { "Warning", LogEventLevel.Warning },
            { "Error", LogEventLevel.Warning },
            { "Fatal", LogEventLevel.Fatal },
        };

        public Settings()
        {
            InitializeComponent();
            comboBox_log_level.DataSource = LogLevels.Select(x => x.Key).ToList();
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
            CalculateGlucoseValues(true);
            glucose_values_grid.Enabled = true;
        }

        private void radio_glucose_unit_mmol_CheckedChanged(object sender, EventArgs e)
        {
            CalculateGlucoseValues(false);
            glucose_values_grid.Enabled = true;
        }

        private void CalculateGlucoseValues(bool setForMG)
        {
            if (SelectedGlucoseTypeIsMG == !setForMG)
                UpdateGlucoseNumericValues(setForMG);

            SelectedGlucoseTypeIsMG = setForMG;
        }

        private void UpdateGlucoseNumericValues(bool setToMG)
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
                if (setToMG)
                {
                    control.Value *= 18;
                    control.Increment = 1;
                    control.DecimalPlaces = 0;
                }
                else
                {
                    control.DecimalPlaces = 1;
                    control.Increment = 0.1m;
                    control.Value /= 18;
                }
            }
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            // If everything is valid, save and return an OK result
            // TODO: Validation

            var settingsModel = new GlucoseTraySettings
            {
                FetchMethod = radio_dexcom.Checked ? FetchMethod.DexcomShare : FetchMethod.NightscoutApi,
                AccessToken = textBox_nightscout_token.Text,
                NightscoutUrl = textBox_nightscout_url.Text,
                DexcomUsername = textBox_dexcom_username.Text,
                DexcomPassword = maskedText_dexcom_password.Text, // TODO: Hash? How will that affect deserialization?
                DexcomServer = radio_dexcom_server_us_share1.Checked ? DexcomServerLocation.DexcomShare1 : radio_dexcom_server_us_share2.Checked ? DexcomServerLocation.DexcomShare2 : DexcomServerLocation.DexcomInternational,
                CriticalLowBg = (double) numeric_glucose_critical.Value,
                DangerHighBg = (double) numeric_glucose_warning_high.Value,
                DangerLowBg = (double) numeric_glucose_low.Value,
                LowBg = (double) numeric_glucose_warning_low.Value,
                HighBg = (double) numeric_glucose_high.Value,
                GlucoseUnit = radio_glucose_unit_mg.Checked ? GlucoseUnitType.MG : GlucoseUnitType.MMOL,
                DatabaseLocation = textBox_db_location_result.Text,
                EnableDebugMode = checkBox_debug_mode.Checked,
                LogLevel = LogLevels[(string)comboBox_log_level.SelectedValue],
                PollingThreshold = (int) numeric_polling_threshold.Value,
                StaleResultsThreshold = (int) numeric_stale_results.Value
            };

            using (var sw = File.CreateText(Program.SettingsFile))
            {
                var model = new { appsettings = settingsModel };
                var json = JsonSerializer.Serialize(model);
                sw.Write(json);
            }

            Close();
            DialogResult = DialogResult.OK;
        }
    }
}


/*
 * (nameof(settingsModel.FetchMethod)) = "",
                        DexcomUsername = "",
                        DexcomPassword = "",
                        DexcomServer = "",
                        AccessToken = "",
                        NightscoutUrl = "",
                        GlucoseUnit = "",
                        HighBg = "",
                        DangerHighBg = "",
                        LowBg = "",
                        DangerLowBg = "",
                        CriticalLowBg = "",
                        PollingThreshold = "",
                        DatabaseLocation = "",
                        EnableDebugMode = "",
                        LogLevel = "",
                        StaleResultsThreshold = "",
 * 
 * 
 * 
 */