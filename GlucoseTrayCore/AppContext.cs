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

namespace GlucoseTrayCore
{
    public class AppContext : ApplicationContext
    {
        private readonly ILogger<AppContext> _logger;
        private readonly GlucoseTraySettings _options;
        private readonly IGlucoseTrayDbContext _context;
        private readonly IGlucoseFetchService _fetchService;
        private readonly NotifyIcon trayIcon;

        private GlucoseResult GlucoseResult;
        private readonly IconService _iconService;
        private readonly TaskSchedulerService _taskScheduler;

        public AppContext(ILogger<AppContext> logger, IGlucoseTrayDbContext context, IconService iconService, IGlucoseFetchService fetchService, IOptions<GlucoseTraySettings> options, TaskSchedulerService taskScheduler)
        {
            _logger = logger;
            _context = context;
            _iconService = iconService;
            _fetchService = fetchService;
            _options = options.Value;
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

            if (!string.IsNullOrWhiteSpace(_options.NightscoutUrl)) // Add Nightscout website shortcut
            {
                _logger.LogDebug("Nightscout url supplied, adding option to context menu.");

                var process = new Process();
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = _options.NightscoutUrl;
                trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Nightscout", null, (obj, e) => process.Start()));
            }

            var taskEnabled = _taskScheduler.HasTaskEnabled();
            trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem(taskEnabled ? "Disable Run on startup" : "Run on startup", null, (obj, e) => ToggleTask(!taskEnabled)));
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
                    await CreateIcon().ConfigureAwait(false);
                    await Task.Delay(_options.PollingThresholdTimeSpan);
                }
                catch (Exception e)
                {
                    if (_options.EnableDebugMode)
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
            if (_options.FetchMethod != FetchMethod.NightscoutApi)
                return;

            var newestExistingRecord = _context.GlucoseResults.OrderByDescending(a => a.DateTimeUTC).FirstOrDefault();

            if (newestExistingRecord == null)
            {
                if (MessageBox.Show("Do you want to import readings from NightScout?\r\n\r\n(Warning this may take some time.)", "GlucoseTrayCore : No Readings found in local database.", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;

                newestExistingRecord = new GlucoseResult() { DateTimeUTC = DateTime.UtcNow.AddYears(-100) };
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var missingResults = await _fetchService.FetchMissingReadings(newestExistingRecord.DateTimeUTC);
            sw.Stop();
            int count = missingResults.Count();
            if (count > 0)
            {
                if (count == 1)
                    _logger.LogWarning($"Found 1 reading recorded at {missingResults[0].DateTimeUTC} UTC since last database record at {newestExistingRecord.DateTimeUTC} UTC ");
                else
                    _logger.LogWarning($"Found {count} readings between {missingResults[0].DateTimeUTC} and {missingResults[count-1].DateTimeUTC} UTC since last database record at {newestExistingRecord.DateTimeUTC} UTC. Retrieving them took {sw.Elapsed.TotalSeconds:#,##0.##} seconds");

                sw.Restart();
                _context.GlucoseResults.AddRange(missingResults);   // None of these records will be in the database, so just add them all now.
                _context.SaveChanges();
                sw.Stop();
                if (sw.Elapsed.TotalSeconds > 5)
                    _logger.LogWarning($"Saving {missingResults.Count()} records took {sw.Elapsed.TotalSeconds:#,##0.##} seconds");
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

        private async Task CreateIcon()
        {
            GlucoseResult = await _fetchService.GetLatestReading().ConfigureAwait(false);
            LogResultToDb(GlucoseResult);
            trayIcon.Text = GetGlucoseMessage(GlucoseResult);
            _iconService.CreateTextIcon(GlucoseResult, trayIcon);
        }

        private void ShowBalloon(object sender, EventArgs e) => trayIcon.ShowBalloonTip(2000, "Glucose", GetGlucoseMessage(GlucoseResult), ToolTipIcon.Info);

        private string GetGlucoseMessage(GlucoseResult result) => $"{result.GetFormattedStringValue(_options.GlucoseUnit)}   {result.DateTimeUTC.ToLocalTime().ToLongTimeString()}  {result.Trend.GetTrendArrow()}{result.StaleMessage(_options.StaleResultsThreshold)}";

        private void LogResultToDb(GlucoseResult result)
        {
            if (_context.GlucoseResults.Any(g => g.DateTimeUTC == result.DateTimeUTC && !result.WasError && g.MgValue == result.MgValue))
                return;

            _context.GlucoseResults.Add(result);
            _context.SaveChanges();
        }
    }
}