namespace GlucoseTray.Domain.DisplayResults;

public interface IUiService
{
    void ShowErrorAlert(string messageBoxText, string caption);
    void InitializeTrayIcon(EventHandler exitEvent);
    void DisposeTrayIcon();
    void CreateIcon(GlucoseResult glucoseResult);
    void ShowAlert(string alertName);
    void ShowCriticalAlert(string alertText, string alertName);
}
