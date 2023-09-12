using System.Threading;

namespace GlucoseTray.Services
{
    public static class LocalizationService
    {
        private const string DefaultCulture = "en-us";

        public static string GetText(string key)
        {
            var currentCulter = Thread.CurrentThread.CurrentUICulture;

            if (Program.AppSettings.Languages.TryGetValue(currentCulter.Name.ToLower(), out var text))
            {
                if (text.TryGetValue(key, out var value))
                    return value;
            }

            if (Program.AppSettings.Languages.TryGetValue(DefaultCulture, out var defaultText))
            {
                if (defaultText.TryGetValue(key, out var value))
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
