using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using GlucoseTray.Domain.Enums;
using GlucoseTray.GlucoseSettings;
using GlucoseTray.Infrastructure;

namespace GlucoseTray.Views;

public interface ISettingsWindowService
{
    GlucoseTraySettings GetDefaultSettings();
    IEnumerable<string> GetDexComServerLocationDescriptions();
    GlucoseTraySettings? GetSettingsFromFile();
    void UpdateValuesFromMMoLToMG(GlucoseTraySettings settings);
    void UpdateValuesFromMGToMMoL(GlucoseTraySettings _settings);
    void UpdateServerDetails(GlucoseTraySettings settings, FetchMethod fetchMethod, GlucoseUnitType unitType, DexcomServerLocation dexcomServerLocation, string dexcomUsername, string dexcomPassword, string nightscoutAccessToken);
    (bool IsValid, IEnumerable<string> Errors) IsValid(GlucoseTraySettings settings);
    void Save(GlucoseTraySettings settings);
}

public class SettingsWindowService(IFileService<GlucoseTraySettings> fileService, ISettingsService settingsService) : ISettingsWindowService
{
    public void Save(GlucoseTraySettings settings)
    {
        fileService.WriteModelToJsonFile(settings, Program.SettingsFile);
    }

    public (bool IsValid, IEnumerable<string> Errors) IsValid(GlucoseTraySettings settings)
    {
        var errors = settingsService.ValidateSettings(settings);
        if (errors.Any())
            return (false, errors);
        return (true, errors);
    }

    public void UpdateServerDetails(GlucoseTraySettings settings, FetchMethod fetchMethod, GlucoseUnitType unitType, DexcomServerLocation dexcomServerLocation, string dexcomUsername, string dexcomPassword, string nightscoutAccessToken)
    {
        settings.FetchMethod = fetchMethod;
        settings.GlucoseUnit = unitType;
        settings.DexcomServer = dexcomServerLocation;
        settings.DexcomUsername = dexcomUsername;
        settings.DexcomPassword = dexcomPassword;
        settings.AccessToken = nightscoutAccessToken;
    }

    public void UpdateValuesFromMGToMMoL(GlucoseTraySettings settings)
    {
        settings.HighBg = Math.Round(settings.HighBg /= 18, 1);
        settings.WarningHighBg = Math.Round(settings.WarningHighBg /= 18, 1);
        settings.WarningLowBg = Math.Round(settings.WarningLowBg /= 18, 1);
        settings.LowBg = Math.Round(settings.LowBg /= 18, 1);
        settings.CriticalLowBg = Math.Round(settings.CriticalLowBg /= 18, 1);
    }

    public void UpdateValuesFromMMoLToMG(GlucoseTraySettings settings)
    {
        settings.HighBg = Math.Round(settings.HighBg *= 18);
        settings.WarningHighBg = Math.Round(settings.WarningHighBg *= 18);
        settings.WarningLowBg = Math.Round(settings.WarningLowBg *= 18);
        settings.LowBg = Math.Round(settings.LowBg *= 18);
        settings.CriticalLowBg = Math.Round(settings.CriticalLowBg *= 18);
    }

    public GlucoseTraySettings GetDefaultSettings()
    {
        return new GlucoseTraySettings
        {
            HighBg = 250,
            WarningHighBg = 200,
            WarningLowBg = 80,
            LowBg = 70,
            CriticalLowBg = 55,
            PollingThreshold = 15,
            StaleResultsThreshold = 15,
            DexcomUsername = "",
            NightscoutUrl = "",
            IsServerDataUnitTypeMmol = false,
            IsDebugMode = false,
            IsDarkMode = true,
            // Appears we will need to manually bind radios, dropdowns, and password fields for now.
        };
    }

    public IEnumerable<string> GetDexComServerLocationDescriptions()
    {
        return typeof(DexcomServerLocation).GetFields().Select(x => (DescriptionAttribute[])x.GetCustomAttributes(typeof(DescriptionAttribute), false)).SelectMany(x => x).Select(x => x.Description);
    }

    public GlucoseTraySettings? GetSettingsFromFile()
    {
        GlucoseTraySettings? model = null;
        if (File.Exists(Program.SettingsFile))
        {
            try
            {
                model = fileService.ReadModelFromFile(Program.SettingsFile);

                if (model is null)
                    MessageBox.Show("Unable to load existing settings due to a bad file.");
            }
            catch (Exception e) // Catch serialization errors due to a bad file
            {
                MessageBox.Show("Unable to load existing settings due to a bad file.  " + e.Message + e.InnerException?.Message);
            }
        }

        return model;
    }
}
