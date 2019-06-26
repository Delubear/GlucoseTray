using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using GlucoseTray.Models;

namespace GlucoseTray.Services
{
    public class GlucoseFetchService
    {
        private readonly string baseDexcomUrl = "https://share1.dexcom.com";
        private readonly string nightscoutRequestUrl = $"{Constants.NightscoutUrl}/api/v1/entries/sgv?count=1";

        public GlucoseFetchResult GetLatestReading(ILogService logger)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage();
            var response = new HttpResponseMessage();

            try
            {
                if(Convert.ToInt32(Constants.FetchMethod) == 0) // Dexcom method
                {
                    // Get Session Id
                    request.RequestUri = new Uri($"{baseDexcomUrl}/ShareWebServices/Services/General/LoginPublisherAccountByName");
                    request.Method = HttpMethod.Post;
                    request.Content = new StringContent("{\"accountName\":\"" + Constants.DexcomUsername + "\"," +
                                                         "\"applicationId\":\"d8665ade-9673-4e27-9ff6-92db4ce13d13\"," +
                                                         "\"password\":\"" + Constants.DexcomPassword + "\"}", Encoding.UTF8, "application/json");
                    response = client.SendAsync(request).Result;
                    var sessionId = response.Content.ReadAsStringAsync().Result.Replace("\"", "");

                    request = new HttpRequestMessage
                    {
                        RequestUri = new Uri($"{baseDexcomUrl}/ShareWebServices/Services/Publisher/ReadPublisherLatestGlucoseValues?sessionId={sessionId}&minutes=1440&maxCount=1"),
                        Method = HttpMethod.Post
                    };
                    var result = client.SendAsync(request).Result.Content.ReadAsStringAsync().Result
                                                                                                    .Replace("[", "")
                                                                                                    .Replace("]", "")
                                                                                                    .Replace("\\", "")
                                                                                                    .Replace("/", "")
                                                                                                    .Replace("\"", "")
                                                                                                    .Replace("{", "")
                                                                                                    .Replace("}", "")
                                                                                                    .Replace("(", "")
                                                                                                    .Replace(")", "")
                                                                                                    .Split(',');

                    // Get raw data seperated
                    var unixTime = result[1].Split('e')[1];
                    var trend = result[2].Split(':')[1];
                    var stringVal = result[3].Split(':')[1];

                    // Convert raw to correct data
                    DateTime localTime = DateTime.MinValue;
                    if (!string.IsNullOrWhiteSpace(unixTime))
                        localTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(unixTime)).LocalDateTime;
                    int.TryParse(stringVal, out int val);
                    var trendIcon = GetTrendArrowDexcom(trend);

                    return new GlucoseFetchResult()
                    {
                        Value = val,
                        Time = localTime,
                        TrendIcon = trendIcon
                    };
                }
                else // Nightscout method
                {
                    if (!string.IsNullOrWhiteSpace(Constants.AccessToken))
                        request.RequestUri = new Uri($"{nightscoutRequestUrl}&token={Constants.AccessToken}");
                    else
                        request.RequestUri = new Uri(nightscoutRequestUrl);
                    request.Method = HttpMethod.Get;
                    response = client.SendAsync(request).Result;
                    var content = response.Content.ReadAsStringAsync().Result.Split('\t');
                    Regex rgx = new Regex("[^a-zA-Z]");
                    var time = ParseDate(content[0]);
                    var trendString = rgx.Replace(content[3], "");
                    var val = int.Parse(content[2]);

                    return new GlucoseFetchResult()
                    {
                        Value = val,
                        Time = time,
                        TrendIcon = GetTrendArrowNightscout(trendString)
                    };
                }
            }
            catch (Exception e)
            {
                logger.Log(e);

                return new GlucoseFetchResult()
                {
                    Value = 0,
                    Time = DateTime.Now,
                    TrendIcon = GetTrendArrowDexcom("4")
                };
            }
            finally
            {
                client.Dispose();
                response.Dispose();
                request.Dispose();
            }
        }

        private DateTime ParseDate(string date)
        {
            Regex rgx = new Regex("[^0-9]");

            var ymd = date.Split('T')[0].Split('-');
            var hms = date.Split('T')[1].Split('.')[0].Split(':');

            var year = int.Parse(rgx.Replace(ymd[0], ""));
            var month = int.Parse(rgx.Replace(ymd[1], ""));
            var day = int.Parse(rgx.Replace(ymd[2], ""));

            var hour = int.Parse(rgx.Replace(hms[0], ""));
            var minute = int.Parse(rgx.Replace(hms[1], ""));
            var second = int.Parse(rgx.Replace(hms[2], ""));

            var dateTime = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);

            return dateTime.ToLocalTime();
        }

        private string GetTrendArrowDexcom(string trend)
        {
            if (string.Equals(trend, "4", StringComparison.OrdinalIgnoreCase))
                return "→";
            else if (string.Equals(trend, "5", StringComparison.OrdinalIgnoreCase))
                return "↘";
            else if (string.Equals(trend, "3", StringComparison.OrdinalIgnoreCase))
                return "↗";
            else if (string.Equals(trend, "6", StringComparison.OrdinalIgnoreCase))
                return "↓";
            else if (string.Equals(trend, "2", StringComparison.OrdinalIgnoreCase))
                return "↑";
            else if (string.Equals(trend, "7", StringComparison.OrdinalIgnoreCase))
                return "⮇";
            else if (string.Equals(trend, "1", StringComparison.OrdinalIgnoreCase))
                return "⮅";
            else
                return "";
        }

        private string GetTrendArrowNightscout(string trend)
        {
            if (string.Equals(trend, "Flat", StringComparison.OrdinalIgnoreCase))
                return "→";
            else if (string.Equals(trend, "FortyFiveDown", StringComparison.OrdinalIgnoreCase))
                return "↘";
            else if (string.Equals(trend, "FortyFiveUp", StringComparison.OrdinalIgnoreCase))
                return "↗";
            else if (string.Equals(trend, "SingleDown", StringComparison.OrdinalIgnoreCase))
                return "↓";
            else if (string.Equals(trend, "SingleUp", StringComparison.OrdinalIgnoreCase))
                return "↑";
            else if (string.Equals(trend, "DoubleDown", StringComparison.OrdinalIgnoreCase))
                return "⮇";
            else if (string.Equals(trend, "DoubleUp", StringComparison.OrdinalIgnoreCase))
                return "⮅";
            else
                return "";
        }
    }
}
