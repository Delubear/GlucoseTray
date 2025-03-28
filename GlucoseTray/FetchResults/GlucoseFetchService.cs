﻿using GlucoseTray.Enums;
using GlucoseTray.FetchResults.Contracts;
using GlucoseTray.GlucoseSettings.Contracts;
using Microsoft.Extensions.Logging;

namespace GlucoseTray.FetchResults;

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

    public async Task GetLatestReadingsAsync()
    {
        try
        {
            switch (_options.FetchMethod)
            {
                case DataSource.DexcomShare:
                    await _dexcomService.GetLatestReadingAsync();
                    break;
                case DataSource.NightscoutApi:
                    await _nightscoutService.GetLatestReadingAsync();
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
    }
}
