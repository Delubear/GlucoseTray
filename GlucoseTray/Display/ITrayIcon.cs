using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace GlucoseTray.Display;

public interface ITrayIcon
{
    void ClearMenu();
    void AddAutoRunMenu(bool isAlreadyOn, EventHandler toggleCallback);
    void AddSettingsMenu();
    void AddExitMenu();
    void RefreshIcon(GlucoseDisplay display);
}

public class NotificationIcon : ITrayIcon
{
    private readonly NotifyIcon _trayIcon;
    private GlucoseDisplay? _latestGlucose;

    public NotificationIcon()
    {
        _trayIcon = new NotifyIcon
        {
            ContextMenuStrip = new ContextMenuStrip(new Container()),
            Visible = true,
        };
        _trayIcon.DoubleClick += ShowBalloon;
    }

    private void ShowBalloon(object? sender, EventArgs e) => _trayIcon?.ShowBalloonTip(2000, "Glucose", _latestGlucose?.GetDisplayMessage(DateTime.UtcNow) ?? "error", ToolTipIcon.Info);

    public void ClearMenu() => _trayIcon?.ContextMenuStrip?.Items.Clear();
    public void AddAutoRunMenu(bool isAlreadyOn, EventHandler toggleCallback) => _trayIcon?.ContextMenuStrip?.Items.Add(new ToolStripMenuItem(isAlreadyOn ? "Disable auto-start" : "Run on startup", null, toggleCallback));
    public void AddSettingsMenu() => _trayIcon?.ContextMenuStrip?.Items.Add(new ToolStripMenuItem("Settings", null, Settings));
    public void AddExitMenu() => _trayIcon?.ContextMenuStrip?.Items.Add(new ToolStripMenuItem("Exit", null, Exit));

    private void Settings(object? sender, EventArgs e)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\appsettings.json",
            UseShellExecute = true,
        };
        Process.Start(startInfo);
    }

    private void Exit(object? sender, EventArgs e)
    {
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
        var bitmapText = new Bitmap(64, 64);
        var g = Graphics.FromImage(bitmapText);
        g.Clear(Color.Transparent);

        var font = new Font("Roboto", display.FontSize, display.IsStale ? FontStyle.Strikeout : FontStyle.Regular, GraphicsUnit.Pixel);
        var offset = -10f;
        g.DrawString(display.DisplayValue, font, new SolidBrush(Convert(display.Color)), offset, 0f);
        var hIcon = bitmapText.GetHicon();
        var myIcon = Icon.FromHandle(hIcon);
        _trayIcon.Icon = myIcon;

        DestroyMyIcon(myIcon.Handle);
        bitmapText.Dispose();
        g.Dispose();
        myIcon.Dispose();
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
