namespace GlucoseTray;

public class AppWrapper : ApplicationContext
{
    public AppWrapper(AppRunner app)
    {
        try
        {
            _ = app.Start();
        }
        catch (Exception)
        {
            Environment.Exit(0);
        }
    }
}
