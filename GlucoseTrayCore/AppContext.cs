using Dexcom.Fetch;
using Dexcom.Fetch.Extensions;
using Dexcom.Fetch.Models;
using GlucoseTrayCore.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GlucoseTrayCore
{
    public class AppContext : ApplicationContext
    {
        private readonly ILogger _logger;
        private readonly NotifyIcon trayIcon;
        private bool IsCriticalLow;

        private GlucoseFetchResult FetchResult;
        private readonly IconService _iconService;

        public AppContext(ILogger logger)
        {
            _logger = logger;
            _iconService = new IconService(_logger);

            trayIcon = new NotifyIcon()
            {
                ContextMenu = new ContextMenu(new MenuItem[] { }),
                Visible = true
            };

            if (!string.IsNullOrWhiteSpace(Constants.NightscoutUrl))
            {
                _logger.LogDebug("Nightscout url supplied, adding option to context menu.");
                trayIcon.ContextMenu.MenuItems.Add(new MenuItem("Nightscout", (obj, e) => Process.Start(Constants.NightscoutUrl)));
            }
            trayIcon.ContextMenu.MenuItems.Add(new MenuItem(nameof(Exit), new EventHandler(Exit)));
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
                    await CreateIcon();
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
            var service = new GlucoseFetchService(new GlucoseFetchConfiguration
            {
                DexcomUsername = Constants.DexcomUsername,
                DexcomPassword = Constants.DexcomPassword,
                FetchMethod = Constants.FetchMethod,
                NightscoutUrl = Constants.NightscoutUrl,
                NightscoutAccessToken = Constants.AccessToken,
                UnitDisplayType = Constants.GlucoseUnitType
            }, _logger);
            FetchResult = await service.GetLatestReading();
            trayIcon.Text = GetGlucoseMessage();
            if (FetchResult.Value <= Constants.CriticalLowBg)
                IsCriticalLow = true;
            _iconService.CreateTextIcon(FetchResult, IsCriticalLow, trayIcon);
        }

        private void ShowBalloon(object sender, EventArgs e) => trayIcon.ShowBalloonTip(2000, "Glucose", GetGlucoseMessage(), ToolTipIcon.Info);

        private string GetGlucoseMessage() => $"{FetchResult.GetFormattedStringValue()}   {FetchResult.Time.ToLongTimeString()}  {FetchResult.TrendIcon}";
    }
}