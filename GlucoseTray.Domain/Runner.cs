using GlucoseTray.Domain.DisplayResults;
using GlucoseTray.Domain.FetchResults;

namespace GlucoseTray.Domain;

public interface IRunner
{
    void Initialize(EventHandler exitHandler);
    Task DoWorkAsync();
    void HandleShutdown(Exception? e = null);
}

public class Runner(IGlucoseFetchService fetchService, IIconService uiService, AlertService alertService, IDialogService dialogService) : IRunner
{
    private readonly IGlucoseFetchService _fetchService = fetchService;
    private readonly IIconService _iconService = uiService;
    private readonly IDialogService _dialogService = dialogService;
    private readonly AlertService _alertService = alertService;

    public void Initialize(EventHandler exitHandler) => _iconService.InitializeTrayIcon(exitHandler);

    public async Task DoWorkAsync()
    {
        await _fetchService.GetLatestReadingsAsync();
        _iconService.CreateIcon();
        _alertService.AlertNotification();
    }

    public void HandleShutdown(Exception? e = null)
    {
        if (e is not null)
            _dialogService.ShowErrorAlert($"ERROR: {e}", "ERROR");
        _iconService.DisposeTrayIcon();
    }
}
