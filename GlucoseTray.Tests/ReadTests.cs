
using GlucoseTray.Tests.DSL;

namespace GlucoseTray.Tests;

public class ReadTests
{
    [Test]
    public void ShouldReadDexcomResult()
    {
        var driver = new AppDriver();
        driver.GivenADexcomResult()
              .WithMgValue(100)
              .When.GettingLatestDexcomReading()
              .Then.ShouldBeRefreshedWithValue("100");
    }

    [Test]
    public void ShouldReadDexcomResultInMmol()
    {
        var driver = new AppDriver();
        driver.GivenADexcomResult()
              .WithMmolDisplay()
              .WithMmolServerUnit()
              .WithMmolValue(5.5f)
              .When.GettingLatestDexcomReading()
              .Then.ShouldBeRefreshedWithValue("5'5");
    }
}
