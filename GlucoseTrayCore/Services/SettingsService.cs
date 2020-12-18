using GlucoseTrayCore.Enums;
using System.Collections.Generic;

namespace GlucoseTrayCore.Services
{
    public static class SettingsService
    {
        /// <summary>
        /// If model is null, will validate from stored settings file.
        /// </summary>
        /// <param name="model"></param>
        public static List<string> ValidateSettings(GlucoseTraySettings model = null)
        {
            var errors = new List<string>();

            if (model is null)
            {
                model = FileService<GlucoseTraySettings>.ReadModelFromFile(Program.SettingsFile);
                if (model is null)
                {
                    errors.Add("File is Invalid");
                    return errors;
                }
            }

            if (model.FetchMethod == FetchMethod.DexcomShare)
            {
                if (string.IsNullOrWhiteSpace(model.DexcomUsername))
                    errors.Add("Dexcom Username is missing");
                if (string.IsNullOrWhiteSpace(model.DexcomPassword))
                    errors.Add("Dexcom Password is missing");
            }
            else if (string.IsNullOrWhiteSpace(model.NightscoutUrl))
            {
                errors.Add("Nightscout Url is missing");
            }

            if (string.IsNullOrWhiteSpace(model.DatabaseLocation))
                errors.Add("Database Location is missing");

            if (!(model.HighBg > model.WarningHighBg && model.WarningHighBg > model.WarningLowBg && model.WarningLowBg > model.LowBg && model.LowBg > model.CriticalLowBg))
                errors.Add("Thresholds overlap ");

            return errors;
        }
    }
}
