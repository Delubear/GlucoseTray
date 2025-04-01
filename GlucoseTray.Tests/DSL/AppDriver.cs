using GlucoseTray.Read.Dexcom;
using NSubstitute;
namespace GlucoseTray.Tests.DSL;

internal class AppDriver
{
    private readonly DslProvider _provider = new();
    private GlucoseReading _reading = new();
    private DexcomResult _dexcomResult = new();
    private readonly AppSettings _settings = new()
    {
        MinutesUntilStale = 5,
        CriticalLowMgThreshold = 55,
        LowMgThreshold = 70,
        HighMgThreshold = 250,
        CriticalHighMgThreshold = 300,
        IsDarkMode = false,
        DataSource = GlucoseSource.Dexcom,
        DexcomServer = DexcomServer.DexcomShare1,
        DexcomUsername = "bob",
        DexcomPassword = "pass",
        ServerUnitType = GlucoseUnitType.Mg,
    };

    public AppDriver() => _provider.Options.CurrentValue.Returns(_settings);

    public AppDriver GivenADexcomResult()
    {
        _dexcomResult = new DexcomResult();
        return this;
    }

    public AppDriver GivenAGlucoseReading()
    {
        _reading = new GlucoseReading();
        return this;
    }

    public AppDriver WithMgServerUnit()
    {
        _settings.ServerUnitType = GlucoseUnitType.Mg;
        _provider.Options.CurrentValue.Returns(_settings);
        return this;
    }

    public AppDriver WithMmolServerUnit()
    {
        _settings.ServerUnitType = GlucoseUnitType.Mmol;
        _provider.Options.CurrentValue.Returns(_settings);
        return this;
    }

    public AppDriver WithMgDisplay()
    {
        _settings.DisplayUnitType = GlucoseUnitType.Mg;
        _provider.Options.CurrentValue.Returns(_settings);
        return this;
    }

    public AppDriver WithMmolDisplay()
    {
        _settings.DisplayUnitType = GlucoseUnitType.Mmol;
        _provider.Options.CurrentValue.Returns(_settings);
        return this;
    }

    public AppDriver WithMgValue(int value)
    {
        _reading.MgValue = value;
        _dexcomResult.Value = value;
        return this;
    }

    public AppDriver WithMmolValue(float value)
    {
        _reading.MmolValue = value;
        _dexcomResult.Value = value;
        return this;
    }

    public AppDriver WithDarkMode()
    {
        _settings.IsDarkMode = true;
        _provider.Options.CurrentValue.Returns(_settings);
        return this;
    }

    public AppDriver WithStaleData()
    {
        _settings.MinutesUntilStale = 15;
        _provider.Options.CurrentValue.Returns(_settings);
        _reading.TimestampUtc = DateTime.UtcNow.AddMinutes(-30);
        _dexcomResult.ST = DateTime.UtcNow.AddMinutes(-30).Ticks.ToString();
        return this;
    }

    public AppDriver WithCriticalLowValue()
    {
        _reading.MgValue = 50;
        _reading.MmolValue = 2.8f;
        _dexcomResult.Value = 50;
        return this;
    }

    public AppDriver WithLowValue()
    {
        _reading.MgValue = 65;
        _reading.MmolValue = 3.6f;
        _dexcomResult.Value = 65;
        return this;
    }

    public AppDriver WithHighValue()
    {
        _reading.MgValue = 260;
        _reading.MmolValue = 14.4f;
        _dexcomResult.Value = 260;
        return this;
    }

    public AppDriver WithCriticalHighValue()
    {
        _reading.MgValue = 300;
        _reading.MmolValue = 16.6f;
        _dexcomResult.Value = 300;
        return this;
    }

    public BehaviorDriver When => new(_provider, _reading, _dexcomResult);
}
