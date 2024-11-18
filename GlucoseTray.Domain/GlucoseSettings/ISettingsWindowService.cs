using GlucoseTray.Domain.Enums;

namespace GlucoseTray.Domain.GlucoseSettings;

public interface ISettingsWindowService
{
    GlucoseTraySettings GetDefaultSettings();
    IEnumerable<string> GetDexComServerLocationDescriptions();
    GlucoseTraySettings? GetSettingsFromFile();
    void UpdateValuesFromMMoLToMG(GlucoseTraySettings settings);
    void UpdateValuesFromMGToMMoL(GlucoseTraySettings _settings);
    void UpdateServerDetails(GlucoseTraySettings settings, DataSource fetchMethod, GlucoseUnitType unitType, DexcomServerLocation dexcomServerLocation, string dexcomUsername, string dexcomPassword, string nightscoutAccessToken);
    (bool IsValid, IEnumerable<string> Errors) IsValid(GlucoseTraySettings settings);
    void Save(GlucoseTraySettings settings);
}
