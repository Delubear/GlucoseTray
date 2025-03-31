using GlucoseTray.DisplayResults.Contracts;
using System.Windows.Forms;

namespace GlucoseTray.DisplayResults;

public class DialogService() : IDialogService
{
    public void ShowErrorAlert(string messageBoxText, string caption) => MessageBox.Show(messageBoxText, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);

    public void ShowCriticalAlert(string alertText, string alertName) => MessageBox.Show(alertText, alertName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
}
