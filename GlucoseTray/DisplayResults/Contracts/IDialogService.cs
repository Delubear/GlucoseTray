namespace GlucoseTray.DisplayResults.Contracts;

public interface IDialogService
{
    void ShowErrorAlert(string messageBoxText, string caption);
    void ShowCriticalAlert(string alertText, string alertName);
}
