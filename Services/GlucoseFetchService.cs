using System;
using System.Net.Http;
using System.Text;
using GlucoseTray.Models;

namespace GlucoseTray.Services
{
    public class GlucoseFetchService
    {
        private readonly string baseUrl = "https://share1.dexcom.com";

        public GlucoseFetchResult GetLatestReading()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage();
            var response = new HttpResponseMessage();

            try
            {
                // Get Session Id
                request.RequestUri = new Uri($"{baseUrl}/ShareWebServices/Services/General/LoginPublisherAccountByName");
                request.Method = HttpMethod.Post;
                request.Content = new StringContent("{\"accountName\":\"" + Constants.DexcomUsername + "\"," +
                                                     "\"applicationId\":\"d8665ade-9673-4e27-9ff6-92db4ce13d13\"," +
                                                     "\"password\":\"" + Constants.DexcomPassword + "\"}", Encoding.UTF8, "application/json");
                response = client.SendAsync(request).Result;
                var sessionId = response.Content.ReadAsStringAsync().Result.Replace("\"", "");

                request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{baseUrl}/ShareWebServices/Services/Publisher/ReadPublisherLatestGlucoseValues?sessionId={sessionId}&minutes=1440&maxCount=1"),
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
                var trendIcon = GetTrendArrow(trend);

                return new GlucoseFetchResult()
                {
                    Value = val,
                    Time = localTime,
                    TrendIcon = trendIcon
                };
            }
            catch (Exception e)
            {
                System.IO.File.AppendAllText(Constants.ErrorLogPath, DateTime.Now.ToString() + e.Message + e.Message + e.InnerException + e.StackTrace + Environment.NewLine + Environment.NewLine);

                return new GlucoseFetchResult()
                {
                    Value = 0,
                    Time = DateTime.Now,
                    TrendIcon = GetTrendArrow("4")
                };
            }
            finally
            {
                client.Dispose();
                response.Dispose();
                request.Dispose();
            }
        }

        private string GetTrendArrow(string trend)
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
    }
}
