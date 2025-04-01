using GlucoseTray.Enums;
using GlucoseTray.Read;
using NSubstitute;

namespace GlucoseTray.Tests.DSL.Display;

internal class DisplayDriver
{
    private readonly DisplayProvider _provider = new();
    private GlucoseReading _reading = new();
    private readonly AppSettings _settings = new()
    {
        MinutesUntilStale = 5,
        CriticalLowMgThreshold = 55,
        LowMgThreshold = 70,
        HighMgThreshold = 250,
        CriticalHighMgThreshold = 300,
        CriticalLowMmolThreshold = 3.0f,
        LowMmolThreshold = 3.8f,
        HighMmolThreshold = 13.8f,
        CriticalHighMmolThreshold = 16.6f,
        DisplayUnitType = GlucoseUnitType.Mg,
        IsDarkMode = false,
        ServerUnitType = GlucoseUnitType.Mg,
        EnableAlerts = true,
    };

    public DisplayDriver() => _provider.Options.CurrentValue.Returns(_settings);

    public DisplayDriver GivenAGlucoseReading()
    {
        _reading = new GlucoseReading() { TimestampUtc = DateTime.UtcNow };
        return this;
    }

    public DisplayDriver WithMgServerUnit()
    {
        _settings.ServerUnitType = GlucoseUnitType.Mg;
        _provider.Options.CurrentValue.Returns(_settings);
        return this;
    }

    public DisplayDriver WithMmolServerUnit()
    {
        _settings.ServerUnitType = GlucoseUnitType.Mmol;
        _provider.Options.CurrentValue.Returns(_settings);
        return this;
    }

    public DisplayDriver WithMgDisplay()
    {
        _settings.DisplayUnitType = GlucoseUnitType.Mg;
        _provider.Options.CurrentValue.Returns(_settings);
        return this;
    }

    public DisplayDriver WithMmolDisplay()
    {
        _settings.DisplayUnitType = GlucoseUnitType.Mmol;
        _provider.Options.CurrentValue.Returns(_settings);
        return this;
    }

    public DisplayDriver WithMgValue(int value)
    {
        _reading.MgValue = value;
        return this;
    }

    public DisplayDriver WithMmolValue(float value)
    {
        _reading.MmolValue = value;
        return this;
    }

    public DisplayDriver WithDarkMode()
    {
        _settings.IsDarkMode = true;
        _provider.Options.CurrentValue.Returns(_settings);
        return this;
    }

    public DisplayDriver WithStaleData()
    {
        _settings.MinutesUntilStale = 15;
        _provider.Options.CurrentValue.Returns(_settings);
        _reading.TimestampUtc = DateTime.UtcNow.AddMinutes(-30);
        return this;
    }

    public DisplayDriver WithCriticalLowValue()
    {
        _reading.MgValue = 50;
        _reading.MmolValue = 2.8f;
        return this;
    }

    public DisplayDriver WithLowValue()
    {
        _reading.MgValue = 65;
        _reading.MmolValue = 3.6f;
        return this;
    }

    public DisplayDriver WithHighValue()
    {
        _reading.MgValue = 260;
        _reading.MmolValue = 14.4f;
        return this;
    }

    public DisplayDriver WithCriticalHighValue()
    {
        _reading.MgValue = 300;
        _reading.MmolValue = 16.6f;
        return this;
    }

    public DisplayBehaviorDriver When => new(_provider, _reading);
}
