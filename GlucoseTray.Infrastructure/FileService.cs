using System.Text.Json;

namespace GlucoseTray.Infrastructure;

public interface IFileService<T> where T : class
{
    void WriteModelToJsonFile(T model, string file);
    T? ReadModelFromFile(string file);
}

public class FileService<T> : IFileService<T> where T : class
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
}
