using GlucoseTray.Views.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace GlucoseTray.Services;

public interface IUiService
{
    void ShowErrorAlert(string messageBoxText, string caption);
    NotifyIcon InitializeTrayIcon(EventHandler exitEvent);
    void CreateIcon(GlucoseResult glucoseResult);
    void ShowAlert(string alertName);
    void ShowCriticalAlert(string alertText, string alertName);
}

public class UiService(IOptionsMonitor<GlucoseTraySettings> options, ILogger<UiService> logger, TaskSchedulerService taskScheduler, IconService iconService) : IUiService
{
    private readonly IOptionsMonitor<GlucoseTraySettings> _options = options;
    private readonly ILogger<UiService> _logger = logger;
    private readonly TaskSchedulerService _taskScheduler = taskScheduler;
    private readonly IconService _iconService = iconService;
    private bool SettingsFormIsOpen;
    private GlucoseResult _currentGlucoseResult = new();
    private NotifyIcon _trayIcon = new();

    public void ShowErrorAlert(string messageBoxText, string caption) => MessageBox.Show(messageBoxText, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);

    public NotifyIcon InitializeTrayIcon(EventHandler exitEvent)
    {
        _trayIcon = new NotifyIcon()
        {
            ContextMenuStrip = new ContextMenuStrip(new Container()),
            Visible = true
        };
        PopulateContextMenu(exitEvent);
        _trayIcon.DoubleClick += ShowBalloon;
        return _trayIcon;
    }

    public void CreateIcon(GlucoseResult glucoseResult)
    {
        _currentGlucoseResult = glucoseResult;
        _trayIcon.Text = GetGlucoseMessage(_currentGlucoseResult);
        _iconService.CreateTextIcon(_currentGlucoseResult, _trayIcon);
    }

    public void ShowAlert(string alertName) => _trayIcon.ShowBalloonTip(2000, "Glucose Alert", alertName, ToolTipIcon.Warning);

    public void ShowCriticalAlert(string alertText, string alertName) => MessageBox.Show(alertText, alertName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

    private void ShowBalloon(object? sender, EventArgs e) => _trayIcon.ShowBalloonTip(2000, "Glucose", GetGlucoseMessage(_currentGlucoseResult), ToolTipIcon.Info);

    private string GetGlucoseMessage(GlucoseResult result) => $"{result.GetFormattedStringValue(_options.CurrentValue.GlucoseUnit)}   {result.DateTimeUTC.ToLocalTime().ToLongTimeString()}  {result.Trend.GetTrendArrow()}{result.StaleMessage(_options.CurrentValue.StaleResultsThreshold)}";

    private void PopulateContextMenu(EventHandler exitEvent)
    {
        _trayIcon.ContextMenuStrip?.Items.Clear(); // Remove all existing items

        if (!string.IsNullOrWhiteSpace(_options.CurrentValue.NightscoutUrl)) // Add Nightscout website shortcut
        {
            _logger.LogDebug("Nightscout url supplied, adding option to context menu.");

            var process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = _options.CurrentValue.NightscoutUrl;
            _trayIcon.ContextMenuStrip?.Items.Add(new ToolStripMenuItem("Nightscout", null, (obj, e) => process.Start()));
        }

        var taskEnabled = _taskScheduler.HasTaskEnabled();
        _trayIcon.ContextMenuStrip?.Items.Add(new ToolStripMenuItem(taskEnabled ? "Disable Run on startup" : "Run on startup", null, (obj, e) => ToggleTask(!taskEnabled, exitEvent)));
        _trayIcon.ContextMenuStrip?.Items.Add(new ToolStripMenuItem("Change Settings", null, new EventHandler(ChangeSettings)));
        _trayIcon.ContextMenuStrip?.Items.Add(new ToolStripMenuItem("About", null, new EventHandler(About)));
        _trayIcon.ContextMenuStrip?.Items.Add(new ToolStripMenuItem("Exit", null, exitEvent));
    }

    private void ToggleTask(bool enable, EventHandler exitEvent)
    {
        _taskScheduler.ToggleTask(enable);
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
            var settingsWindow = new SettingsWindow(new SettingsWindowService());
            SettingsFormIsOpen = true;
            settingsWindow.ShowDialog();
            SettingsFormIsOpen = false;
        }
    }
}
