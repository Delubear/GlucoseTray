using GlucoseTray.Enums;
using System.Text.Json;

namespace GlucoseTray.Read.Dexcom;

internal class DexcomReadStrategy(AppSettings settings, IExternalCommunicationAdapter communicator, IGlucoseReadingMapper mapper) : IReadStrategy
{
    public async Task<GlucoseReading> GetLatestGlucoseAsync()
    {
        string accountId = await GetAccountIdAsync();
        string sessionId = await GetSessionIdAsync(accountId);
        string response = await GetApiResponseAsync(sessionId);

        var data = JsonSerializer.Deserialize<List<DexcomResult>>(response)!.First();

        var result = mapper.Map(data);
        return result;
    }

    private async Task<string> GetApiResponseAsync(string sessionId)
    {
        var url = $"https://{GetDexComServer()}/ShareWebServices/Services/Publisher/ReadPublisherLatestGlucoseValues?sessionId={sessionId}&minutes=1440&maxCount=1";
        var result = await communicator.PostApiResponseAsync(url, sessionId);
        return result;
    }

    private async Task<string> GetSessionIdAsync(string accountId)
    {
        var sessionIdRequestJson = JsonSerializer.Serialize(new
        {
            accountId,
            applicationId = "d8665ade-9673-4e27-9ff6-92db4ce13d13",
            password = settings.DexcomPassword
        });

        var sessionUrl = $"https://{GetDexComServer()}/ShareWebServices/Services/General/LoginPublisherAccountById";

        var result = await communicator.PostApiResponseAsync(sessionUrl, sessionIdRequestJson);
        var sessionId = result.Replace("\"", "");

        return sessionId;
    }

    private async Task<string> GetAccountIdAsync()
    {
        var accountIdRequestJson = JsonSerializer.Serialize(new
        {
            accountName = settings.DexcomUsername,
            applicationId = "d8665ade-9673-4e27-9ff6-92db4ce13d13",
            password = settings.DexcomPassword
        });

        var accountUrl = $"https://{GetDexComServer()}/ShareWebServices/Services/General/AuthenticatePublisherAccount";

        var result = await communicator.PostApiResponseAsync(accountUrl, accountIdRequestJson);
        var accountId = result.Replace("\"", "");

        return accountId;
    }

    public string GetDexComServer() => settings.DexcomServer switch
    {
        DexcomServer.DexcomShare1 => "share1.dexcom.com",
        DexcomServer.DexcomShare2 => "share2.dexcom.com",
        DexcomServer.DexcomInternational => "shareous1.dexcom.com",
        _ => "share1.dexcom.com",
    };
}
