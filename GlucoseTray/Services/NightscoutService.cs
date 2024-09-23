using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Text.Json;

namespace GlucoseTray.Services;

public interface INightscoutService
{
    Task<GlucoseResult> GetLatestReadingAsync();
}

public class NightscoutService : INightscoutService
{
    private readonly IOptionsMonitor<GlucoseTraySettings> _options;
    private readonly List<string> DebugText = [];
    private readonly ILogger _logger;
    private readonly UrlAssembler _urlBuilder;
    private readonly IExternalCommunicationAdapter _externalAdapter;

    public NightscoutService(IOptionsMonitor<GlucoseTraySettings> options, ILogger<NightscoutService> logger, UrlAssembler urlBuilder, IExternalCommunicationAdapter externalAdapter)
    {
        _options = options;
        _logger = logger;
        _urlBuilder = urlBuilder;
        _externalAdapter = externalAdapter;
    }

    public async Task<GlucoseResult> GetLatestReadingAsync()
    {
        DebugText.Clear();
        DebugText.Add("Starting Nightscout Fetch");
        DebugText.Add(!string.IsNullOrWhiteSpace(_options.CurrentValue.AccessToken) ? "Using access token." : "No access token.");

        GlucoseResult result = new();

        try
        {
            var response = await GetApiResponse();

            DebugText.Add("Attempting to deserialize");
            var record = JsonSerializer.Deserialize<List<NightScoutResult>>(response)!.Last();
            DebugText.Add("Deserialized.");

            result = MapToResult(record);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Nightscout fetching failed or received incorrect format.");
            if (_options.CurrentValue.IsDebugMode)
                DebugService.ShowDebugAlert(ex, "Nightscout result fetch", string.Join(Environment.NewLine, DebugText));
        }

        return result;
    }

    private GlucoseResult MapToResult(NightScoutResult data)
    {
        GlucoseResult result = new()
        {
            Source = FetchMethod.NightscoutApi,
            DateTimeUTC = !string.IsNullOrEmpty(data.DateString) ? DateTime.Parse(data.DateString).ToUniversalTime() : DateTimeOffset.FromUnixTimeMilliseconds(data.Date).UtcDateTime,
            Trend = data.Direction.GetTrend()
        };

        GlucoseMath.CalculateValues(result, data.Sgv, _options.CurrentValue);

        if (result.Trend == TrendResult.Unknown)
            _logger.LogWarning("Un-expected value for direction/Trend {Direction}", data.Direction);

        return result;
    }

    private async Task<string> GetApiResponse()
    {
        var url = _urlBuilder.BuildNightscoutUrl();
        var result = await _externalAdapter.GetApiResponseAsync(url);
        return result;
    }
}
