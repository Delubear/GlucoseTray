using GlucoseTray.Domain.DisplayResults;
using GlucoseTray.Domain.Enums;
using GlucoseTray.Domain.GlucoseSettings;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GlucoseTray.Domain.FetchResults.Nightscout;

public interface INightscoutService
{
    Task GetLatestReadingAsync();
}

public class NightscoutService(ISettingsProxy settings, ILogger<NightscoutService> logger, UrlAssembler urlBuilder, IExternalCommunicationAdapter externalAdapter, DebugService debug, GlucoseResult glucoseResult) : INightscoutService
{
    public async Task GetLatestReadingAsync()
    {
        debug.ClearDebugText();
        debug.AddDebugText("Starting Nightscout Fetch");
        debug.AddDebugText(!string.IsNullOrWhiteSpace(settings.AccessToken) ? "Using access token." : "No access token.");

        try
        {
            var response = await GetApiResponse();

            debug.AddDebugText("Attempting to deserialize");
            var record = JsonSerializer.Deserialize<List<NightScoutResult>>(response)!.Last();
            debug.AddDebugText("Deserialized.");

            MapToResult(record);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Nightscout fetching failed or received incorrect format.");
            if (settings.IsDebugMode)
                debug.ShowDebugAlert(ex, "Nightscout result fetch");

            glucoseResult.SetDefault();
        }
    }

    private void MapToResult(NightScoutResult data)
    {
        glucoseResult.SetDateTimeUtc(!string.IsNullOrEmpty(data.DateString) ? DateTime.Parse(data.DateString).ToUniversalTime() : DateTimeOffset.FromUnixTimeMilliseconds(data.Date).UtcDateTime);
        glucoseResult.SetTrend(data.Direction.GetTrend());
        glucoseResult.SetGlucoseValues(data.Sgv);

        if (glucoseResult.Trend == TrendResult.Unknown)
            logger.LogWarning("Un-expected value for direction/Trend {Direction}", data.Direction);
    }

    private async Task<string> GetApiResponse()
    {
        var url = urlBuilder.BuildNightscoutUrl();
        var result = await externalAdapter.GetApiResponseAsync(url);
        return result;
    }
}
