using GlucoseTray.Enums;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GlucoseTray.Display;

public interface ITrayIcon : IDisposable
{
    void ClearMenu();
    void AddAutoRunMenu(bool isAlreadyOn, EventHandler toggleCallback);
    void AddSettingsMenu();
    void AddExitMenu();
    void RefreshIcon(GlucoseDisplay display);
    void ShowNotification(string alertText);
}

public class NotificationIcon : ITrayIcon
{
    // Icons are pre-drawn at 16x16; Windows scales them up on high-DPI displays.
    private const int IconSize = 16;

    private readonly NotifyIcon _trayIcon;
    private readonly IGlucoseIconRenderer _renderer;
    private GlucoseDisplay? _latestGlucose;

    public NotificationIcon(IGlucoseIconRenderer renderer)
    {
        _renderer = renderer;
        _trayIcon = new NotifyIcon
        {
            ContextMenuStrip = new ContextMenuStrip(new Container()),
            Visible = true,
        };
        _trayIcon.DoubleClick += ShowBalloon;
    }

    public void ShowNotification(string alertText) => _trayIcon.ShowBalloonTip(2000, "Glucose Alert", alertText, ToolTipIcon.Warning);
    private void ShowBalloon(object? sender, EventArgs e) => _trayIcon?.ShowBalloonTip(2000, "Glucose", _latestGlucose?.GetDisplayMessage(DateTime.UtcNow) ?? "error", ToolTipIcon.Info);

    public void ClearMenu() => _trayIcon?.ContextMenuStrip?.Items.Clear();
    public void AddAutoRunMenu(bool isAlreadyOn, EventHandler toggleCallback) => _trayIcon?.ContextMenuStrip?.Items.Add(new ToolStripMenuItem(isAlreadyOn ? "Disable auto-start" : "Run on startup", null, toggleCallback));
    public void AddSettingsMenu() => _trayIcon?.ContextMenuStrip?.Items.Add(new ToolStripMenuItem("Settings", null, Settings));
    public void AddExitMenu() => _trayIcon?.ContextMenuStrip?.Items.Add(new ToolStripMenuItem("Exit", null, Exit));

    private void Settings(object? sender, EventArgs e)
    {
        ProcessStartInfo startInfo = new()
        {
            FileName = Path.GetDirectoryName(AppContext.BaseDirectory) + "\\appsettings.json",
            UseShellExecute = true,
        };
        Process.Start(startInfo);
    }

    public void Dispose()
    {
        _trayIcon.Dispose();
    }

    private void Exit(object? sender, EventArgs e)
    {
        Dispose();
        Application.ExitThread();
        Application.Exit();
    }

    public void RefreshIcon(GlucoseDisplay display)
    {
        _latestGlucose = display;
        CreateTextIcon(display);
    }

    private void CreateTextIcon(GlucoseDisplay display)
    {
        using var bitmap = _renderer.Render(display.DisplayValue, Convert(display.Color), display.IsStale, IconSize);
        var hIcon = bitmap.GetHicon();
        using var myIcon = Icon.FromHandle(hIcon);
        _trayIcon.Icon = myIcon;

        DestroyMyIcon(hIcon);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool DestroyIcon(nint handle);

    private static void DestroyMyIcon(nint handle) => DestroyIcon(handle);

    private static Color Convert(IconTextColor color) => color switch
    {
        IconTextColor.White => Color.White,
        IconTextColor.Black => Color.Black,
        IconTextColor.Yellow => Color.Yellow,
        IconTextColor.Gold => Color.DarkGoldenrod,
        IconTextColor.Red => Color.Red,
        _ => Color.Black,
    };
}
