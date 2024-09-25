using GlucoseTray.Domain.DisplayResults;
using GlucoseTray.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text.Json;

namespace GlucoseTray.Domain.FetchResults;

public interface IDexcomService
{
    Task<GlucoseResult> GetLatestReadingAsync();
}

public class DexcomService : IDexcomService
{
    private readonly ISettingsProxy _options;
    private readonly ILogger _logger;
    private readonly UrlAssembler _urlBuilder;
    private readonly IExternalCommunicationAdapter _externalAdapter;
    private readonly DebugService _debug;

    public DexcomService(ISettingsProxy options, ILogger<DexcomService> logger, UrlAssembler urlBuilder, IExternalCommunicationAdapter externalAdapter, DebugService debug)
    {
        _options = options;
        _logger = logger;
        _urlBuilder = urlBuilder;
        _externalAdapter = externalAdapter;
        _debug = debug;
    }

    public async Task<GlucoseResult> GetLatestReadingAsync()
    {
        _debug.ClearDebugText();
        _debug.AddDebugText("Starting DexCom Fetch");
        _debug.AddDebugText("Server: " + _urlBuilder.GetDexComServer());

        GlucoseResult glucoseResult;

        try
        {
            string accountId = await GetAccountId();
            string sessionId = await GetSessionId(accountId);
            string response = await GetApiResponse(sessionId);

            _debug.AddDebugText("Attempting to deserialize");
            var data = JsonSerializer.Deserialize<List<DexcomResult>>(response)!.First();
            _debug.AddDebugText("Deserialized");

            glucoseResult = MapToResult(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dexcom fetching failed or received incorrect format.");
            if (_options.IsDebugMode)
                _debug.ShowDebugAlert(ex, "Dexcom result fetch");

            glucoseResult = GlucoseResult.Default;
        }

        return glucoseResult;
    }

    private GlucoseResult MapToResult(DexcomResult data)
    {
        GlucoseResult result = new();

        var unixTime = string.Join("", data.ST.Where(char.IsDigit));
        var trend = data.Trend;

        GlucoseMath.CalculateValues(result, data.Value, _options);
        result.DateTimeUTC = !string.IsNullOrWhiteSpace(unixTime) ? DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(unixTime)).UtcDateTime : DateTime.MinValue;
        result.Trend = trend.GetTrend();
        result.Source = FetchMethod.DexcomShare;
        return result;
    }

    private async Task<string> GetApiResponse(string sessionId)
    {
        var url = _urlBuilder.BuildDexComGlucoseValueUrl(sessionId);
        var result = await _externalAdapter.PostApiResponseAsync(url);
        return result;
    }

    private async Task<string> GetSessionId(string accountId)
    {
        var sessionIdRequestJson = JsonSerializer.Serialize(new
        {
            accountId,
            applicationId = "d8665ade-9673-4e27-9ff6-92db4ce13d13",
            password = _options.DexcomPassword
        });

        var sessionUrl = _urlBuilder.BuildDexComSessionUrl();

        var result = await _externalAdapter.PostApiResponseAsync(sessionUrl, sessionIdRequestJson);
        var sessionId = result.Replace("\"", "");

        if (IsValidId(sessionId))
            _debug.AddDebugText("Got a valid session id");
        else
        {
            _debug.AddDebugText("Invalid session id");
            throw new InvalidOperationException("Invalid session id");
        }

        return sessionId;
    }

    private async Task<string> GetAccountId()
    {
        var accountIdRequestJson = JsonSerializer.Serialize(new
        {
            accountName = _options.DexcomUsername,
            applicationId = "d8665ade-9673-4e27-9ff6-92db4ce13d13",
            password = _options.DexcomPassword
        });

        var accountUrl = _urlBuilder.BuildDexComAccountIdUrl();

        var result = await _externalAdapter.PostApiResponseAsync(accountUrl, accountIdRequestJson);
        var accountId = result.Replace("\"", "");

        if (IsValidId(accountId))
            _debug.AddDebugText("Got a valid account id");
        else
        {
            _debug.AddDebugText("Invalid account id");
            throw new InvalidOperationException("Invalid account id");
        }

        return accountId;
    }

    private static bool IsValidId(string id) => id.Any(x => x != '0' && x != '-');
}
