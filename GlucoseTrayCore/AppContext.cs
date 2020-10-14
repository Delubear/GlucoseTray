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

        public AppContext(ILogger<AppContext> logger, IGlucoseTrayDbContext context, IconService iconService, IGlucoseFetchService fetchService, IOptions<GlucoseTraySettings> options)
        {
            _logger = logger;
            _context = context;
            _iconService = iconService;
            _fetchService = fetchService;
            _options = options.Value;

            trayIcon = new NotifyIcon()
            {
                ContextMenuStrip = new ContextMenuStrip(new Container()),
                Visible = true
            };

            if (!string.IsNullOrWhiteSpace(_options.NightscoutUrl))
            {
                _logger.LogDebug("Nightscout url supplied, adding option to context menu.");

                var process = new Process();
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = _options.NightscoutUrl;

                trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Nightscout", null, (obj, e) => process.Start()));
            }
            trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem(nameof(Exit), null, new EventHandler(Exit)));
            trayIcon.DoubleClick += ShowBalloon;
            BeginCycle();
        }

        private async void BeginCycle()
        {
            while (true)
            {
                try
                {
                    Application.DoEvents();
                    await CreateIcon().ConfigureAwait(false);
                    await Task.Delay(_options.GetPollingThreshold);
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