using System.Text.Json;
using System.Text.Json.Serialization;

namespace GlucoseTray;

internal static class AppSettingsJson
{
    public static JsonSerializerOptions SerializerOptions { get; } = new()
    {
        Converters = { new JsonStringEnumConverter() },
        WriteIndented = true,
    };
}
