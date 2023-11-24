using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GlucoseTray.Services;

public interface INightscoutService
{
    Task<GlucoseResult> GetLatestReadingAsync();
}

public class NightscoutService : INightscoutService
{
    private readonly IOptionsMonitor<GlucoseTraySettings> _options;
    private readonly List<string> DebugText = new();
    private readonly ILogger _logger;
    private readonly UrlAssembler _urlBuilder;
    private readonly IHttpClientFactory _httpClientFactory;

    public NightscoutService(IOptionsMonitor<GlucoseTraySettings> options, ILogger<NightscoutService> logger, UrlAssembler urlBuilder, IHttpClientFactory httpClientFactory)
    {
        _options = options;
        _logger = logger;
        _urlBuilder = urlBuilder;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<GlucoseResult> GetLatestReadingAsync()
    {
        DebugText.Clear();
        DebugText.Add("Starting Nightscout Fetch");
        DebugText.Add(!string.IsNullOrWhiteSpace(_options.CurrentValue.AccessToken) ? "Using access token." : "No access token.");

        var url = _urlBuilder.BuildNightscoutUrl();
        var request = new HttpRequestMessage(HttpMethod.Get, new Uri(url));
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        GlucoseResult fetchResult = new();
        var client = _httpClientFactory.CreateClient();
        try
        {
            var response = await client.SendAsync(request);
            DebugText.Add("Sending request.  Status code response: " + response.StatusCode);
            var result = await response.Content.ReadAsStringAsync();
            DebugText.Add("Result: " + result);
            DebugText.Add("Attempting to deserialize");
            var record = JsonSerializer.Deserialize<List<NightScoutResult>>(result)!.Last();
            DebugText.Add("Deserialized.");
            fetchResult.Source = FetchMethod.NightscoutApi;
            fetchResult.DateTimeUTC = !string.IsNullOrEmpty(record.DateString) ? DateTime.Parse(record.DateString).ToUniversalTime() : DateTimeOffset.FromUnixTimeMilliseconds(record.Date).UtcDateTime;
            fetchResult.Trend = record.Direction.GetTrend();
            GlucoseMath.CalculateValues(fetchResult, record.Sgv, _options.CurrentValue);
            if (fetchResult.Trend == TrendResult.Unknown)
                _logger.LogWarning($"Un-expected value for direction/Trend {record.Direction}");

            response.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Nightscout fetching failed or received incorrect format.");
            if (_options.CurrentValue.IsDebugMode)
                DebugService.ShowDebugAlert(ex, "Nightscout result fetch", string.Join(Environment.NewLine, DebugText));
        }
        finally
        {
            request.Dispose();
        }

        return fetchResult;
    }
}
