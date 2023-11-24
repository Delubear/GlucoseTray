using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Net.Http;
using System.Text;
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
    private readonly IHttpClientFactory _httpClientFactory;


    public DexcomService(IOptionsMonitor<GlucoseTraySettings> options, ILogger<DexcomService> logger, UrlAssembler urlBuilder, IHttpClientFactory httpClientFactory)
    {
        _options = options;
        _logger = logger;
        _urlBuilder = urlBuilder;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<GlucoseResult> GetLatestReadingAsync()
    {
        DebugText.Clear();
        DebugText.Add("Starting DexCom Fetch");
        DebugText.Add("Server: " + _urlBuilder.GetDexComServer());

        var client = _httpClientFactory.CreateClient();
        string accountId = string.Empty;

        // Get Account Id
        var accountIdRequestJson = JsonSerializer.Serialize(new
        {
            accountName = _options.CurrentValue.DexcomUsername,
            applicationId = "d8665ade-9673-4e27-9ff6-92db4ce13d13",
            password = _options.CurrentValue.DexcomPassword
        });

        var accountUrl = _urlBuilder.BuildDexComAccountIdUrl();
        var accountIdRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(accountUrl))
        {
            Content = new StringContent(accountIdRequestJson, Encoding.UTF8, "application/json")
        };

        try
        {
            var response = await client.SendAsync(accountIdRequest);
            DebugText.Add("Sending Account Id Request. Status code: " + response.StatusCode);
            accountId = (await response.Content.ReadAsStringAsync()).Replace("\"", "");
            if (accountId.Any(x => x != '0' && x != '-'))
                DebugText.Add("Got a valid account id");
            else
                DebugText.Add("Invalid account id");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Issue getting account id");
            if (_options.CurrentValue.IsDebugMode)
                DebugService.ShowDebugAlert(ex, "DexCom account id fetch", string.Join(Environment.NewLine, DebugText));
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
            applicationId = "d8665ade-9673-4e27-9ff6-92db4ce13d13",
            password = _options.CurrentValue.DexcomPassword
        });

        var sessionUrl = _urlBuilder.BuildDexComSessionUrl();
        var request = new HttpRequestMessage(HttpMethod.Post, new Uri(sessionUrl))
        {
            Content = new StringContent(sessionIdRequestJson, Encoding.UTF8, "application/json")
        };

        GlucoseResult fetchResult = new();
        try
        {
            var response = await client.SendAsync(request);
            DebugText.Add("Sending Session Id Request. Status code: " + response.StatusCode);
            var sessionId = (await response.Content.ReadAsStringAsync()).Replace("\"", "");
            if (accountId.Any(x => x != '0' && x != '-'))
                DebugText.Add("Got a valid session id");
            else
                DebugText.Add("Invalid session id");

            var url = _urlBuilder.BuildDexComGlucoseValueUrl(sessionId);
            request = new HttpRequestMessage(HttpMethod.Post, new Uri(url));
            var initialResult = await client.SendAsync(request);
            DebugText.Add("Sending Gluocse Event Request. Status code: " + initialResult.StatusCode);
            var stringResult = await initialResult.Content.ReadAsStringAsync();
            DebugText.Add("Result: " + stringResult);
            DebugText.Add("Attempting to deserialize");
            var result = JsonSerializer.Deserialize<List<DexcomResult>>(stringResult)!.First();
            DebugText.Add("Deserialized");
            var unixTime = string.Join("", result.ST.Where(char.IsDigit));
            var trend = result.Trend;

            GlucoseMath.CalculateValues(fetchResult, result.Value, _options.CurrentValue);
            fetchResult.DateTimeUTC = !string.IsNullOrWhiteSpace(unixTime) ? DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(unixTime)).UtcDateTime : DateTime.MinValue;
            fetchResult.Trend = trend.GetTrend();
            fetchResult.Source = FetchMethod.DexcomShare;
            response.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dexcom fetching failed or received incorrect format.");
            if (_options.CurrentValue.IsDebugMode)
                DebugService.ShowDebugAlert(ex, "DexCom result fetch", string.Join(Environment.NewLine, DebugText));
            fetchResult = GlucoseResult.Default;
        }
        finally
        {
            request.Dispose();
        }

        return fetchResult;
    }
}
