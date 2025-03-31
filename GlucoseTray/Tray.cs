
namespace GlucoseTray;

public interface ITray
{
    void Refresh(GlucoseResult result);
    //void Initialize(EventHandler exitHandler);
    //void ShowBalloon(string alertName);
    //void Dispose();
    //void CreateIcon();
}

public class Tray : ITray
{
    private ITrayIcon _icon;

    public Tray(ITrayIcon icon)
    {
        _icon = icon;

        _icon.AddExitMenu();
    }

    private void PopulateContextMenu(EventHandler exitHandler)
    {
        // Remove all existing items

        //if (!string.IsNullOrWhiteSpace(options.NightscoutUrl)) // Add Nightscout website shortcut
        //{
        //    logger.LogDebug("Nightscout url supplied, adding option to context menu.");

        //    var process = new Process();
        //    process.StartInfo.UseShellExecute = true;
        //    process.StartInfo.FileName = options.NightscoutUrl;
        //    _trayIcon.ContextMenuStrip?.Items.Add(new ToolStripMenuItem("Nightscout", null, (obj, e) => process.Start()));
        //}

        //var taskEnabled = taskScheduler.HasTaskEnabled();
        //_trayIcon.ContextMenuStrip?.Items.Add(new ToolStripMenuItem(taskEnabled ? "Disable Run on startup" : "Run on startup", null, (obj, e) => ToggleTask(!taskEnabled, exitEvent)));
        //_trayIcon.ContextMenuStrip?.Items.Add(new ToolStripMenuItem("Change Settings", null, new EventHandler(ChangeSettings)));
        //_trayIcon.ContextMenuStrip?.Items.Add(new ToolStripMenuItem("About", null, new EventHandler(About)));
        //_trayIcon.ContextMenuStrip?.Items.Add(new ToolStripMenuItem("Exit", null, exitHandler));
    }

    public void Refresh(GlucoseResult result)
    {
        var display = new GlucoseDisplay() { DisplayValue = result.MgValue.ToString(), Color = GlucoseColor.White, FontSize = 10 };
        _icon.RefreshIcon(display);
    }
}
