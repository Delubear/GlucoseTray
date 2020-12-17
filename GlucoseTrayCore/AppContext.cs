using GlucoseTrayCore.Data;
using GlucoseTrayCore.Enums;
using GlucoseTrayCore.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GlucoseTrayCore.Extensions;
using GlucoseTrayCore.Models;
using GlucoseTrayCore.Views;
using System.Collections.Generic;

namespace GlucoseTrayCore
{
    public class AppContext : ApplicationContext
    {
        private readonly ILogger<AppContext> _logger;
        private readonly IOptionsMonitor<GlucoseTraySettings> _options;
        private readonly IGlucoseTrayDbContext _context;
        private readonly IGlucoseFetchService _fetchService;
        private readonly NotifyIcon trayIcon;

        private GlucoseResult GlucoseResult = null;
        private readonly IconService _iconService;
        private readonly TaskSchedulerService _taskScheduler;

        public AppContext(ILogger<AppContext> logger, IGlucoseTrayDbContext context, IconService iconService, IGlucoseFetchService fetchService, IOptionsMonitor<GlucoseTraySettings> options, TaskSchedulerService taskScheduler)
        {
            _logger = logger;
            _context = context;
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
            await CheckForMissingReadings();

            while (true)
            {
                try
                {
                    Application.DoEvents();

                    var results = await _fetchService.GetLatestReadings(GlucoseResult?.DateTimeUTC).ConfigureAwait(false);

                    if (results.Any())
                    {
                        LogResultToDb(results);

                        GlucoseResult = results.Last();
                    }

                    CreateIcon();

                    await Task.Delay(_options.CurrentValue.PollingThresholdTimeSpan);
                }
                catch (Exception e)
                {
                    if (_options.CurrentValue.EnableDebugMode)
                        MessageBox.Show($"ERROR: {e}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _logger.LogError(e.ToString());
                    trayIcon.Visible = false;
                    trayIcon?.Dispose();
                    Environment.Exit(0);
                }
            }
        }

        private async Task CheckForMissingReadings()
        {
            if (_options.CurrentValue.FetchMethod != FetchMethod.NightscoutApi)
                return;

            GlucoseResult = _context.GlucoseResults.OrderByDescending(a => a.DateTimeUTC).FirstOrDefault();

            if (GlucoseResult == null)
            {
                if (MessageBox.Show("Do you want to import readings from NightScout?\r\n\r\n(Warning this may take some time.)", "GlucoseTrayCore : No Readings found in local database.", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
            }

            DateTime startDate = GlucoseResult?.DateTimeUTC ?? DateTime.UtcNow.AddYears(-100);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var missingResults = await _fetchService.GetLatestReadings(startDate).ConfigureAwait(false);
            sw.Stop();
            int count = missingResults.Count();
            if (count > 0)
            {
                var sinceMessage = (GlucoseResult != null) ? $" since last database record at {GlucoseResult.DateTimeUTC} UTC" : "";

                if (count == 1)
                    _logger.LogWarning($"Starting Up : Found 1 reading recorded at {missingResults[0].DateTimeUTC} UTC{sinceMessage}.");
                else
                    _logger.LogWarning($"Found {count} readings between {missingResults[0].DateTimeUTC} and {missingResults[count - 1].DateTimeUTC} UTC{sinceMessage}. Retrieving them took {sw.Elapsed.TotalSeconds:#,##0.##} seconds");

                sw.Restart();
                _context.GlucoseResults.AddRange(missingResults);   // None of these records will be in the database, so just add them all now.
                _context.SaveChanges();
                sw.Stop();
                if (sw.Elapsed.TotalSeconds > 5)
                    _logger.LogWarning($"Saving {missingResults.Count()} records took {sw.Elapsed.TotalSeconds:#,##0.##} seconds");

                GlucoseResult = missingResults.Last();
            }
        }


        private void Exit(object sender, EventArgs e)
        {
            _logger.LogInformation("Exiting application.");
            trayIcon.Visible = false;
            trayIcon?.Dispose();
            Application.ExitThread();
            Application.Exit();
        }

        private void ChangeSettings(object sender, EventArgs e)
        {
            var existingSettingsForm = Application.OpenForms.OfType<Settings>().FirstOrDefault();
            if (existingSettingsForm != null)
            {
                Application.OpenForms.OfType<Settings>().First().BringToFront();
            }
            else
            {
                using var settings = new Settings();
                if (settings.ShowDialog() == DialogResult.OK)
                    MessageBox.Show("Settings saved");
            }
        }

        private void CreateIcon()
        {
            trayIcon.Text = GetGlucoseMessage(GlucoseResult);
            _iconService.CreateTextIcon(GlucoseResult, trayIcon);
        }

        private void ShowBalloon(object sender, EventArgs e) => trayIcon.ShowBalloonTip(2000, "Glucose", GetGlucoseMessage(GlucoseResult), ToolTipIcon.Info);

        private string GetGlucoseMessage(GlucoseResult result) => $"{result.GetFormattedStringValue(_options.CurrentValue.GlucoseUnit)}   {result.DateTimeUTC.ToLocalTime().ToLongTimeString()}  {result.Trend.GetTrendArrow()}{result.StaleMessage(_options.CurrentValue.StaleResultsThreshold)}";

        private void LogResultToDb(List<GlucoseResult> results)
        {
            if (results.Count > 1)
            {
                _logger.LogWarning($"Found {results.Count} readings between {results[0].DateTimeUTC} and {results[results.Count - 1].DateTimeUTC} UTC{(System.Diagnostics.Debugger.IsAttached ? " (Debugging Mode)" : "")}" );
            }
            foreach (var result in results)
            {
                if (!_context.GlucoseResults.Any(g => g.DateTimeUTC == result.DateTimeUTC && !result.WasError && g.MgValue == result.MgValue))
                {
                    _context.GlucoseResults.Add(result);
                    _context.SaveChanges();
                }
            }
        }
    }
}