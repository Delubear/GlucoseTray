using GlucoseTrayCore.Enums;
using GlucoseTrayCore.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        private readonly string ResourcePath = "GlucoseTrayCore.Views.Settings.SettingsUI.html";

        public SettingsWindow()
        {
            InitializeComponent();

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
                }
                catch (Exception e) // Catch serialization errors due to a bad file
                {
                    MessageBox.Show("Unable to load existing settings due to a bad file.  " + e.Message + e.InnerException?.Message);
                }
            }

            var html = FileService.LoadHtmlFromResource(ResourcePath);
            webBrowser.ObjectForScripting = new SettingsJavaScriptInteractionModel(this);
            webBrowser.NavigateToString(html);
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
