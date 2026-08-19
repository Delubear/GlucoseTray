using System.Text.Json;

namespace GlucoseTray.Read.Nightscout;

internal class NightscoutReadStrategy(AppSettings settings, IExternalCommunicationAdapter communicator, IGlucoseReadingMapper mapper, ICredentialProtector protector) : IReadStrategy
{
    public async Task<GlucoseReading> GetLatestGlucoseAsync()
    {
        var response = await GetApiResponseAsync();
        var data = JsonSerializer.Deserialize<List<NightScoutResult>>(response)?.LastOrDefault() ?? throw new InvalidOperationException("Nightscout returned no glucose readings.");

        var result = mapper.Map(data);
        return result;
    }

    private async Task<string> GetApiResponseAsync()
    {
        var url = $"{settings.NightscoutUrl.TrimEnd('/')}/api/v1/entries/sgv?count=1";
        var token = protector.Unprotect(settings.NightscoutToken);
        url += !string.IsNullOrWhiteSpace(token) ? $"&token={token}" : string.Empty;

        var result = await communicator.GetApiResponseAsync(url);
        return result;
    }
}
