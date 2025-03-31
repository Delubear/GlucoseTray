using System.Text.Json.Serialization;

namespace GlucoseTray.Settings;

public class AppSettingsContainer
{
    [JsonPropertyName("appsettings")]
    public AppSettings AppSettings { get; set; } = new AppSettings();
}