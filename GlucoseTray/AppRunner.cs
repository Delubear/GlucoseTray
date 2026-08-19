using GlucoseTray.Display;
using GlucoseTray.Read;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GlucoseTray;

public class AppRunner(ITray tray, IGlucoseReader reader, IOptionsMonitor<AppSettings> options, ILogger<AppRunner> logger) : IDisposable
{
    private readonly SemaphoreSlim _processLock = new(1, 1);
    private IDisposable? _onChangeSubscription;

    public async Task Start()
    {
        _onChangeSubscription = options.OnChange(settings => { _ = RefreshOnConfigChangeAsync(); });

        while (true)
        {
            try
            {
                await Process();
                await Task.Delay(TimeSpan.FromMinutes(Math.Max(options.CurrentValue.RefreshIntervalInMinutes, 1)));
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Fatal error in the glucose refresh loop. Disposing tray and shutting down.");
                tray.Dispose();
                throw;
            }
        }
    }

    private async Task RefreshOnConfigChangeAsync()
    {
        try
        {
            await Process();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to refresh glucose reading after a configuration change.");
        }
    }

    public async Task Process()
    {
        await _processLock.WaitAsync();
        try
        {
            var result = await reader.GetLatestGlucoseAsync();
            tray.Refresh(result);
        }
        finally
        {
            _processLock.Release();
        }
    }

    public void Dispose()
    {
        _onChangeSubscription?.Dispose();
        _processLock.Dispose();
        GC.SuppressFinalize(this);
    }
}
