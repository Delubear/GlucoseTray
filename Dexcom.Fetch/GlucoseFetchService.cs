using Dexcom.Fetch.Enums;
using Dexcom.Fetch.Extensions;
using Dexcom.Fetch.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dexcom.Fetch
{
    public class GlucoseFetchService
    {
        private readonly GlucoseFetchConfiguration _config;
        private readonly ILogger _logger;
        private readonly string _dexcomShareHost;

        public GlucoseFetchService(GlucoseFetchConfiguration config, ILogger logger)
        {
            _config = config;
            _logger = logger;

            switch (_config.DexcomServer)
            {
                case DexcomServerLocation.DexcomShare1:
                    _dexcomShareHost = "share1.dexcom.com";
                    break;
                case DexcomServerLocation.DexcomShare2:
                    _dexcomShareHost = "share2.dexcom.com";
                    break;
                case DexcomServerLocation.DexcomInternational:
                    _dexcomShareHost = "shareous1.dexcom.com";
                    break;
                default:
                    _dexcomShareHost = "share1.dexcom.com";
                    break;
            }
        }

        public async Task<GlucoseFetchResult> GetLatestReading()
        {
            var fetchResult = new GlucoseFetchResult();

            try
            {
                if (_config.FetchMethod == FetchMethod.DexcomShare)
                    await GetFetchResultFromDexcom(fetchResult).ConfigureAwait(false);
                else if (_config.FetchMethod == FetchMethod.NightscoutApi)
                    await GetFetchResultFromNightscout(fetchResult).ConfigureAwait(false);
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
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var client = new HttpClient();
            try
            {
                var response = await client.SendAsync(request).ConfigureAwait(false);
                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var content = JsonConvert.DeserializeObject<List<NightScoutResult>>(result).FirstOrDefault();
                fetchResult.Value = content.sgv;
                fetchResult.Time = DateTime.Parse(content.dateString);
                fetchResult.TrendIcon = content.direction.GetTrendArrow();
                if (fetchResult.TrendIcon.Length > 1)
                    _logger.LogWarning($"Un-expected value for direction/Trend {content.direction}");

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
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri($"https://{_dexcomShareHost}/ShareWebServices/Services/General/LoginPublisherAccountByName"))
            {
                Content = new StringContent("{\"accountName\":\"" + _config.DexcomUsername + "\"," +
                                                 "\"applicationId\":\"d8665ade-9673-4e27-9ff6-92db4ce13d13\"," +
                                                 "\"password\":\"" + _config.DexcomPassword + "\"}", Encoding.UTF8, "application/json")
            };

            var client = new HttpClient();
            try
            {
                var response = await client.SendAsync(request).ConfigureAwait(false);
                var sessionId = (await response.Content.ReadAsStringAsync().ConfigureAwait(false)).Replace("\"", "");
                request = new HttpRequestMessage(HttpMethod.Post, new Uri($"https://{_dexcomShareHost}/ShareWebServices/Services/Publisher/ReadPublisherLatestGlucoseValues?sessionId={sessionId}&minutes=1440&maxCount=1"));
                var result = JsonConvert.DeserializeObject<List<DexcomResult>>(await (await client.SendAsync(request).ConfigureAwait(false)).Content.ReadAsStringAsync().ConfigureAwait(false)).First();

                var unixTime = string.Join("", result.ST.Where(char.IsDigit));
                var trend = result.Trend;

                fetchResult.Value = result.Value;
                fetchResult.Time = !string.IsNullOrWhiteSpace(unixTime) ? DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(unixTime)).LocalDateTime : DateTime.MinValue;
                fetchResult.TrendIcon = trend.GetTrendArrow();
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
            TrendIcon = "",
            ErrorResult = true
        };
    }
}