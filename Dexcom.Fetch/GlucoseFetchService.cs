using Dexcom.Fetch.Enums;
using Dexcom.Fetch.Extensions;
using Dexcom.Fetch.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

        public async Task<GlucoseFetchResult> GetLatestReading()
        {
            var fetchResult = new GlucoseFetchResult();

            try
            {
                if (_config.FetchMethod == FetchMethod.DexcomShare)
                    await GetFetchResultFromDexcom(fetchResult);
                else if (_config.FetchMethod == FetchMethod.NightscoutApi)
                    await GetFetchResultFromNightscout(fetchResult);
                else
                {
                    _logger.LogError("Invalid fetch method specified.");
                    throw new InvalidOperationException("Fetch Method either not specified or invalid specification.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get data. {0}", ex);
                fetchResult = GetDefaultFetchResult();
            }

            fetchResult.UnitDisplayType = _config.UnitDisplayType;
            if (fetchResult.UnitDisplayType == GlucoseUnitType.MMOL && !fetchResult.Value.ToString().Contains(".")) // If decimal value, then it is already MMOL
                fetchResult.Value /= 18;

            return fetchResult;
        }

        private async Task<GlucoseFetchResult> GetFetchResultFromNightscout(GlucoseFetchResult fetchResult)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_config.NightscoutUrl}/api/v1/entries/sgv?count=1" + (!string.IsNullOrWhiteSpace(_config.NightscoutAccessToken) ? $"&token={_config.NightscoutAccessToken}" : "")));

            var client = new HttpClient();
            try
            {
                var response = await client.SendAsync(request);
                var content = (await response.Content.ReadAsStringAsync()).Split('\t');
                Regex rgx = new Regex("[^a-zA-Z]");
                fetchResult.Value = int.Parse(content[2]);
                fetchResult.Time = content[0].ParseNightscoutDate();
                fetchResult.TrendIcon = rgx.Replace(content[3], "").GetTrendArrowFromNightscout();
                response.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError("Nightscout fetching failed or received incorrect format. {0}", ex);
                fetchResult = GetDefaultFetchResult();
            }
            finally
            {
                client.Dispose();
                request.Dispose();
            }

            return fetchResult;
        }

        private async Task<GlucoseFetchResult> GetFetchResultFromDexcom(GlucoseFetchResult fetchResult)
        {
            // Get Session Id
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri("https://share1.dexcom.com/ShareWebServices/Services/General/LoginPublisherAccountByName"))
            {
                Content = new StringContent("{\"accountName\":\"" + _config.DexcomUsername + "\"," +
                                                 "\"applicationId\":\"d8665ade-9673-4e27-9ff6-92db4ce13d13\"," +
                                                 "\"password\":\"" + _config.DexcomPassword + "\"}", Encoding.UTF8, "application/json")
            };

            var client = new HttpClient();
            try
            {
                var response = await client.SendAsync(request);
                var sessionId = (await response.Content.ReadAsStringAsync()).Replace("\"", "");
                request = new HttpRequestMessage(HttpMethod.Post, new Uri($"https://share1.dexcom.com/ShareWebServices/Services/Publisher/ReadPublisherLatestGlucoseValues?sessionId={sessionId}&minutes=1440&maxCount=1"));
                var result = (await (await client.SendAsync(request)).Content.ReadAsStringAsync()).RemoveUnnecessaryCharacters().Split(',');

                // Get raw data seperated
                var unixTime = result[1].Split('e')[1];
                var trend = result[2].Split(':')[1];
                var stringVal = result[3].Split(':')[1];

                int.TryParse(stringVal, out int val);

                fetchResult.Value = val;
                fetchResult.Time = !string.IsNullOrWhiteSpace(unixTime) ? DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(unixTime)).LocalDateTime : DateTime.MinValue;
                fetchResult.TrendIcon = trend.GetTrendArrowFromDexcom();
                response.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError("Dexcom fetching failed or received incorrect format. {0}", ex);
                fetchResult = GetDefaultFetchResult();
            }
            finally
            {
                client.Dispose();
                request.Dispose();
            }

            return fetchResult;
        }

        private GlucoseFetchResult GetDefaultFetchResult() => new GlucoseFetchResult
        {
            Value = 0,
            Time = DateTime.Now,
            TrendIcon = "4".GetTrendArrowFromDexcom(),
            ErrorResult = true
        };
    }
}