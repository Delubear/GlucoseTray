using GlucoseTray.Domain.DisplayResults;
using GlucoseTray.Domain.Enums;
using GlucoseTray.Domain.GlucoseSettings;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GlucoseTray.Domain.FetchResults.Nightscout;

public interface INightscoutService
{
    Task<GlucoseResult> GetLatestReadingAsync();
}

public class NightscoutService(ISettingsProxy settings, ILogger<NightscoutService> logger, UrlAssembler urlBuilder, IExternalCommunicationAdapter externalAdapter, DebugService debug) : INightscoutService
{
    private readonly ISettingsProxy _settings = settings;
    private readonly ILogger _logger = logger;
    private readonly UrlAssembler _urlBuilder = urlBuilder;
    private readonly IExternalCommunicationAdapter _externalAdapter = externalAdapter;
    private readonly DebugService _debug = debug;

    public async Task<GlucoseResult> GetLatestReadingAsync()
    {
        _debug.ClearDebugText();
        _debug.AddDebugText("Starting Nightscout Fetch");
        _debug.AddDebugText(!string.IsNullOrWhiteSpace(_settings.AccessToken) ? "Using access token." : "No access token.");

        GlucoseResult result = new();

        try
        {
            var response = await GetApiResponse();

            _debug.AddDebugText("Attempting to deserialize");
            var record = JsonSerializer.Deserialize<List<NightScoutResult>>(response)!.Last();
            _debug.AddDebugText("Deserialized.");

            result = MapToResult(record);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Nightscout fetching failed or received incorrect format.");
            if (_settings.IsDebugMode)
                _debug.ShowDebugAlert(ex, "Nightscout result fetch");
        }

        return result;
    }

    private GlucoseResult MapToResult(NightScoutResult data)
    {
        GlucoseResult result = new();

        result.SetDateTimeUtc(!string.IsNullOrEmpty(data.DateString) ? DateTime.Parse(data.DateString).ToUniversalTime() : DateTimeOffset.FromUnixTimeMilliseconds(data.Date).UtcDateTime);
        result.SetTrend(data.Direction.GetTrend());
        result.SetGlucoseValues(data.Sgv, _settings);

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
