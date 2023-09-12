using System.Threading;

namespace GlucoseTray.Services
{
    public static class LocalizationService
    {
        private const string DefaultCulture = "en-us";

        public static string GetText(string key)
        {
            var currentCulture = Thread.CurrentThread.CurrentUICulture;

            if (Program.AppSettings.Languages.TryGetValue(currentCulture.Name.ToLower(), out var currentLanguage))
            {
                if (currentLanguage.TryGetValue(key, out var value))
                    return value;
            }

            if (Program.AppSettings.Languages.TryGetValue(DefaultCulture, out var defaultLanguage))
            {
                if (defaultLanguage.TryGetValue(key, out var value))
                    return value;
            }

            return "KEY NOT FOUND";
        }

        public static void SetLanguage(string language)
        {
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language);
        }
    }
}
