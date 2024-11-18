using GlucoseTray.Domain;
using GlucoseTray.Domain.DisplayResults;
using GlucoseTray.Domain.Enums;
using GlucoseTray.Domain.GlucoseSettings;
using GlucoseTray.GlucoseSettings;
using GlucoseTray.Views.Settings;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GlucoseTray.DisplayResults;

public class IconService(ILogger<IconService> logger, ISettingsProxy options, ISchedulingAdapter taskScheduler, ISettingsWindowService settingsWindowService, IDialogService dialogService, GlucoseResult glucoseResult) : IIconService
{
    private readonly float _standardOffset = -10f;
    private readonly int _defaultFontSize = 40;
    private readonly int _smallerFontSize = 38;
    private NotifyIcon _trayIcon = new();
    private bool SettingsFormIsOpen;

    public void InitializeTrayIcon(EventHandler exitEvent)
    {
        _trayIcon = new NotifyIcon()
        {
            ContextMenuStrip = new ContextMenuStrip(new Container()),
            Visible = true
        };
        PopulateContextMenu(exitEvent);
        _trayIcon.DoubleClick += ShowBalloon;
    }

    public void ShowTrayNotification(string alertName) => _trayIcon.ShowBalloonTip(2000, "Glucose Alert", alertName, ToolTipIcon.Warning);
    private void ShowBalloon(object? sender, EventArgs e) => _trayIcon.ShowBalloonTip(2000, "Glucose", GetGlucoseMessage(), ToolTipIcon.Info);

    public void DisposeTrayIcon()
    {
        _trayIcon.Visible = false;
        _trayIcon.Dispose();
    }

    public void CreateIcon()
    {
        _trayIcon.Text = GetGlucoseMessage();
        CreateTextIcon(_trayIcon);
    }

    private string GetGlucoseMessage() => $"{glucoseResult.GetFormattedStringValue(options.GlucoseUnit)}   {glucoseResult.DateTimeUTC.ToLocalTime().ToLongTimeString()}  {glucoseResult.Trend.GetTrendArrow()}{glucoseResult.StaleMessage(options.StaleResultsThreshold)}";

    private void PopulateContextMenu(EventHandler exitEvent)
    {
        _trayIcon.ContextMenuStrip?.Items.Clear(); // Remove all existing items

        if (!string.IsNullOrWhiteSpace(options.NightscoutUrl)) // Add Nightscout website shortcut
        {
            logger.LogDebug("Nightscout url supplied, adding option to context menu.");

            var process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = options.NightscoutUrl;
            _trayIcon.ContextMenuStrip?.Items.Add(new ToolStripMenuItem("Nightscout", null, (obj, e) => process.Start()));
        }

        var taskEnabled = taskScheduler.HasTaskEnabled();
        _trayIcon.ContextMenuStrip?.Items.Add(new ToolStripMenuItem(taskEnabled ? "Disable Run on startup" : "Run on startup", null, (obj, e) => ToggleTask(!taskEnabled, exitEvent)));
        _trayIcon.ContextMenuStrip?.Items.Add(new ToolStripMenuItem("Change Settings", null, new EventHandler(ChangeSettings)));
        _trayIcon.ContextMenuStrip?.Items.Add(new ToolStripMenuItem("About", null, new EventHandler(About)));
        _trayIcon.ContextMenuStrip?.Items.Add(new ToolStripMenuItem("Exit", null, exitEvent));
    }

    private void ToggleTask(bool enable, EventHandler exitEvent)
    {
        taskScheduler.ToggleTask(enable);
        PopulateContextMenu(exitEvent);
    }

