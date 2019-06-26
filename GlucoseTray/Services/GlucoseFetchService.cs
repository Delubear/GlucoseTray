using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using GlucoseTray.Enums;
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
                if(Constants.FetchMethod == FetchMethod.DexcomShare)
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
                else if(Constants.FetchMethod == FetchMethod.NightscoutApi)
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
                else
                {
                    return new GlucoseFetchResult()
                    {
                        Value = 0,
                        Time = DateTime.Now,
                        TrendIcon = GetTrendArrowDexcom("4")
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
                client?.Dispose();
                response?.Dispose();
                request?.Dispose();
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
            if (int.TryParse(trend, out var trendInt))
            {
                switch ((TrendResult)trendInt)
                {
                    case TrendResult.DoubleUp: return "⮅";
                    case TrendResult.SingleUp: return "↑";
                    case TrendResult.FortyFiveUp: return "↗";
                    case TrendResult.Flat: return "→";
                    case TrendResult.FortFiveDown: return "↘";
                    case TrendResult.SingleDown: return "↓";
                    case TrendResult.DoubleDown: return "⮇";
                    default: return string.Empty;
                }
            }
            else
                return string.Empty;
        }

        private string GetTrendArrowNightscout(string trend)
        {
            switch (trend)
            {
                case nameof(TrendResult.DoubleUp): return "⮅";
                case nameof(TrendResult.SingleUp): return "↑";
                case nameof(TrendResult.FortyFiveUp): return "↗";
                case nameof(TrendResult.Flat): return "→";
                case nameof(TrendResult.FortFiveDown): return "↘";
                case nameof(TrendResult.SingleDown): return "↓";
                case nameof(TrendResult.DoubleDown): return "⮇";
                default: return string.Empty;
            }
        }
    }
}
