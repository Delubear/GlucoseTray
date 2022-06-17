using GlucoseTray.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GlucoseTray.Extensions;
using GlucoseTray.Models;
using System.Text.Json;

namespace GlucoseTray.Services
{
    public interface IGlucoseFetchService
    {
        Task<List<GlucoseResult>> GetLatestReadings(DateTime? timeOflastGoodResult);
    }

    public class GlucoseFetchService : IGlucoseFetchService
    {
        private readonly IOptionsMonitor<GlucoseTraySettings> _options;
        private readonly ILogger<IGlucoseFetchService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public GlucoseFetchService(IOptionsMonitor<GlucoseTraySettings> options, ILogger<IGlucoseFetchService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _options = options;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<GlucoseResult>> GetLatestReadings(DateTime? timeOflastGoodResult)
        {
            var fetchResult = new GlucoseResult();

            var results = new List<GlucoseResult>();
            try
            {
                if (_options.CurrentValue.FetchMethod == FetchMethod.DexcomShare)
                {
                    await GetFetchResultFromDexcom(fetchResult).ConfigureAwait(false);
                    results.Add(fetchResult);
                }
                else if (_options.CurrentValue.FetchMethod == FetchMethod.NightscoutApi)
                {
                    results = await GetResultsFromNightscout(timeOflastGoodResult).ConfigureAwait(false);
                }
                else
                {
                    _logger.LogError("Invalid fetch method specified.");
                    throw new InvalidOperationException("Fetch Method either not specified or invalid specification.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get data. {0}", ex);
            }

            return results;
        }

        private bool IsCriticalLow(GlucoseResult result) =>
            (_options.CurrentValue.GlucoseUnit == GlucoseUnitType.MMOL && result.MmolValue <= _options.CurrentValue.CriticalLowBg)
            || (_options.CurrentValue.GlucoseUnit == GlucoseUnitType.MG && result.MgValue <= _options.CurrentValue.CriticalLowBg);

        private void CalculateValues(GlucoseResult result, double value)
        {
            if (value == 0)
            {
                result.MmolValue = value;
                result.MgValue = Convert.ToInt32(value);
            }
            else if (_options.CurrentValue.IsServerDataUnitTypeMmol)
            {
                result.MmolValue = value;
                result.MgValue = Convert.ToInt32(value * 18);
            }
            else
            {
                result.MmolValue = value / 18;
                result.MgValue = Convert.ToInt32(value);
            }
            result.IsCriticalLow = IsCriticalLow(result);
        }

        private async Task<List<GlucoseResult>> GetResultsFromNightscout(DateTime? timeOflastGoodResult)
        {
            var url = $"{_options.CurrentValue.NightscoutUrl?.TrimEnd('/')}/api/v1/entries/sgv?";
            var count = 1;

            if (timeOflastGoodResult.HasValue)
            {
                count = 1000000; // Without sending a maximum count, Nightscout will only return 10 results.
                var fromDate = timeOflastGoodResult.Value.AddSeconds(1).ToString("s") + "Z";
                var toDate = DateTime.UtcNow.ToString("s") + "Z";
                url += $"find[dateString][$gte]={fromDate}&find[dateString][$lte]={toDate}&";
            }

            url += $"count={count}";
            url += !string.IsNullOrWhiteSpace(_options.CurrentValue.AccessToken) ? $"&token={_options.CurrentValue.AccessToken}" : string.Empty;

            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(url));
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var results = new List<GlucoseResult>();
            var client = _httpClientFactory.CreateClient();
            try
            {
                var response = await client.SendAsync(request).ConfigureAwait(false);
                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var content = JsonSerializer.Deserialize<List<NightScoutResult>>(result).ToList();
                foreach (var record in content)
                {
                    var fetchResult = new GlucoseResult
                    {
                        Source = FetchMethod.NightscoutApi,
                        DateTimeUTC = !String.IsNullOrEmpty(record.DateString) ?
                                        DateTime.Parse(record.DateString).ToUniversalTime() :
                                        DateTimeOffset.FromUnixTimeMilliseconds(record.Date).UtcDateTime,
                        Trend = record.Direction.GetTrend()
                    };
                    CalculateValues(fetchResult, record.Sgv);
                    if (fetchResult.Trend == TrendResult.Unknown)
                        _logger.LogWarning($"Un-expected value for direction/Trend {record.Direction}");
                    results.Add(fetchResult);
                }

                response.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError("Nightscout fetching failed or received incorrect format. {0}", ex);
            }
            finally
            {
                request.Dispose();
            }

            return results.OrderBy(a => a.DateTimeUTC).ToList();
        }

        private async Task<GlucoseResult> GetFetchResultFromDexcom(GlucoseResult fetchResult)
        {
            var host = _options.CurrentValue.DexcomServer switch
            {
                DexcomServerLocation.DexcomShare1 => "share1.dexcom.com",
                DexcomServerLocation.DexcomShare2 => "share2.dexcom.com",
                DexcomServerLocation.DexcomInternational => "shareous1.dexcom.com",
                _ => "share1.dexcom.com",
            };

            // Get Session Id
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri($"https://{host}/ShareWebServices/Services/General/LoginPublisherAccountByName"))
            {
                Content = new StringContent($"{{\"accountName\":\"{_options.CurrentValue.DexcomUsername}\",\"applicationId\":\"d8665ade-9673-4e27-9ff6-92db4ce13d13\",\"password\":\"{_options.CurrentValue.DexcomPassword}\"}}", Encoding.UTF8, "application/json")
            };

            var client = _httpClientFactory.CreateClient();
            try
            {
                var response = await client.SendAsync(request).ConfigureAwait(false);
                var sessionId = (await response.Content.ReadAsStringAsync().ConfigureAwait(false)).Replace("\"", "");
                request = new HttpRequestMessage(HttpMethod.Post, new Uri($"https://{host}/ShareWebServices/Services/Publisher/ReadPublisherLatestGlucoseValues?sessionId={sessionId}&minutes=1440&maxCount=1"));
                var result = JsonSerializer.Deserialize<List<DexcomResult>>(await (await client.SendAsync(request).ConfigureAwait(false)).Content.ReadAsStringAsync().ConfigureAwait(false)).First();

                var unixTime = string.Join("", result.ST.Where(char.IsDigit));
                var trend = result.Trend;

                CalculateValues(fetchResult, result.Value);
                fetchResult.DateTimeUTC = !string.IsNullOrWhiteSpace(unixTime) ? DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(unixTime)).UtcDateTime : DateTime.MinValue;
                fetchResult.Trend = (TrendResult)trend;
                response.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError("Dexcom fetching failed or received incorrect format. " + ex.Message + ex?.InnerException?.Message + "{0}", ex);
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