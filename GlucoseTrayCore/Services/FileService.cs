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
        /// <summary>
        /// TODO: Possible optimizations available or do we want to unpack everytime?
        /// </summary>
        public static void UnpackHtmlResources()
        {
            var assembly = Assembly.GetEntryAssembly();
            var htmlResourceNames = assembly.GetManifestResourceNames().Where(x => x.EndsWith(".html", System.StringComparison.OrdinalIgnoreCase));
            foreach(var resource in htmlResourceNames)
            {
                using var sr = new StreamReader(assembly.GetManifestResourceStream(resource));
                using var reader = File.CreateText(Application.UserAppDataPath + @"\" + resource);
                reader.Write(sr.ReadToEnd());
            }
        }
    }
}
