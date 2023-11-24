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
    private AlertLevel _currentAlertLevel = AlertLevel.None;
    private GlucoseResult? _currentGlucoseResult = null;
    private readonly UiService _uiService;

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
                AlertNotification();

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

    private void AlertNotification()
    {
        if (_currentGlucoseResult?.IsStale(_options.CurrentValue.StaleResultsThreshold) != false)
            return;

        var highAlertTriggered = _options.CurrentValue.HighAlert && IsAlertTriggered(_currentGlucoseResult.MgValue, _currentGlucoseResult.MmolValue, _options.CurrentValue.HighBg, UpDown.Down);
        var warningHighAlertTriggered = _options.CurrentValue.WarningHighAlert && IsAlertTriggered(_currentGlucoseResult.MgValue, _currentGlucoseResult.MmolValue, _options.CurrentValue.WarningHighBg, UpDown.Down);
        var warningLowAlertTriggered = _options.CurrentValue.WarningLowAlert && IsAlertTriggered(_currentGlucoseResult.MgValue, _currentGlucoseResult.MmolValue, _options.CurrentValue.WarningLowBg, UpDown.Up);
        var lowAlertTriggered = _options.CurrentValue.LowAlert && IsAlertTriggered(_currentGlucoseResult.MgValue, _currentGlucoseResult.MmolValue, _options.CurrentValue.LowBg, UpDown.Up);
        var criticalLowAlertTriggered = _options.CurrentValue.CriticallyLowAlert && IsAlertTriggered(_currentGlucoseResult.MgValue, _currentGlucoseResult.MmolValue, _options.CurrentValue.CriticalLowBg, UpDown.Up);

        // Order matters. We need to show the most severe alert while avoid multiple alerts. (High > warning high.  Critical > low > warning low)
        if (highAlertTriggered)
        {
            if (_currentAlertLevel != AlertLevel.High)
                _uiService.ShowAlert("High Glucose Alert");
            _currentAlertLevel = AlertLevel.High;
            return;
        }
        if (warningHighAlertTriggered)
        {
            if (_currentAlertLevel != AlertLevel.High && _currentAlertLevel != AlertLevel.WarningHigh)
                _uiService.ShowAlert("Warning High Glucose Alert");
            _currentAlertLevel = AlertLevel.WarningHigh;
            return;
        }
        if (criticalLowAlertTriggered)
        {
            if (_currentAlertLevel != AlertLevel.CriticalLow)
                MessageBox.Show("Critical Low Glucose Alert", "Critical Low Glucose Alert", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            _currentAlertLevel = AlertLevel.CriticalLow;
            return;
        }
        if (lowAlertTriggered)
        {
            if (_currentAlertLevel != AlertLevel.CriticalLow && _currentAlertLevel != AlertLevel.Low)
                _uiService.ShowAlert("Low Glucose Alert");
            _currentAlertLevel = AlertLevel.Low;
            return;
        }
        if (warningLowAlertTriggered)
        {
            if (_currentAlertLevel != AlertLevel.CriticalLow && _currentAlertLevel != AlertLevel.Low && _currentAlertLevel != AlertLevel.WarningLow)
                _uiService.ShowAlert("Warning Low Glucose Alert");
            _currentAlertLevel = AlertLevel.WarningLow;
            return;
        }
        _currentAlertLevel = AlertLevel.None;
    }

    private bool IsAlertTriggered(double glucoseValueMG, double glucoseValueMMOL, double alertThreshold, UpDown directionGlucoseShouldBeToNotAlert) =>
        _options.CurrentValue.GlucoseUnit == GlucoseUnitType.MG
            ? directionGlucoseShouldBeToNotAlert == UpDown.Down ? glucoseValueMG >= alertThreshold : glucoseValueMG <= alertThreshold
            : directionGlucoseShouldBeToNotAlert == UpDown.Down ? glucoseValueMMOL >= alertThreshold : glucoseValueMMOL <= alertThreshold;

    private void Exit(object? sender, EventArgs e)
    {
        _logger.LogInformation("Exiting application.");
        _trayIcon.Visible = false;
        _trayIcon?.Dispose();
        Application.ExitThread();
        Application.Exit();
    }
}