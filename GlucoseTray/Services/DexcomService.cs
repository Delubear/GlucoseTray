using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Text.Json;

namespace GlucoseTray.Services;

public interface IDexcomService
{
    Task<GlucoseResult> GetLatestReadingAsync();
}

public class DexcomService : IDexcomService
{
    private readonly IOptionsMonitor<GlucoseTraySettings> _options;
    private readonly List<string> DebugText = new();
    private readonly ILogger _logger;
    private readonly UrlAssembler _urlBuilder;
    private readonly IExternalCommunicationAdapter _externalAdapter;

    public DexcomService(IOptionsMonitor<GlucoseTraySettings> options, ILogger<DexcomService> logger, UrlAssembler urlBuilder, IExternalCommunicationAdapter externalAdapter)
    {
        _options = options;
        _logger = logger;
        _urlBuilder = urlBuilder;
        _externalAdapter = externalAdapter;
    }

    public async Task<GlucoseResult> GetLatestReadingAsync()
    {
        DebugText.Clear();
        DebugText.Add("Starting DexCom Fetch");
        DebugText.Add("Server: " + _urlBuilder.GetDexComServer());

        GlucoseResult glucoseResult;

        try
        {
            string accountId = await GetAccountId();
            string sessionId = await GetSessionId(accountId);
            string response = await GetApiResponse(sessionId);

            DebugText.Add("Attempting to deserialize");
            var data = JsonSerializer.Deserialize<List<DexcomResult>>(response)!.First();
            DebugText.Add("Deserialized");

            glucoseResult = MapToResult(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dexcom fetching failed or received incorrect format.");
            if (_options.CurrentValue.IsDebugMode)
                DebugService.ShowDebugAlert(ex, "Dexcom result fetch", string.Join(Environment.NewLine, DebugText));

            glucoseResult = GlucoseResult.Default;
        }

        return glucoseResult;
    }

    private GlucoseResult MapToResult(DexcomResult data)
    {
        GlucoseResult result = new();

        var unixTime = string.Join("", data.ST.Where(char.IsDigit));
        var trend = data.Trend;

        GlucoseMath.CalculateValues(result, data.Value, _options.CurrentValue);
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
            accountId = accountId,
            applicationId = "d8665ade-9673-4e27-9ff6-92db4ce13d13",
            password = _options.CurrentValue.DexcomPassword
        });

        var sessionUrl = _urlBuilder.BuildDexComSessionUrl();

        var result = await _externalAdapter.PostApiResponseAsync(sessionUrl, sessionIdRequestJson);
        var sessionId = result.Replace("\"", "");

        if (sessionId.Any(x => x != '0' && x != '-'))
            DebugText.Add("Got a valid session id");
        else
        {
            DebugText.Add("Invalid session id");
            throw new InvalidOperationException("Invalid session id");
        }

        return sessionId;
    }

    private async Task<string> GetAccountId()
    {
        var accountIdRequestJson = JsonSerializer.Serialize(new
        {
            accountName = _options.CurrentValue.DexcomUsername,
            applicationId = "d8665ade-9673-4e27-9ff6-92db4ce13d13",
            password = _options.CurrentValue.DexcomPassword
        });

        var accountUrl = _urlBuilder.BuildDexComAccountIdUrl();

        var result = await _externalAdapter.PostApiResponseAsync(accountUrl, accountIdRequestJson);
        var accountId = result.Replace("\"", "");

        if (accountId.Any(x => x != '0' && x != '-'))
            DebugText.Add("Got a valid account id");
        else
        {
            DebugText.Add("Invalid account id");
            throw new InvalidOperationException("Invalid account id");
        }

        return accountId;
    }
}
