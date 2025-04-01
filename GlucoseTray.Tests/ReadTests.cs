
using GlucoseTray.Tests.DSL.Read;

namespace GlucoseTray.Tests;

public class ReadTests
{
    [Test]
    public void ShouldReadDexcomMgResult()
    {
        var driver = new ReadDriver();
        driver.GivenADexcomResult()
              .WithMgValue(100)
              .When.GettingLatestDexcomReading()
              .Then.ShouldHaveMgValueOf(100);
    }

    [Test]
    public void ShouldReadDexcomMmolResult()
    {
        var driver = new ReadDriver();
        driver.GivenADexcomResult()
              .WithMmolServerUnitType()
              .WithMmolValue(5.5f)
              .When.GettingLatestDexcomReading()
              .Then.ShouldHaveMmolValueOf(5.5f);
    }

    [Test]
    public void ShouldReadDexcomMgResultWhenServerIsReturningMmol()
    {
        var driver = new ReadDriver();
        driver.GivenADexcomResult()
              .WithMmolServerUnitType()
              .WithMmolValue(5.55077f)
              .When.GettingLatestDexcomReading()
              .Then.ShouldHaveMgValueOf(100);
    }

    [Test]
    public void ShouldReadDexcomMmolResultWhenServerIsReturningMg()
    {
        var driver = new ReadDriver();
        driver.GivenADexcomResult()
              .WithMgServerUnitType()
              .WithMgValue(100)
              .When.GettingLatestDexcomReading()
              .Then.ShouldHaveMmolValueOf(5.549944f);
    }

    [Test]
    public void ShouldReadNightScoutResult()
    {
        var driver = new ReadDriver();
        driver.GivenANightscoutResult()
              .WithMgValue(100)
              .When.GettingLatestNightScoutReading()
              .Then.ShouldHaveMgValueOf(100);
    }

    [Test]
    public void ShouldReadNightScoutMmolResult()
    {
        var driver = new ReadDriver();
        driver.GivenANightscoutResult()
              .WithMmolServerUnit()
              .WithMmolValue(5.5f)
              .When.GettingLatestNightScoutReading()
              .Then.ShouldHaveMmolValueOf(5.5f);
    }

    [Test]
    public void ShouldReadNightScoutMgResultWhenServerIsReturningMmol()
    {
        var driver = new ReadDriver();
        driver.GivenANightscoutResult()
              .WithMmolServerUnit()
              .WithMmolValue(5.55077f)
              .When.GettingLatestNightScoutReading()
              .Then.ShouldHaveMgValueOf(100);
    }

    [Test]
    public void ShouldReadNightScoutMmolResultWhenServerIsReturningMg()
    {
        var driver = new ReadDriver();
        driver.GivenANightscoutResult()
              .WithMgServerUnit()
              .WithMgValue(100)
              .When.GettingLatestNightScoutReading()
              .Then.ShouldHaveMmolValueOf(5.549944f);
    }
}