    private void About(object? sender, EventArgs e)
    {
        var result = MessageBox.Show($"Version: {Program.AppSettings.Version} \r\n\r\n Link: {Program.AppSettings.Url} \r\n\r\n Open link?", "About", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
        if (result == DialogResult.OK)
        {
            var process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = Program.AppSettings.Url;
            process.Start();
        }
    }

    private void ChangeSettings(object? sender, EventArgs e)
    {
        if (!SettingsFormIsOpen)
        {
            var settingsWindow = new SettingsWindow(settingsWindowService, dialogService);
            SettingsFormIsOpen = true;
            settingsWindow.ShowDialog();
            SettingsFormIsOpen = false;
        }
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool DestroyIcon(nint handle);

    private static void DestroyMyIcon(nint handle) => DestroyIcon(handle);

    private Brush SetColor(double val)
    {
        if (options.IsDarkMode)
        {
            return val switch
            {
                double n when n < options.WarningHighBg && n > options.WarningLowBg => new SolidBrush(Color.White),
                double n when n >= options.WarningHighBg && n < options.HighBg => new SolidBrush(Color.Yellow),
                double n when n >= options.HighBg => new SolidBrush(Color.Red),
                double n when n <= options.WarningLowBg && n > options.LowBg => new SolidBrush(Color.Yellow),
                double n when n <= options.LowBg && n > options.CriticalLowBg => new SolidBrush(Color.Red),
                double n when n <= options.CriticalLowBg && n > 0 => new SolidBrush(Color.Red),
                _ => new SolidBrush(Color.White),
            };
        }
        else
        {
            // Still having issues when using against a "light" background. Main issue appears to be when we use "DrawString" it is adding a black border/shadow here around the generated image numbers.
            // Need to investigate if it's possible to have a double-wide icon, feasibility of multiple icons, or some other method of making this clearer.
            return val switch
            {
                double n when n < options.WarningHighBg && n > options.WarningLowBg => new SolidBrush(Color.Black),
                double n when n >= options.WarningHighBg && n < options.HighBg => new SolidBrush(Color.DarkGoldenrod),
                double n when n >= options.HighBg => new SolidBrush(Color.Red),
                double n when n <= options.WarningLowBg && n > options.LowBg => new SolidBrush(Color.DarkGoldenrod),
                double n when n <= options.LowBg && n > options.CriticalLowBg => new SolidBrush(Color.Red),
                double n when n <= options.CriticalLowBg && n > 0 => new SolidBrush(Color.Red),
                _ => new SolidBrush(Color.Black),
            };
        }
    }

    private void CreateTextIcon(NotifyIcon trayIcon)
    {
        var glucoseValue = glucoseResult.GetFormattedStringValue(options.GlucoseUnit).Replace('.', '\''); // Use ' instead of . since it is narrower and allows a better display of a two digit number + decimal place.

        var isStale = glucoseResult.IsStale(options.StaleResultsThreshold);

        if (glucoseValue == "0")
        {
            logger.LogWarning("Empty glucose result received.");
            glucoseValue = "NUL";
        }
        else if (glucoseResult.IsCriticalLow)
        {
            logger.LogInformation("Critical low glucose read.");
            glucoseValue = "DAN";
        }

        var fontSize = CalculateFontSize(glucoseResult);
        var font = new Font("Roboto", fontSize, isStale ? FontStyle.Strikeout : FontStyle.Regular, GraphicsUnit.Pixel);

        var bitmapText = new Bitmap(64, 64);
        var g = Graphics.FromImage(bitmapText);
        g.Clear(Color.Transparent);
        g.DrawString(glucoseValue, font, SetColor(options.GlucoseUnit == GlucoseUnitType.MG ? glucoseResult.MgValue : glucoseResult.MmolValue), _standardOffset, 0f);
        var hIcon = bitmapText.GetHicon();
        var myIcon = Icon.FromHandle(hIcon);
        trayIcon.Icon = myIcon;

        DestroyMyIcon(myIcon.Handle);
        bitmapText.Dispose();
        g.Dispose();
        myIcon.Dispose();
    }

    private int CalculateFontSize(GlucoseResult result)
    {
        var value = options.GlucoseUnit == GlucoseUnitType.MG ? result.MgValue : result.MmolValue;
        if (options.GlucoseUnit == GlucoseUnitType.MMOL && value > 9.9) // Need to use smaller font size to accommodate 3 numbers + a decimal point
            return _smallerFontSize;
        return _defaultFontSize;
    }
}