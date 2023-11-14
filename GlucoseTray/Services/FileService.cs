using System.IO;
using System.Text.Json;

namespace GlucoseTray.Services;

public static class FileService
{
    public static void WriteModelToJsonFile(GlucoseTraySettings model, string file)
    {
        using var sw = File.CreateText(file);
        //var options = new JsonSerializerOptions { WriteIndented = true, TypeInfoResolver = GlucoseSourceGenerationContext.Default };
        var json = JsonSerializer.Serialize(model, GlucoseSourceGenerationContext.Default.GlucoseTraySettings);
        sw.Write(json);
    }

    public static GlucoseTraySettings? ReadModelFromFile(string file)
    {
        GlucoseTraySettings? model = default;
        try
        {
            var json = File.ReadAllText(file);
            model = JsonSerializer.Deserialize<GlucoseTraySettings>(json, GlucoseSourceGenerationContext.Default.GlucoseTraySettings);
        }
        catch
        {
        }

        return model;
    }
}
