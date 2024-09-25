using GlucoseTray.Domain.DisplayResults;
using GlucoseTray.Domain.FetchResults;
using NSubstitute;

namespace GlucoseTray.Tests;

public class DebugServiceTests
{
    [Test]
    public void AddDebugText_Should_AddText()
    {
        // Arrange
        var mockUiService = Substitute.For<IDialogService>();
        var debugService = new DebugService(mockUiService);
        var text = "Test";

        // Act
        debugService.AddDebugText(text);
        debugService.ShowDebugAlert(new Exception(), "");

        // Assert
        mockUiService.Received().ShowErrorAlert(Arg.Is<string>(x => x.Contains("Test")), Arg.Any<string>());
    }

    [Test]
    public void ClearDebugText_Should_ClearText()
    {
        // Arrange
        var mockUiService = Substitute.For<IDialogService>();
        var debugService = new DebugService(mockUiService);
        var text = "Test";

        // Act
        debugService.AddDebugText(text);
        debugService.ClearDebugText();
        debugService.ShowDebugAlert(new Exception(), "");

        // Assert
        mockUiService.Received().ShowErrorAlert(Arg.Is<string>(x => !x.Contains("Test")), Arg.Any<string>());
    }

    [Test]
    public void ShowDebugAlert_Should_ShowAlert()
    {
        // Arrange
        var mockUiService = Substitute.For<IDialogService>();
        var debugService = new DebugService(mockUiService);

        // Act
        debugService.ShowDebugAlert(new Exception("Test"), "Test");

        // Assert
        mockUiService.Received().ShowErrorAlert(Arg.Any<string>(), Arg.Any<string>());
    }
}
