using GlucoseTray.Enums;
using GlucoseTray.Extensions;
using GlucoseTray.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GlucoseTray.Services
{
    public interface IGlucoseFetchService
    {
        Task<GlucoseResult> GetLatestReadings();
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

        public async Task<GlucoseResult> GetLatestReadings()
        {
            var fetchResult = new GlucoseResult();
            try
            {
                switch (_options.CurrentValue.FetchMethod)
                {
                    case FetchMethod.DexcomShare:
                        fetchResult = await GetFetchResultFromDexcom();
                        break;
                    case FetchMethod.NightscoutApi:
                        fetchResult = await GetResultsFromNightscout();
                        break;
                    default:
                        _logger.LogError("Invalid fetch method specified.");
                        throw new InvalidOperationException("Fetch Method either not specified or invalid specification.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get data.");
            }

            return fetchResult;
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

        private async Task<GlucoseResult> GetResultsFromNightscout()
        {
            var url = $"{_options.CurrentValue.NightscoutUrl?.TrimEnd('/')}/api/v1/entries/sgv?count=1";

            url += !string.IsNullOrWhiteSpace(_options.CurrentValue.AccessToken) ? $"&token={_options.CurrentValue.AccessToken}" : string.Empty;

            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(url));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            GlucoseResult results = new();
            var client = _httpClientFactory.CreateClient();
            try
            {
                var response = await client.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();
                var record = JsonSerializer.Deserialize<List<NightScoutResult>>(result).Last();
                var fetchResult = new GlucoseResult
                {
                    Source = FetchMethod.NightscoutApi,
                    DateTimeUTC = !string.IsNullOrEmpty(record.DateString) ?
                                    DateTime.Parse(record.DateString).ToUniversalTime() :
                                    DateTimeOffset.FromUnixTimeMilliseconds(record.Date).UtcDateTime,
                    Trend = record.Direction.GetTrend(),
                };
                CalculateValues(fetchResult, record.Sgv);
                if (fetchResult.Trend == TrendResult.Unknown)
                    _logger.LogWarning($"Un-expected value for direction/Trend {record.Direction}");
                results = fetchResult;

                response.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Nightscout fetching failed or received incorrect format.");
                if (_options.CurrentValue.IsDebugMode)
                    DebugService.ShowDebugAlert(ex, "Nightscout result fetch");
            }
            finally
            {
                request.Dispose();
            }

            return results;
        }

        private async Task<GlucoseResult> GetFetchResultFromDexcom()
        {
            var host = _options.CurrentValue.DexcomServer switch
            {
                DexcomServerLocation.DexcomShare1 => "share1.dexcom.com",
                DexcomServerLocation.DexcomShare2 => "share2.dexcom.com",
                DexcomServerLocation.DexcomInternational => "shareous1.dexcom.com",
                _ => "share1.dexcom.com",
            };

            var client = _httpClientFactory.CreateClient();
            string accountId = string.Empty;

            // Get Account Id
            var accountIdRequestJson = JsonSerializer.Serialize(new
            {
                accountName = _options.CurrentValue.DexcomUsername,
                applicationId = "d8665ade-9673-4e27-9ff6-92db4ce13d13",
                password = _options.CurrentValue.DexcomPassword
            });
            var accountIdRequest = new HttpRequestMessage(HttpMethod.Post, new Uri($"https://{host}/ShareWebServices/Services/General/AuthenticatePublisherAccount"))
            {
                Content = new StringContent(accountIdRequestJson, Encoding.UTF8, "application/json")
            };

            try
            {
                var response = await client.SendAsync(accountIdRequest);
                accountId = (await response.Content.ReadAsStringAsync()).Replace("\"", "");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Issue getting account id");
                if (_options.CurrentValue.IsDebugMode)
                    DebugService.ShowDebugAlert(ex, "DexCom account id fetch");
                throw;
            }
            finally
            {
                accountIdRequest.Dispose();
            }

            // Get Session Id
            var sessionIdRequestJson = JsonSerializer.Serialize(new
            {
                accountId = accountId,
                accountName = _options.CurrentValue.DexcomUsername,
                applicationId = "d8665ade-9673-4e27-9ff6-92db4ce13d13",
                password = _options.CurrentValue.DexcomPassword
            });
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri($"https://{host}/ShareWebServices/Services/General/LoginPublisherAccountByName"))
            {
                Content = new StringContent(sessionIdRequestJson, Encoding.UTF8, "application/json")
            };

            GlucoseResult fetchResult = new();
            try
            {
                var response = await client.SendAsync(request);
                var sessionId = (await response.Content.ReadAsStringAsync()).Replace("\"", "");
                request = new HttpRequestMessage(HttpMethod.Post, new Uri($"https://{host}/ShareWebServices/Services/Publisher/ReadPublisherLatestGlucoseValues?sessionId={sessionId}&minutes=1440&maxCount=1"));
                var initialResult = await client.SendAsync(request);
                var stringResult = await initialResult.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<DexcomResult>>(stringResult).First();

                var unixTime = string.Join("", result.ST.Where(char.IsDigit));
                var trend = result.Trend;

                CalculateValues(fetchResult, result.Value);
                fetchResult.DateTimeUTC = !string.IsNullOrWhiteSpace(unixTime) ? DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(unixTime)).UtcDateTime : DateTime.MinValue;
                fetchResult.Trend = trend.GetTrend();
                response.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dexcom fetching failed or received incorrect format.");
                if (_options.CurrentValue.IsDebugMode)
                    DebugService.ShowDebugAlert(ex, "DexCom result fetch");
                fetchResult = GetDefaultFetchResult();
            }
            finally
            {
                request.Dispose();
            }

            fetchResult.Source = FetchMethod.DexcomShare;
            return fetchResult;
        }

        private static GlucoseResult GetDefaultFetchResult() => new()
        {
            MmolValue = 0,
            MgValue = 0,
            DateTimeUTC = DateTime.Now.ToUniversalTime(),
            Trend = TrendResult.Unknown,
            WasError = true
        };
    }
}