using System.ComponentModel;
using System.Runtime.InteropServices;

namespace GlucoseTray.Display;

public interface ITrayIcon
{
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
        _trayIcon.ContextMenuStrip?.Items.Clear();
    }

    private void ShowBalloon(object? sender, EventArgs e) => _trayIcon?.ShowBalloonTip(2000, "Glucose", _latestGlucose?.GetDisplayMessage(DateTime.UtcNow) ?? "error", ToolTipIcon.Info);

    public void AddExitMenu() => _trayIcon?.ContextMenuStrip?.Items.Add(new ToolStripMenuItem("Exit", null, Exit));

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
