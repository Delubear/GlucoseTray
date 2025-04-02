using GlucoseTray.Display;
using GlucoseTray.Read;
using Microsoft.Extensions.Options;

namespace GlucoseTray;

public class AppRunner(ITray tray, IGlucoseReader reader, IOptionsMonitor<AppSettings> options)
{
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
            catch
            {
                tray.Dispose();
                throw;
            }
        }
    }

    public async Task Process()
    {
        var result = await reader.GetLatestGlucoseAsync();
        tray.Refresh(result);
    }
}
