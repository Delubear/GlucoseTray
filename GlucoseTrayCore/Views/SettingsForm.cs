using GlucoseTrayCore.Enums;
using GlucoseTrayCore.Services;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GlucoseTrayCore.Views
{
    public partial class SettingsForm : Form
    {
        private readonly string ResourcePath = Application.UserAppDataPath + @"\" +"GlucoseTrayCore.Views.html.settings.html";
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


        public SettingsForm()
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
            
        }
    }
}
