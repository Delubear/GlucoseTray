using Dexcom.Fetch;
using Dexcom.Fetch.Enums;
using Dexcom.Fetch.Extensions;
using Dexcom.Fetch.Models;
using GlucoseTrayCore.Data;
using GlucoseTrayCore.Services;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GlucoseTrayCore
{
    public class AppContext : ApplicationContext
    {
        private readonly ILogger<AppContext> _logger;
        private readonly IGlucoseTrayDbContext _context;
        private readonly IGlucoseFetchService _fetchService;
        private readonly NotifyIcon trayIcon;
        private bool IsCriticalLow;

        private GlucoseFetchResult FetchResult;
        private readonly IconService _iconService;

        public AppContext(ILogger<AppContext> logger, IGlucoseTrayDbContext context, IconService iconService, IGlucoseFetchService fetchService)
        {
            _logger = logger;
            _context = context;
            _iconService = iconService;
            _fetchService = fetchService;

            trayIcon = new NotifyIcon()
            {
                ContextMenuStrip = new ContextMenuStrip(new Container()),
                Visible = true
            };

            if (!string.IsNullOrWhiteSpace(Constants.NightscoutUrl))
            {
                _logger.LogDebug("Nightscout url supplied, adding option to context menu.");

                var process = new Process();
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = Constants.NightscoutUrl;

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
                    await Task.Delay(Constants.PollingThreshold);
                }
                catch (Exception e)
                {
                    if (Constants.EnableDebugMode)
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
            IsCriticalLow = false;
            FetchResult = await _fetchService.GetLatestReading().ConfigureAwait(false);
            LogResultToDb(FetchResult);
            trayIcon.Text = GetGlucoseMessage();
            if ((Constants.GlucoseUnitType == GlucoseUnitType.MMOL && FetchResult.MmolValue <= Constants.CriticalLowBg) || (Constants.GlucoseUnitType == GlucoseUnitType.MG && FetchResult.MgValue <= Constants.CriticalLowBg))
                IsCriticalLow = true;
            _iconService.CreateTextIcon(FetchResult, IsCriticalLow, trayIcon);
        }

        private void ShowBalloon(object sender, EventArgs e) => trayIcon.ShowBalloonTip(2000, "Glucose", GetGlucoseMessage(), ToolTipIcon.Info);

        private string GetGlucoseMessage() => $"{FetchResult.GetFormattedStringValue(Constants.GlucoseUnitType)}   {FetchResult.Time.ToLongTimeString()}  {FetchResult.Trend.GetTrendArrow()}{FetchResult.StaleMessage(Constants.StaleResultsThreshold)}";

        private void LogResultToDb(GlucoseFetchResult result)
        {
            if (_context.GlucoseResults.Any(g => g.DateTimeUTC == result.Time.ToUniversalTime() && !result.ErrorResult && g.MgValue == result.MgValue))
                return;

            var model = new GlucoseResult
            {
                DateTimeUTC = result.Time.ToUniversalTime(),
                Source = result.Source,
                MgValue = result.MgValue,
                MmolValue = result.MmolValue,
                Trend = result.Trend,
                WasError = result.ErrorResult
            };

            _context.GlucoseResults.Add(model);
            _context.SaveChanges();
        }
    }
}