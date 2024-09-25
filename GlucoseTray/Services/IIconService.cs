using System.Windows.Forms;

namespace GlucoseTray.Domain
{
    public interface IIconService
    {
        void CreateTextIcon(GlucoseResult result, NotifyIcon trayIcon);
    }
}