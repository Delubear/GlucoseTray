using GlucoseTray.Display;
using GlucoseTray.Read;
using Microsoft.Extensions.Options;

namespace GlucoseTray;

public class AppRunner(ITray tray, IGlucoseReader reader, IOptionsMonitor<AppSettings> options)
{
    public async Task Start()
    {
        while (true)
        {
            try
            {
                await Process();
                await Task.Delay(Math.Max(options.CurrentValue.RefreshIntervalInMinutes, 1));
            }
            catch
            {
                throw new Exception();
            }
        }
    }

    public async Task Process()
    {
        var result = await reader.GetLatestGlucoseAsync();
        tray.Refresh(result);
    }
}
