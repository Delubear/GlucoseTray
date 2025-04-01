using NSubstitute;
namespace GlucoseTray.Tests.DSL;

internal class AppDriver
{
    private readonly DslProvider _provider = new();
    private GlucoseReading _reading = new();
    private readonly AppSettings _settings = new()
    {
        MinutesUntilStale = 5,
        CriticalLowMgThreshold = 55,
        LowMgThreshold = 70,
        HighMgThreshold = 250,
        CriticalHighMgThreshold = 300,
        IsDarkMode = false,
    };

    public AppDriver() => _provider.Options.CurrentValue.Returns(_settings);

    public AppDriver GivenAGlucoseReading()
    {
        _reading = new GlucoseReading();
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
        return this;
    }

    public AppDriver WithMmolValue(float value)
    {
        _reading.MmolValue = value;
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
        return this;
    }

    public AppDriver WithCriticalLowValue()
    {
        _reading.MgValue = 50;
        _reading.MmolValue = 2.8f;
        return this;
    }

    public AppDriver WithLowValue()
    {
        _reading.MgValue = 65;
        _reading.MmolValue = 3.6f;
        return this;
    }

    public AppDriver WithHighValue()
    {
        _reading.MgValue = 260;
        _reading.MmolValue = 14.4f;
        return this;
    }

    public AppDriver WithCriticalHighValue()
    {
        _reading.MgValue = 300;
        _reading.MmolValue = 16.6f;
        return this;
    }

    public BehaviorDriver When => new(_provider, _reading);
}
