using GlucoseTray.Extensions;
using GlucoseTray.Models;
using GlucoseTray.Views.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace GlucoseTray.Services
{
    public class UiService
    {
        readonly IOptionsMonitor<GlucoseTraySettings> _options;
        readonly ILogger<UiService> _logger;
        readonly TaskSchedulerService _taskScheduler;
        readonly IconService _iconService;
        bool SettingsFormIsOpen;
        GlucoseResult _currentGlucoseResult;
        NotifyIcon _trayIcon;

        public UiService(IOptionsMonitor<GlucoseTraySettings> options, ILogger<UiService> logger, TaskSchedulerService taskScheduler, IconService iconService)
        {
            _options = options;
            _logger = logger;
            _taskScheduler = taskScheduler;
            _iconService = iconService;
        }

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
        void ShowBalloon(object sender, EventArgs e) => _trayIcon.ShowBalloonTip(2000, "Glucose", GetGlucoseMessage(_currentGlucoseResult), ToolTipIcon.Info);
        string GetGlucoseMessage(GlucoseResult result) => $"{result.GetFormattedStringValue(_options.CurrentValue.GlucoseUnit)}   {result.DateTimeUTC.ToLocalTime().ToLongTimeString()}  {result.Trend.GetTrendArrow()}{result.StaleMessage(_options.CurrentValue.StaleResultsThreshold)}";

        void PopulateContextMenu(EventHandler exitEvent)
        {
            _trayIcon.ContextMenuStrip.Items.Clear(); // Remove all existing items

            if (!string.IsNullOrWhiteSpace(_options.CurrentValue.NightscoutUrl)) // Add Nightscout website shortcut
            {
                _logger.LogDebug("Nightscout url supplied, adding option to context menu.");
                Process process = new Process();
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = _options.CurrentValue.NightscoutUrl;
                _trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Nightscout", null, (obj, e) => process.Start()));
            }

            bool taskEnabled = _taskScheduler.HasTaskEnabled();
            _trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem(taskEnabled ? "Disable Run on startup" : "Run on startup", null, (obj, e) => ToggleTask(!taskEnabled, exitEvent)));
            _trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Change Settings", null, new EventHandler(ChangeSettings)));
            _trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("About", null, new EventHandler(About)));
            _trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Exit", null, exitEvent));
        }

        void ToggleTask(bool enable, EventHandler exitEvent)
        {
            _taskScheduler.ToggleTask(enable);
            PopulateContextMenu(exitEvent);
        }

        void About(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show($"Version: {Program.AppSettings.Version} \r\n\r\n Link: {Program.AppSettings.Url} \r\n\r\n Open link?", "About", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.OK)
            {
                Process process = new Process();
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = Program.AppSettings.Url;
                process.Start();
            }
        }

        void ChangeSettings(object sender, EventArgs e)
        {
            if (!SettingsFormIsOpen)
            {
                SettingsWindow settingsWindow = new SettingsWindow();
                SettingsFormIsOpen = true;
                if (settingsWindow.ShowDialog() == true)
                    MessageBox.Show("Settings saved");
                SettingsFormIsOpen = false;
            }
        }
    }
}
