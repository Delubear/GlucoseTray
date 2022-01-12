using GlucoseTrayCore.Enums;
using System.Collections.Generic;
using System;
using System.Net.Http;

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
            else
            {
                string nightScoutError = ValidateNightScout(model);
                if (!String.IsNullOrWhiteSpace(nightScoutError))
                    errors.Add(nightScoutError);
            }
            if (string.IsNullOrWhiteSpace(model.DatabaseLocation))
                errors.Add("Database Location is missing");

            if (!(model.HighBg > model.WarningHighBg && model.WarningHighBg > model.WarningLowBg && model.WarningLowBg > model.LowBg && model.LowBg > model.CriticalLowBg))
                errors.Add("Thresholds overlap ");

            return errors;
        }

        private static string ValidateNightScout(GlucoseTraySettings model)
        {
            if (string.IsNullOrWhiteSpace(model.NightscoutUrl))
                return "Nightscout Url is missing";

            if (!model.NightscoutUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase) &&
                !model.NightscoutUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                return "Nightscout URL must start with http:// or https://";

            Uri testUrl;
            if (!Uri.TryCreate(model.NightscoutUrl, System.UriKind.Absolute, out testUrl))
                return "Invalid Nightscout URL";
            
            try
            {
                var url = $"{model.NightscoutUrl}/api/v1/status.json";

                url += !string.IsNullOrWhiteSpace(model.AccessToken) ? $"?token={model.AccessToken}" : string.Empty;

                using (var httpClient = new HttpClient())
                {
                    var request = new  HttpRequestMessage(HttpMethod.Get, new Uri(url));
                    request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    var response = httpClient.SendAsync(request).Result;

                    if (!response.IsSuccessStatusCode)
                        return "Invalid Nightscout url - Error : " + response.ReasonPhrase;

                    var result = response.Content.ReadAsStringAsync().Result;

                    try
                    {
                        var status = System.Text.Json.JsonSerializer.Deserialize<Models.NightScoutStatus>(result);

                        if (String.Equals(status.status, "ok", StringComparison.CurrentCultureIgnoreCase))
                            return null;

                        return "Nightscout status is " + status.status;
                    }
                    catch (System.Text.Json.JsonException)
                    {
                        return "Nightscout URL returned invalid staus " + result;
                    }

                }
            }
            catch (Exception ex)
            {
                return "Can not access Nightscout : Error " + ex.Message;
            }

        }

    }
}
