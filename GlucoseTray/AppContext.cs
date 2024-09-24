using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Windows.Forms;

namespace GlucoseTray;

public class AppContext : ApplicationContext
{
    private readonly ILogger<AppContext> _logger;
    private readonly IOptionsMonitor<GlucoseTraySettings> _options;
    private readonly IGlucoseFetchService _fetchService;
    private readonly NotifyIcon _trayIcon;
    private readonly IUiService _uiService;
    private readonly AlertService _alertService;

    public AppContext(ILogger<AppContext> logger, IGlucoseFetchService fetchService, IOptionsMonitor<GlucoseTraySettings> options, IUiService uiService, AlertService alertService)
    {
        _logger = logger;
        _fetchService = fetchService;
        _options = options;
        _uiService = uiService;
        _alertService = alertService;

        _trayIcon = _uiService.InitializeTrayIcon(new EventHandler(Exit));
        BeginCycle();
    }

    private async void BeginCycle()
    {
        while (true)
        {
            try
            {
                Application.DoEvents();

                var currentGlucoseResult = await _fetchService.GetLatestReadingsAsync();
                _uiService.CreateIcon(currentGlucoseResult);
                _alertService.AlertNotification(currentGlucoseResult);

                await Task.Delay(_options.CurrentValue.PollingThresholdTimeSpan);
            }
            catch (Exception e)
            {
                MessageBox.Show($"ERROR: {e}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _logger.LogError(e, "An error occurred while fetching the latest glucose readings.");
                _trayIcon.Visible = false;
                _trayIcon.Dispose();
                Environment.Exit(0);
            }
        }
    }

    private void Exit(object? sender, EventArgs e)
    {
        _logger.LogInformation("Exiting application.");
        _trayIcon.Visible = false;
        _trayIcon.Dispose();
        Application.ExitThread();
        Application.Exit();
    }
}
