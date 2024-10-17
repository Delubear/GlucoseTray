using GlucoseTray.Domain.Enums;
using GlucoseTray.Domain.FetchResults.Dexcom;
using GlucoseTray.Domain.FetchResults.Nightscout;
using GlucoseTray.Domain.GlucoseSettings;
using Microsoft.Extensions.Logging;

namespace GlucoseTray.Domain.FetchResults;

public interface IGlucoseFetchService
{
    Task<GlucoseResult> GetLatestReadingsAsync();
}

public class GlucoseFetchService : IGlucoseFetchService
{
    private readonly ISettingsProxy _options;
    private readonly ILogger<GlucoseFetchService> _logger;
    private readonly IDexcomService _dexcomService;
    private readonly INightscoutService _nightscoutService;

    public GlucoseFetchService(ISettingsProxy options, ILogger<GlucoseFetchService> logger, IDexcomService dexcomService, INightscoutService nightscoutService)
    {
        _logger = logger;
        _options = options;
        _dexcomService = dexcomService;
        _nightscoutService = nightscoutService;
    }

    public async Task<GlucoseResult> GetLatestReadingsAsync()
    {
        var fetchResult = new GlucoseResult();
        try
        {
            switch (_options.FetchMethod)
            {
                case FetchMethod.DexcomShare:
                    fetchResult = await _dexcomService.GetLatestReadingAsync();
                    break;
                case FetchMethod.NightscoutApi:
                    fetchResult = await _nightscoutService.GetLatestReadingAsync();
                    break;
                default:
                    _logger.LogError("Invalid fetch method specified.");
                    throw new InvalidOperationException("Fetch Method either not specified or invalid specification.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get data.");
        }

        return fetchResult;
    }
}
