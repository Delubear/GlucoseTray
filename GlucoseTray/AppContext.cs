using GlucoseTray.Domain;
using GlucoseTray.Domain.GlucoseSettings;
using Microsoft.Extensions.Logging;
using System.Windows.Forms;

namespace GlucoseTray;

public class AppContext : ApplicationContext
{
    private readonly ILogger<AppContext> _logger;
    private readonly IRunner _runner;
    private readonly ISettingsProxy _options;

    public AppContext(ILogger<AppContext> logger, IRunner runner, ISettingsProxy options)
    {
        _logger = logger;
        _runner = runner;
        _options = options;

        RunUntilFailureAsync();
    }

    private async void RunUntilFailureAsync()
    {
        _runner.Initialize(new EventHandler(Exit));

        while (true)
        {
            try
            {
                Application.DoEvents();
                await _runner.DoWorkAsync();
                await Task.Delay(_options.PollingThresholdTimeSpan);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while fetching the latest glucose readings.");
                _runner.HandleShutdown(e);
                Environment.Exit(0);
            }
        }
    }

    private void Exit(object? sender, EventArgs e)
    {
        _logger.LogInformation("Exiting application.");
        _runner.HandleShutdown();
        Application.ExitThread();
        Application.Exit();
    }
}
