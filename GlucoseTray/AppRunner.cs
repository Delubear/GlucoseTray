using GlucoseTray.Display;
using Microsoft.Extensions.Options;

namespace GlucoseTray;

public class AppRunner(IOptionsMonitor<AppSettings> settings, ITray tray, IGlucoseReader reader)
{
    public async Task Start()
    {
        if (settings.CurrentValue == null)
        {
            throw new Exception("Settings not found");
        }

        while (true)
        {
            try
            {
                await Process();
                await Task.Delay(1000);
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
