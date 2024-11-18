using GlucoseTray.Domain.DisplayResults;
using GlucoseTray.Domain.GlucoseSettings;
using GlucoseTray.Infrastructure;

namespace GlucoseTray.GlucoseSettings;

public class SettingsWindowService(ILocalFileAdapter<GlucoseTraySettings> fileService, ISettingsService settingsService, IDialogService dialogService) : ISettingsWindowService
{
    public void Save(GlucoseTraySettings settings) => fileService.WriteModelToJsonFile(settings, Program.SettingsFile);

    public (bool IsValid, IEnumerable<string> Errors) IsValid(GlucoseTraySettings settings)
    {
        var errors = settingsService.ValidateSettings(settings);
        return (errors.Count == 0, errors);
    }

    public void UpdateValuesFromMGToMMoL(GlucoseTraySettingsViewModel viewModel)
    {
        viewModel.HighBg = Math.Round(viewModel.HighBg /= 18, 1);
        viewModel.WarningHighBg = Math.Round(viewModel.WarningHighBg /= 18, 1);
        viewModel.WarningLowBg = Math.Round(viewModel.WarningLowBg /= 18, 1);
        viewModel.LowBg = Math.Round(viewModel.LowBg /= 18, 1);
        viewModel.CriticalLowBg = Math.Round(viewModel.CriticalLowBg /= 18, 1);
    }

    public void UpdateValuesFromMMoLToMG(GlucoseTraySettingsViewModel viewModel)
    {
        viewModel.HighBg = Math.Round(viewModel.HighBg *= 18);
        viewModel.WarningHighBg = Math.Round(viewModel.WarningHighBg *= 18);
        viewModel.WarningLowBg = Math.Round(viewModel.WarningLowBg *= 18);
        viewModel.LowBg = Math.Round(viewModel.LowBg *= 18);
        viewModel.CriticalLowBg = Math.Round(viewModel.CriticalLowBg *= 18);
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
        };
    }

    public GlucoseTraySettings? GetSettingsFromFile()
    {
        GlucoseTraySettings? model = null;
        if (fileService.DoesFileExist(Program.SettingsFile))
        {
            model = fileService.ReadModelFromFile(Program.SettingsFile);

            if (model is null)
                dialogService.ShowErrorAlert("Unable to load existing settings due to a bad file.", "Error");
        }

        return model;
    }
}
