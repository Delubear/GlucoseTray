using GlucoseTray.Tests.DSL;

namespace GlucoseTray.Tests;

public class Tests
{
    [Test]
    public async Task GivenAGlucoseResultWithMgValue100_WhenRefreshingIcon_ShouldDisplayWithValue100()
    {
        var driver = new AppDriver();
        driver.GivenAGlucoseResult()
              .WithMgValue(100)
              .When.RefreshingIcon()
              .Then.ShouldBeRefreshedWithValue("100");
    }
}
