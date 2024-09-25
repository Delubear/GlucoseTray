namespace GlucoseTray.Domain.DisplayResults
{
    public interface IIconService
    {
        void InitializeTrayIcon(EventHandler exitEvent);
        void DisposeTrayIcon();
        void CreateIcon(GlucoseResult glucoseResult);
        void ShowAlert(string alertName);
    }
}