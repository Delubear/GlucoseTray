using GlucoseTray.Enums;
using GlucoseTray.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GlucoseTray.Extensions;
using GlucoseTray.Models;
using System.Collections.Generic;
using GlucoseTray.Views.Settings;

namespace GlucoseTray
{
    public class AppContext : ApplicationContext
    {
        private readonly ILogger<AppContext> _logger;
        private readonly IOptionsMonitor<GlucoseTraySettings> _options;
        private readonly IGlucoseFetchService _fetchService;
        private readonly NotifyIcon trayIcon;

        private GlucoseResult GlucoseResult = null;
        private readonly IconService _iconService;
        private readonly TaskSchedulerService _taskScheduler;

        public AppContext(ILogger<AppContext> logger, IconService iconService, IGlucoseFetchService fetchService, IOptionsMonitor<GlucoseTraySettings> options, TaskSchedulerService taskScheduler)
        {
            _logger = logger;
            _iconService = iconService;
            _fetchService = fetchService;
            _options = options;
            _taskScheduler = taskScheduler;

            trayIcon = new NotifyIcon()
            {
                ContextMenuStrip = new ContextMenuStrip(new Container()),
                Visible = true
            };

            PopulateContextMenu();
            trayIcon.DoubleClick += ShowBalloon;
            BeginCycle();
        }

