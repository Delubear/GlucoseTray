using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Dexcom.Fetch.Enums;
using Dexcom.Fetch.Extensions;
using Dexcom.Fetch.Models;

namespace Dexcom.Fetch
{
    public class GlucoseFetchService
    {
        private readonly GlucoseFetchConfiguration _config;

        public GlucoseFetchService(GlucoseFetchConfiguration config)
        {
            _config = config;
        }

        public GlucoseFetchResult GetLatestReading()
        {
            var fetchResult = new GlucoseFetchResult();

            try
            {
                if (_config.FetchMethod == FetchMethod.DexcomShare)
                    GetFetchResultFromDexcom(fetchResult);
                else if (_config.FetchMethod == FetchMethod.NightscoutApi)
                    GetFetchResultFromNightscout(fetchResult);
                else
                    throw new InvalidOperationException("Fetch Method either not specified or invalid specification.");
            }
            catch
            {
                fetchResult.Value = 0;
                fetchResult.Time = DateTime.Now;
                fetchResult.TrendIcon = "4".GetTrendArrowFromDexcom();

                throw;
            }

            return fetchResult;
        }

        private GlucoseFetchResult GetFetchResultFromNightscout(GlucoseFetchResult fetchResult)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{_config.NightscoutUrl}/api/v1/entries/sgv?count=1" + (!string.IsNullOrWhiteSpace(_config.NightscoutAccessToken) ? $"&token={_config.NightscoutAccessToken}" : "")),
                Method = HttpMethod.Get,
            };

            try
            {
                var response = new HttpClient().SendAsync(request).Result;
                var content = response.Content.ReadAsStringAsync().Result.Split('\t');
                Regex rgx = new Regex("[^a-zA-Z]");
                var time = content[0].ParseNightscoutDate();
                var trendString = rgx.Replace(content[3], "");
                var val = int.Parse(content[2]);
                fetchResult.Value = val;
                fetchResult.Time = time;
                fetchResult.TrendIcon = trendString.GetTrendArrowFromNightscout();
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Did not get expected request.", ex);
            }

            return fetchResult;
        }

        private GlucoseFetchResult GetFetchResultFromDexcom(GlucoseFetchResult fetchResult)
        {
            // Get Session Id
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://share1.dexcom.com/ShareWebServices/Services/General/LoginPublisherAccountByName"),
                Method = HttpMethod.Post,
                Content = new StringContent("{\"accountName\":\"" + _config.DexcomUsername + "\"," +
                                                 "\"applicationId\":\"d8665ade-9673-4e27-9ff6-92db4ce13d13\"," +
                                                 "\"password\":\"" + _config.DexcomPassword + "\"}", Encoding.UTF8, "application/json")
            };

            try
            {
                var client = new HttpClient();
                var response = client.SendAsync(request).Result;
                var sessionId = response.Content.ReadAsStringAsync().Result.Replace("\"", "");

                request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"https://share1.dexcom.com/ShareWebServices/Services/Publisher/ReadPublisherLatestGlucoseValues?sessionId={sessionId}&minutes=1440&maxCount=1"),
                    Method = HttpMethod.Post
                };
                var result = client.SendAsync(request).Result.Content.ReadAsStringAsync().Result.RemoveUnnecessaryCharacters().Split(',');

                // Get raw data seperated
                var unixTime = result[1].Split('e')[1];
                var trend = result[2].Split(':')[1];
                var stringVal = result[3].Split(':')[1];

                // Convert raw to correct data
                DateTime localTime = DateTime.MinValue;
                if (!string.IsNullOrWhiteSpace(unixTime))
                    localTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(unixTime)).LocalDateTime;
                int.TryParse(stringVal, out int val);
                var trendIcon = trend.GetTrendArrowFromDexcom();

                fetchResult.Value = val;
                fetchResult.Time = localTime;
                fetchResult.TrendIcon = trendIcon;
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Did not get expected request.", ex);
            }

            return fetchResult;
        }
    }
}
