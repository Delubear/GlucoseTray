namespace GlucoseTray.GlucoseSettings.Contracts;

public interface ISettingsService
{
    List<string> ValidateSettings(GlucoseTraySettings? model = null);
}
