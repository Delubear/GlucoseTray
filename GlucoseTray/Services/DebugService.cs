namespace GlucoseTray.Services;

public class DebugService(IUiService uiService)
{
    private readonly List<string> DebugText = [];
    private readonly IUiService _uiService = uiService;

    public void AddDebugText(string text) => DebugText.Add(text);

    public void ClearDebugText() => DebugText.Clear();

    public void ShowDebugAlert(Exception ex, string message) => _uiService.ShowErrorAlert(ex?.Message + ex?.InnerException?.Message + string.Join(Environment.NewLine, DebugText), message);
}
