using GlucoseTray.Domain;
using GlucoseTray.Domain.DisplayResults;
using GlucoseTray.Domain.FetchResults;
using Microsoft.Extensions.Logging;
using System.Windows.Forms;

namespace GlucoseTray;

public class AppContext : ApplicationContext
{
    private readonly ILogger<AppContext> _logger;
    private readonly ISettingsProxy _options;
    private readonly IGlucoseFetchService _fetchService;
    private readonly IUiService _uiService;
    private readonly AlertService _alertService;

    public AppContext(ILogger<AppContext> logger, IGlucoseFetchService fetchService, ISettingsProxy options, IUiService uiService, AlertService alertService)
    {
        _logger = logger;
        _fetchService = fetchService;
        _options = options;
        _uiService = uiService;
        _alertService = alertService;

        _uiService.InitializeTrayIcon(new EventHandler(Exit));
        BeginCycle();
    }

    private async void BeginCycle()
    {
        while (true)
        {
            try
            {
                Application.DoEvents();

                GlucoseResult currentGlucoseResult = await _fetchService.GetLatestReadingsAsync();
                _uiService.CreateIcon(currentGlucoseResult);
                _alertService.AlertNotification(currentGlucoseResult);

                await Task.Delay(_options.PollingThresholdTimeSpan);
            }
            catch (Exception e)
            {
                _uiService.ShowErrorAlert($"ERROR: {e}", "ERROR");
                _logger.LogError(e, "An error occurred while fetching the latest glucose readings.");
                _uiService.DisposeTrayIcon();
                Environment.Exit(0);
            }
        }
    }

    private void Exit(object? sender, EventArgs e)
    {
        _logger.LogInformation("Exiting application.");
        _uiService.DisposeTrayIcon();
        Application.ExitThread();
        Application.Exit();
    }
}
