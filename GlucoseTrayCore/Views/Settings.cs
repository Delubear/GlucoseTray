using Serilog.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GlucoseTrayCore.Views
{
    public partial class Settings : Form
    {
        private bool? SelectedGlucoseTypeIsMG;

        public static readonly Dictionary<string, int> LogLevels = new Dictionary<string, int>
        {
            { "Verbose", (int) LogEventLevel.Verbose },
            { "Debug", (int) LogEventLevel.Debug },
            { "Informational", (int) LogEventLevel.Information },
            { "Warning", (int) LogEventLevel.Warning },
            { "Error", (int) LogEventLevel.Warning },
            { "Fatal", (int) LogEventLevel.Fatal },
        };

        public Settings()
        {
            InitializeComponent();
            comboBox_log_level.DataSource = (IList<string>) LogLevels.Select(x => x.Key).ToList();
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

            foreach(var control in controls)
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
            Close();
            DialogResult = DialogResult.OK;
        }
    }
}
