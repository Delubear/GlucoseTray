using System.Windows;

namespace GlucoseTray.Services;

public class DebugService
{
    private readonly List<string> DebugText = [];

    public void AddDebugText(string text) => DebugText.Add(text);

    public void ClearDebugText() => DebugText.Clear();

    public void ShowDebugAlert(Exception ex, string message)
    {
        MessageBox.Show(ex?.Message + ex?.InnerException?.Message + string.Join(Environment.NewLine, DebugText), message, MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
