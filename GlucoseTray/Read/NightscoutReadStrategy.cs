using System.Text.Json;

namespace GlucoseTray.Read;

internal class NightscoutReadStrategy(AppSettings settings, IExternalCommunicationAdapter communicator, IGlucoseReadingMapper mapper) : IReadStrategy
{
    public async Task<GlucoseReading> GetLatestGlucoseAsync()
    {
        var response = await GetApiResponseAsync();
        var data = JsonSerializer.Deserialize<List<NightScoutResult>>(response)!.Last();

        var result = mapper.Map(data);
        return result;
    }

    private async Task<string> GetApiResponseAsync()
    {
        var url = $"{settings.NightscoutUrl.TrimEnd('/')}/api/v1/entries/sgv?count=1";
        url += !string.IsNullOrWhiteSpace(settings.NightscoutToken) ? $"&token={settings.NightscoutToken}" : string.Empty;

        var result = await communicator.GetApiResponseAsync(url);
        return result;
    }
}
