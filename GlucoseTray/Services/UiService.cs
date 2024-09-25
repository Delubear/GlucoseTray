using GlucoseTray.Domain;
using GlucoseTray.Domain.DisplayResults;
using GlucoseTray.Views.Settings;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace GlucoseTray.Services;

public class UiService(ISettingsProxy options, ILogger<UiService> logger, ITaskSchedulerService taskScheduler, IIconService iconService) : IUiService
{
    private readonly ISettingsProxy _options = options;
    private readonly ILogger<UiService> _logger = logger;
    private readonly ITaskSchedulerService _taskScheduler = taskScheduler;
    private readonly IIconService _iconService = iconService;
    private bool SettingsFormIsOpen;
    private GlucoseResult _currentGlucoseResult = new();
    private NotifyIcon _trayIcon = new();

    public void ShowErrorAlert(string messageBoxText, string caption) => MessageBox.Show(messageBoxText, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);

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

    public void DisposeTrayIcon()
    {
        _trayIcon.Visible = false;
        _trayIcon.Dispose();
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

    private string GetGlucoseMessage(GlucoseResult result) => $"{result.GetFormattedStringValue(_options.GlucoseUnit)}   {result.DateTimeUTC.ToLocalTime().ToLongTimeString()}  {result.Trend.GetTrendArrow()}{result.StaleMessage(_options.StaleResultsThreshold)}";

    private void PopulateContextMenu(EventHandler exitEvent)
    {
        _trayIcon.ContextMenuStrip?.Items.Clear(); // Remove all existing items

        if (!string.IsNullOrWhiteSpace(_options.NightscoutUrl)) // Add Nightscout website shortcut
        {
            _logger.LogDebug("Nightscout url supplied, adding option to context menu.");

            var process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = _options.NightscoutUrl;
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
