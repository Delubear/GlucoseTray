namespace GlucoseTray;

public class AppRunner(ITray tray, IGlucoseReader reader)
{
    public async Task Start()
    {
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
