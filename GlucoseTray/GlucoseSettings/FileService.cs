using GlucoseTray.GlucoseSettings.Contracts;
using System.IO;
using System.Text.Json;

namespace GlucoseTray.GlucoseSettings;

public class FileService<T> : ILocalFileAdapter<T> where T : class
{
    public void WriteModelToJsonFile(T model, string file)
    {
        using var sw = File.CreateText(file);
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(model, options);
        sw.Write(json);
    }

    public T? ReadModelFromFile(string file)
    {
        T? model = default;
        try
        {
            var json = File.ReadAllText(file);
            model = JsonSerializer.Deserialize<T>(json);
        }
        catch
        {
        }

        return model;
    }

    public bool DoesFileExist(string filePath)
    {
        return File.Exists(filePath);
    }
}
