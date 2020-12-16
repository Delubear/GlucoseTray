using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Windows.Forms;

namespace GlucoseTrayCore.Services
{
    public static class FileService<T>
    {
        public static void WriteModelToJsonFile(T model, string file)
        {
            using var sw = File.CreateText(file);
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(model, options);
            sw.Write(json);
        }

        public static T ReadModelFromFile(string file)
        {
            T model = default;
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

    public static class FileService
    {
        public static string LoadHtmlFromResource(string resourceName)
        {
            using var sr = new StreamReader(Assembly.GetEntryAssembly().GetManifestResourceStream(resourceName));
            return sr.ReadToEnd();
        }
    }
}
