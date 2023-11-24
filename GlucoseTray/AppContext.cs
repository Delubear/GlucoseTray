using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Windows.Forms;

namespace GlucoseTray;

public class AppContext : ApplicationContext
{
    private readonly ILogger<AppContext> _logger;
    private readonly IOptionsMonitor<GlucoseTraySettings> _options;
    private readonly IGlucoseFetchService _fetchService;
    private NotifyIcon _trayIcon;
    private GlucoseResult? _currentGlucoseResult = null;
    private readonly UiService _uiService;
    private readonly AlertService _alertService;

    public AppContext(ILogger<AppContext> logger, IGlucoseFetchService fetchService, IOptionsMonitor<GlucoseTraySettings> options, UiService uiService)
    {
        _logger = logger;
        _fetchService = fetchService;
        _options = options;
        _uiService = uiService;

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

                _currentGlucoseResult = await _fetchService.GetLatestReadingsAsync();
                _uiService.CreateIcon(_currentGlucoseResult);
                _alertService.AlertNotification(_currentGlucoseResult);

                await Task.Delay(_options.CurrentValue.PollingThresholdTimeSpan);
            }
            catch (Exception e)
            {
                MessageBox.Show($"ERROR: {e}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _logger.LogError(e.ToString());
                _trayIcon.Visible = false;
                _trayIcon?.Dispose();
                Environment.Exit(0);
            }
        }
    }

    private void Exit(object? sender, EventArgs e)
    {
        _logger.LogInformation("Exiting application.");
        _trayIcon.Visible = false;
        _trayIcon?.Dispose();
        Application.ExitThread();
        Application.Exit();
    }
}