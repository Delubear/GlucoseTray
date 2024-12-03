namespace GlucoseTray.DisplayResults.Contracts
{
    public interface IIconService
    {
        void InitializeTrayIcon(EventHandler exitEvent);
        void DisposeTrayIcon();
        void CreateIcon();
        void ShowTrayNotification(string alertName);
    }
}