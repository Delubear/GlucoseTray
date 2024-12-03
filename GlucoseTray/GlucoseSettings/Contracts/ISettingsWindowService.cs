namespace GlucoseTray.GlucoseSettings.Contracts;

public interface ISettingsWindowService
{
    GlucoseTraySettings GetDefaultSettings();
    GlucoseTraySettings? GetSettingsFromFile();
    void UpdateValuesFromMMoLToMG(GlucoseTraySettingsViewModel settings);
    void UpdateValuesFromMGToMMoL(GlucoseTraySettingsViewModel _settings);
    (bool IsValid, IEnumerable<string> Errors) IsValid(GlucoseTraySettings settings);
    void Save(GlucoseTraySettings settings);
}
