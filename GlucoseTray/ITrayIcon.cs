
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace GlucoseTray;

public interface ITrayIcon
{
    void AddExitMenu();
    void RefreshIcon(GlucoseDisplay result);
}

public class NotificationIcon : ITrayIcon
{
    private NotifyIcon _trayIcon;

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

    private void ShowBalloon(object? sender, EventArgs e) => _trayIcon?.ShowBalloonTip(2000, "Glucose", "todo", ToolTipIcon.Info);

    public void AddExitMenu() => _trayIcon?.ContextMenuStrip?.Items.Add(new ToolStripMenuItem("Exit", null, Exit));

    private void Exit(object? sender, EventArgs e)
    {
        Application.ExitThread();
        Application.Exit();
    }

    public void RefreshIcon(GlucoseDisplay result) => CreateTextIcon(result);

    private void CreateTextIcon(GlucoseDisplay result)
    {
        var bitmapText = new Bitmap(64, 64);
        var g = Graphics.FromImage(bitmapText);
        g.Clear(Color.Transparent);

        var font = new Font("Roboto", result.FontSize, result.IsStale ? FontStyle.Strikeout : FontStyle.Regular, GraphicsUnit.Pixel);
        var offset = -10f;
        g.DrawString(result.DisplayValue, font, new SolidBrush(Convert(result.Color)), offset, 0f);
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

    private static Color Convert(GlucoseColor color) => color switch
    {
        GlucoseColor.White => Color.White,
        GlucoseColor.Black => Color.Black,
        GlucoseColor.Yellow => Color.Yellow,
        GlucoseColor.Gold => Color.DarkGoldenrod,
        GlucoseColor.Red => Color.Red,
        _ => Color.Black,
    };
}
