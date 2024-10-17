using GlucoseTray.Domain.DisplayResults;
using GlucoseTray.Domain.GlucoseSettings;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GlucoseTray.Domain.FetchResults.Dexcom;

public interface IDexcomService
{
    Task GetLatestReadingAsync();
}

public class DexcomService(ISettingsProxy settings, ILogger<DexcomService> logger, UrlAssembler urlBuilder, IExternalCommunicationAdapter externalAdapter, DebugService debug, GlucoseResult glucoseResult) : IDexcomService
{
    public async Task GetLatestReadingAsync()
    {
        debug.ClearDebugText();
        debug.AddDebugText("Starting DexCom Fetch");
        debug.AddDebugText("Server: " + urlBuilder.GetDexComServer());

        try
        {
            string accountId = await GetAccountId();
            string sessionId = await GetSessionId(accountId);
            string response = await GetApiResponse(sessionId);

            debug.AddDebugText("Attempting to deserialize");
            var data = JsonSerializer.Deserialize<List<DexcomResult>>(response)!.First();
            debug.AddDebugText("Deserialized");

            MapToResult(data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Dexcom fetching failed or received incorrect format.");
            if (settings.IsDebugMode)
                debug.ShowDebugAlert(ex, "Dexcom result fetch");

            glucoseResult.SetDefault();
        }
    }

    private void MapToResult(DexcomResult data)
    {
        var unixTime = string.Join("", data.ST.Where(char.IsDigit));
        var trend = data.Trend;

        glucoseResult.SetGlucoseValues(data.Value);
        glucoseResult.SetDateTimeUtc(!string.IsNullOrWhiteSpace(unixTime) ? DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(unixTime)).UtcDateTime : DateTime.MinValue);
        glucoseResult.SetTrend(trend.GetTrend());
    }

    private async Task<string> GetApiResponse(string sessionId)
    {
        var url = urlBuilder.BuildDexComGlucoseValueUrl(sessionId);
        var result = await externalAdapter.PostApiResponseAsync(url);
        return result;
    }

    private async Task<string> GetSessionId(string accountId)
    {
        var sessionIdRequestJson = JsonSerializer.Serialize(new
        {
            accountId,
            applicationId = "d8665ade-9673-4e27-9ff6-92db4ce13d13",
            password = settings.DexcomPassword
        });

        var sessionUrl = urlBuilder.BuildDexComSessionUrl();

        var result = await externalAdapter.PostApiResponseAsync(sessionUrl, sessionIdRequestJson);
        var sessionId = result.Replace("\"", "");

        if (IsValidId(sessionId))
            debug.AddDebugText("Got a valid session id");
        else
        {
            debug.AddDebugText("Invalid session id");
            throw new InvalidOperationException("Invalid session id");
        }

        return sessionId;
    }

    private async Task<string> GetAccountId()
    {
        var accountIdRequestJson = JsonSerializer.Serialize(new
        {
            accountName = settings.DexcomUsername,
            applicationId = "d8665ade-9673-4e27-9ff6-92db4ce13d13",
            password = settings.DexcomPassword
        });

        var accountUrl = urlBuilder.BuildDexComAccountIdUrl();

        var result = await externalAdapter.PostApiResponseAsync(accountUrl, accountIdRequestJson);
        var accountId = result.Replace("\"", "");

        if (IsValidId(accountId))
            debug.AddDebugText("Got a valid account id");
        else
        {
            debug.AddDebugText("Invalid account id");
            throw new InvalidOperationException("Invalid account id");
        }

        return accountId;
    }

    private static bool IsValidId(string id) => id.Any(x => x != '0' && x != '-');
}
