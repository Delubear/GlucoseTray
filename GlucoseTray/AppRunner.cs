using GlucoseTray.Display;
using GlucoseTray.Read;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GlucoseTray;

public class AppRunner(ITray tray, IGlucoseReader reader, IOptionsMonitor<AppSettings> options, ILogger<AppRunner> logger)
{
    private readonly SemaphoreSlim _processLock = new(1, 1);

    public async Task Start()
    {
        options.OnChange(async _ => await Process());

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
}
