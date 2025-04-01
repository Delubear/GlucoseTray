using GlucoseTray.Read;
using Microsoft.Extensions.Options;

namespace GlucoseTray.Display;

public interface ITray
{
    void Refresh(GlucoseReading result);
    void Dispose();
}

public class Tray : ITray
{
    private readonly ITrayIcon _icon;
    private readonly IGlucoseDisplayMapper _mapper;
    private readonly IScheduler _scheduler;
    private readonly IAlertService _alertService;
    private readonly IOptionsMonitor<AppSettings> _options;

    public Tray(ITrayIcon icon, IGlucoseDisplayMapper mapper, IScheduler scheduler, IAlertService alertService, IOptionsMonitor<AppSettings> options)
    {
        _icon = icon;
        _mapper = mapper;
        _scheduler = scheduler;
        _alertService = alertService;
        _options = options;

        RebuildContextMenu();
    }

    private void RebuildContextMenu()
    {
        _icon.ClearMenu();
        _icon.AddAutoRunMenu(_scheduler.HasTaskEnabled(), ToggleAutoRun);
        _icon.AddSettingsMenu();
        _icon.AddExitMenu();
    }

    private void ToggleAutoRun(object? sender, EventArgs e)
    {
        _scheduler.ToggleTask(!_scheduler.HasTaskEnabled());
        RebuildContextMenu();
    }

    public void Refresh(GlucoseReading result)
    {
        var display = _mapper.Map(result);
        _icon.RefreshIcon(display);

        if (_options.CurrentValue.EnableAlerts)
            NotifyUser(result, display);
    }

    private void NotifyUser(GlucoseReading result, GlucoseDisplay display)
    {
        var alert = _alertService.GetAlertMessage(result.MgValue, result.MmolValue, display.IsStale);
        if (!string.IsNullOrWhiteSpace(alert))
            _icon.ShowNotification(alert);
    }

    public void Dispose() => _icon.Dispose();
}
