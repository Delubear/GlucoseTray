using GlucoseTrayCore.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GlucoseTrayCore.Extensions;
using GlucoseTrayCore.Models;

namespace GlucoseTrayCore.Services
{
    public interface IGlucoseFetchService
    {
        Task<GlucoseResult> GetLatestReading();
    }

    public class GlucoseFetchService : IGlucoseFetchService
    {
        private readonly GlucoseTraySettings _options;
        private readonly ILogger<IGlucoseFetchService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public GlucoseFetchService(IOptions<GlucoseTraySettings> options, ILogger<IGlucoseFetchService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _options = options.Value;
            _httpClientFactory = httpClientFactory;
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

                fetchResult.IsCriticalLow = IsCriticalLow(fetchResult);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get data. {0}", ex);
                fetchResult = GetDefaultFetchResult();
            }

            return fetchResult;
        }

        private bool IsCriticalLow(GlucoseResult result) => (_options.GlucoseUnit == GlucoseUnitType.MMOL && result.MmolValue <= _options.CriticalLowBg) || (_options.GlucoseUnit == GlucoseUnitType.MG && result.MgValue <= _options.CriticalLowBg);

        private void CalculateValues(GlucoseResult result, double value)
        {
            // 25 MMOL is > 540 MG, most readers wont go below 40 or above 400.
            // Catch any full value MMOL readings that may be perfect integers that are delivered without a '.', i.e., 7.0 coming in as just 7
            if (value.ToString().Contains(".") || value <= 30) 
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

            var client = _httpClientFactory.CreateClient();
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
                request.Dispose();
            }

            fetchResult.Source = FetchMethod.NightscoutApi;
            return fetchResult;
        }

        private async Task<GlucoseResult> GetFetchResultFromDexcom(GlucoseResult fetchResult)
        {
            var host = _options.DexcomServer switch
            {
                DexcomServerLocation.DexcomShare1 => "share1.dexcom.com",
                DexcomServerLocation.DexcomShare2 => "share2.dexcom.com",
                DexcomServerLocation.DexcomInternational => "shareous1.dexcom.com",
                _ => "share1.dexcom.com",
            };

            // Get Session Id
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri($"https://{host}/ShareWebServices/Services/General/LoginPublisherAccountByName"))
            {
                Content = new StringContent("{\"accountName\":\"" + _options.DexcomUsername + "\"," +
                                                 "\"applicationId\":\"d8665ade-9673-4e27-9ff6-92db4ce13d13\"," +
                                                 "\"password\":\"" + _options.DexcomPassword + "\"}", Encoding.UTF8, "application/json")
            };

            var client = _httpClientFactory.CreateClient();
            try
            {
                var response = await client.SendAsync(request).ConfigureAwait(false);
                var sessionId = (await response.Content.ReadAsStringAsync().ConfigureAwait(false)).Replace("\"", "");
                request = new HttpRequestMessage(HttpMethod.Post, new Uri($"https://{host}/ShareWebServices/Services/Publisher/ReadPublisherLatestGlucoseValues?sessionId={sessionId}&minutes=1440&maxCount=1"));
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