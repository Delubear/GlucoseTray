namespace GlucoseTray;

public class AppWrapper : ApplicationContext
{
    public AppWrapper(AppRunner app)
    {
        _ = RunAsync(app);
    }

    private static async Task RunAsync(AppRunner app)
    {
        try
        {
            await app.Start();
        }
        catch
        {
            // AppRunner already logs the fatal error and disposes the tray;
            // ensure the WinForms message loop shuts down cleanly.
            Application.Exit();
        }
    }
}
