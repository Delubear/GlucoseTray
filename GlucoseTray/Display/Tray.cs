namespace GlucoseTray.Display;

public interface ITray
{
    void Refresh(GlucoseReading result);
}

public class Tray : ITray
{
    private readonly ITrayIcon _icon;
    private readonly IGlucoseDisplayMapper _mapper;
    private readonly IScheduler _scheduler;
    private readonly IAlertService _alertService;

    public Tray(ITrayIcon icon, IGlucoseDisplayMapper mapper, IScheduler scheduler, IAlertService alertService)
    {
        _icon = icon;
        _mapper = mapper;
        _scheduler = scheduler;
        _alertService = alertService;

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

        var alert = _alertService.GetAlertMessage(result.MgValue, result.MmolValue, display.IsStale);
        if (string.IsNullOrWhiteSpace(alert))
            _icon.ShowNotification(alert);
    }
}
