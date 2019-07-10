using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Dexcom.Fetch.Enums;
using Dexcom.Fetch.Extensions;
using Dexcom.Fetch.Models;
using Microsoft.Extensions.Logging;

namespace Dexcom.Fetch
{
    public class GlucoseFetchService
    {
        private readonly GlucoseFetchConfiguration _config;
        private readonly ILogger _logger;

        public GlucoseFetchService(GlucoseFetchConfiguration config, ILogger logger)
        {
            _config = config;
            _logger = logger;
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
                {
                    _logger.LogError("Invalid fetch method specified.");
                    throw new InvalidOperationException("Fetch Method either not specified or invalid specification.");
                }
            }
            catch ( Exception ex )
            {
                _logger.LogError("Failed to get data. {0}", ex);
                fetchResult = GetDefaultFetchResult();
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
                _logger.LogError("Nightscout fetching failed or received incorrect format. {0}", ex);
                fetchResult = GetDefaultFetchResult();
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
                _logger.LogError("Dexcom fetching failed or received incorrect format. {0}", ex);
                fetchResult = GetDefaultFetchResult();
            }

            return fetchResult;
        }

        private GlucoseFetchResult GetDefaultFetchResult()
        {
            return new GlucoseFetchResult
            {
                Value = 0,
                Time = DateTime.Now,
                TrendIcon = "4".GetTrendArrowFromDexcom(),
                ErrorResult = true
            };
        }
    }
}
