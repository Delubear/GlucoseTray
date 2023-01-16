using System.IO;
using System.Text.Json;

namespace GlucoseTray.Services
{
    public static class FileService<T>
    {
        public static void WriteModelToJsonFile(T model, string file)
        {
            using StreamWriter sw = File.CreateText(file);
            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(model, options);
            sw.Write(json);
        }

        public static T ReadModelFromFile(string file)
        {
            T model = default;
            try
            {
                string json = File.ReadAllText(file);
                model = JsonSerializer.Deserialize<T>(json);
            }
            catch
            {
            }

            return model;
        }
    }
}
