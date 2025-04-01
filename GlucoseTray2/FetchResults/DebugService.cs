using GlucoseTray.DisplayResults.Contracts;

namespace GlucoseTray.FetchResults;

public class DebugService(IDialogService uiService)
{
    private readonly List<string> DebugText = [];
    private readonly IDialogService _uiService = uiService;

    public void AddDebugText(string text) => DebugText.Add(text);

    public void ClearDebugText() => DebugText.Clear();

    public void ShowDebugAlert(Exception ex, string message) => _uiService.ShowErrorAlert(ex?.Message + ex?.InnerException?.Message + string.Join(Environment.NewLine, DebugText), message);
}