        private void PopulateContextMenu()
        {
            trayIcon.ContextMenuStrip.Items.Clear(); // Remove all existing items

            if (!string.IsNullOrWhiteSpace(_options.CurrentValue.NightscoutUrl)) // Add Nightscout website shortcut
            {
                _logger.LogDebug("Nightscout url supplied, adding option to context menu.");

                var process = new Process();
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = _options.CurrentValue.NightscoutUrl;
                trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Nightscout", null, (obj, e) => process.Start()));
            }

            var taskEnabled = _taskScheduler.HasTaskEnabled();
            trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem(taskEnabled ? "Disable Run on startup" : "Run on startup", null, (obj, e) => ToggleTask(!taskEnabled)));
            trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Change Settings", null, new EventHandler(ChangeSettings)));
            trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem(nameof(Exit), null, new EventHandler(Exit)));
        }

        private void ToggleTask(bool enable)
        {
            _taskScheduler.ToggleTask(enable);
            PopulateContextMenu();
        }

        private async void BeginCycle()
        {
            while (true)
            {
                try
                {
                    Application.DoEvents();

                    var results = await _fetchService.GetLatestReadings(GlucoseResult?.DateTimeUTC);

                    if (results.Any())
                        GlucoseResult = results.Last();

                    CreateIcon();
                    AlertNotification();

                    await Task.Delay(_options.CurrentValue.PollingThresholdTimeSpan);
                }
                catch (Exception e)
                {
                    MessageBox.Show($"ERROR: {e}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _logger.LogError(e.ToString());
                    trayIcon.Visible = false;
                    trayIcon?.Dispose();
                    Environment.Exit(0);
                }
            }
        }

        private AlertLevel CurrentAlertLevel = AlertLevel.None;

        private void AlertNotification()
        {
            if (GlucoseResult?.IsStale(_options.CurrentValue.StaleResultsThreshold) != false)
                return;

            var highAlertTriggered = _options.CurrentValue.HighAlert && IsAlertTriggered(GlucoseResult.MgValue, GlucoseResult.MmolValue, _options.CurrentValue.HighBg, UpDown.Down);
            var warningHighAlertTriggered = _options.CurrentValue.WarningHighAlert && IsAlertTriggered(GlucoseResult.MgValue, GlucoseResult.MmolValue, _options.CurrentValue.WarningHighBg, UpDown.Down);
            var warningLowAlertTriggered = _options.CurrentValue.WarningLowAlert && IsAlertTriggered(GlucoseResult.MgValue, GlucoseResult.MmolValue, _options.CurrentValue.WarningLowBg, UpDown.Up);
            var lowAlertTriggered = _options.CurrentValue.LowAlert && IsAlertTriggered(GlucoseResult.MgValue, GlucoseResult.MmolValue, _options.CurrentValue.LowBg, UpDown.Up);
            var criticalLowAlertTriggered = _options.CurrentValue.CriticallyLowAlert && IsAlertTriggered(GlucoseResult.MgValue, GlucoseResult.MmolValue, _options.CurrentValue.CriticalLowBg, UpDown.Up);

            // Order matters. We need to show the most severe alert while avoid multiple alerts. (High > warning high.  Critical > low > warning low)
            if (highAlertTriggered)
            {
                if (CurrentAlertLevel != AlertLevel.High)
                    ShowAlert("High Glucose Alert");
                CurrentAlertLevel = AlertLevel.High;
                return;
            }
            if (warningHighAlertTriggered)
            {
                if (CurrentAlertLevel != AlertLevel.High && CurrentAlertLevel != AlertLevel.WarningHigh)
                    ShowAlert("Warning High Glucose Alert");
                CurrentAlertLevel = AlertLevel.WarningHigh;
                return;
            }
            if (criticalLowAlertTriggered)
            {
                if (CurrentAlertLevel != AlertLevel.CriticalLow)
                    MessageBox.Show("Critical Low Glucose Alert", "Critical Low Glucose Alert", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                CurrentAlertLevel = AlertLevel.CriticalLow;
                return;
            }
            if (lowAlertTriggered)
            {
                if (CurrentAlertLevel != AlertLevel.CriticalLow && CurrentAlertLevel != AlertLevel.Low)
                    ShowAlert("Low Glucose Alert");
                CurrentAlertLevel = AlertLevel.Low;
                return;
            }
            if (warningLowAlertTriggered)
            {
                if (CurrentAlertLevel != AlertLevel.CriticalLow && CurrentAlertLevel != AlertLevel.Low && CurrentAlertLevel != AlertLevel.WarningLow)
                    ShowAlert("Warning Low Glucose Alert");
                CurrentAlertLevel = AlertLevel.WarningLow;
                return;
            }
            CurrentAlertLevel = AlertLevel.None;
        }

        private void ShowAlert(string alertName) => trayIcon.ShowBalloonTip(2000, "Glucose Alert", alertName, ToolTipIcon.Warning);

        private bool IsAlertTriggered(double glucoseValueMG, double glucoseValueMMOL, double alertThreshold, UpDown directionGlucoseShouldBeToNotAlert) =>
            _options.CurrentValue.GlucoseUnit == GlucoseUnitType.MG
                ? directionGlucoseShouldBeToNotAlert == UpDown.Down ? glucoseValueMG >= alertThreshold : glucoseValueMG <= alertThreshold
                : directionGlucoseShouldBeToNotAlert == UpDown.Down ? glucoseValueMMOL >= alertThreshold : glucoseValueMMOL <= alertThreshold;

        private void Exit(object sender, EventArgs e)
        {
            _logger.LogInformation("Exiting application.");
            trayIcon.Visible = false;
            trayIcon?.Dispose();
            Application.ExitThread();
            Application.Exit();
        }

        private bool SettingsFormIsOpen;
        private void ChangeSettings(object sender, EventArgs e)
        {
            if (!SettingsFormIsOpen)
            {
                var settingsWindow = new SettingsWindow();
                SettingsFormIsOpen = true;
                if (settingsWindow.ShowDialog() == true)
                    MessageBox.Show("Settings saved");
                SettingsFormIsOpen = false;
            }
        }

        private void CreateIcon()
        {
            trayIcon.Text = GetGlucoseMessage(GlucoseResult);
            _iconService.CreateTextIcon(GlucoseResult, trayIcon);
        }

        private void ShowBalloon(object sender, EventArgs e) => trayIcon.ShowBalloonTip(2000, "Glucose", GetGlucoseMessage(GlucoseResult), ToolTipIcon.Info);

        private string GetGlucoseMessage(GlucoseResult result) => $"{result.GetFormattedStringValue(_options.CurrentValue.GlucoseUnit)}   {result.DateTimeUTC.ToLocalTime().ToLongTimeString()}  {result.Trend.GetTrendArrow()}{result.StaleMessage(_options.CurrentValue.StaleResultsThreshold)}";
    }
}