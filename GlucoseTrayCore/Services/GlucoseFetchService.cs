using Dexcom.Fetch.Enums;
using Dexcom.Fetch.Extensions;
using Dexcom.Fetch.Models;
using GlucoseTrayCore;
using GlucoseTrayCore.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dexcom.Fetch
{
    public interface IGlucoseFetchService
    {
        Task<GlucoseResult> GetLatestReading();
    }

    public class GlucoseFetchService : IGlucoseFetchService
    {
        private readonly GlucoseTraySettings _options;
        private readonly ILogger<IGlucoseFetchService> _logger;
        private readonly string _dexcomShareHost;

        public GlucoseFetchService(IOptions<GlucoseTraySettings> options, ILogger<IGlucoseFetchService> logger)
        {
            _logger = logger;
            _options = options.Value;

            switch (_options.DexcomServer)
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

        public async Task<GlucoseResult> GetLatestReading()
        {
            var fetchResult = new GlucoseResult();

            try
            {
                if (_options.FetchMethod == FetchMethod.DexcomShare)
                    await GetFetchResultFromDexcom(fetchResult).ConfigureAwait(false);
                else if (_options.FetchMethod == FetchMethod.NightscoutApi)
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

            return fetchResult;
        }

        private void CalculateValues(GlucoseResult result, double value)
        {
            if (value.ToString().Contains("."))
            {
                result.MmolValue = value;
                result.MgValue = Convert.ToInt32(value *= 18);
            }
            else
            {
                result.MgValue = Convert.ToInt32(value);
                result.MmolValue = value /= 18;
            }
        }

        private async Task<GlucoseResult> GetFetchResultFromNightscout(GlucoseResult fetchResult)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_options.NightscoutUrl}/api/v1/entries/sgv?count=1" + (!string.IsNullOrWhiteSpace(_options.AccessToken) ? $"&token={_options.AccessToken}" : "")));
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var client = new HttpClient();
            try
            {
                var response = await client.SendAsync(request).ConfigureAwait(false);
                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var content = JsonConvert.DeserializeObject<List<NightScoutResult>>(result).FirstOrDefault();
                CalculateValues(fetchResult, content.sgv);
                fetchResult.DateTimeUTC = DateTime.Parse(content.dateString).ToUniversalTime();
                fetchResult.Trend = content.direction.GetTrend();
                if (fetchResult.Trend == TrendResult.Unknown)
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

            fetchResult.Source = FetchMethod.NightscoutApi;
            return fetchResult;
        }

        private async Task<GlucoseResult> GetFetchResultFromDexcom(GlucoseResult fetchResult)
        {
            // Get Session Id
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri($"https://{_dexcomShareHost}/ShareWebServices/Services/General/LoginPublisherAccountByName"))
            {
                Content = new StringContent("{\"accountName\":\"" + _options.DexcomUsername + "\"," +
                                                 "\"applicationId\":\"d8665ade-9673-4e27-9ff6-92db4ce13d13\"," +
                                                 "\"password\":\"" + _options.DexcomPassword + "\"}", Encoding.UTF8, "application/json")
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

                CalculateValues(fetchResult, result.Value);
                fetchResult.DateTimeUTC = !string.IsNullOrWhiteSpace(unixTime) ? DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(unixTime)).UtcDateTime : DateTime.MinValue;
                fetchResult.Trend = (TrendResult)trend;
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

            fetchResult.Source = FetchMethod.DexcomShare;
            return fetchResult;
        }

        private GlucoseResult GetDefaultFetchResult() => new GlucoseResult
        {
            MmolValue = 0,
            MgValue = 0,
            DateTimeUTC = DateTime.Now.ToUniversalTime(),
            Trend = TrendResult.Unknown,
            WasError = true
        };
    }
}