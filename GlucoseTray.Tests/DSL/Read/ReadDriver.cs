using GlucoseTray.Enums;
using GlucoseTray.Read.Dexcom;
using GlucoseTray.Read.Nightscout;
using NSubstitute;
namespace GlucoseTray.Tests.DSL.Read;

internal class ReadDriver
{
    private readonly ReadProvider _provider = new();
    private DexcomResult _dexcomResult = new();
    private NightScoutResult _nightScoutResult = new();
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

    public ReadDriver() => _provider.Options.CurrentValue.Returns(_settings);

    public ReadDriver GivenADexcomResult()
    {
        _settings.DataSource = GlucoseSource.Dexcom;
        _provider.Options.CurrentValue.Returns(_settings);
        _dexcomResult = new DexcomResult();
        return this;
    }

    public ReadDriver GivenANightscoutResult()
    {
        _settings.DataSource = GlucoseSource.Nightscout;
        _provider.Options.CurrentValue.Returns(_settings);
        _nightScoutResult = new NightScoutResult();
        return this;
    }

    public ReadDriver WithMgServerUnit()
    {
        _settings.ServerUnitType = GlucoseUnitType.Mg;
        _provider.Options.CurrentValue.Returns(_settings);
        return this;
    }

    public ReadDriver WithMmolServerUnit()
    {
        _settings.ServerUnitType = GlucoseUnitType.Mmol;
        _provider.Options.CurrentValue.Returns(_settings);
        return this;
    }

    public ReadDriver WithMgDisplay()
    {
        _settings.DisplayUnitType = GlucoseUnitType.Mg;
        _provider.Options.CurrentValue.Returns(_settings);
        return this;
    }

    public ReadDriver WithMmolDisplay()
    {
        _settings.DisplayUnitType = GlucoseUnitType.Mmol;
        _provider.Options.CurrentValue.Returns(_settings);
        return this;
    }

    public ReadDriver WithMgValue(int value)
    {
        _dexcomResult.Value = value;
        _nightScoutResult.Sgv = value;
        return this;
    }

    public ReadDriver WithMmolValue(float value)
    {
        _dexcomResult.Value = value;
        _nightScoutResult.Sgv = value;
        return this;
    }

    public ReadDriver WithMgServerUnitType()
    {
        _settings.ServerUnitType = GlucoseUnitType.Mg;
        _provider.Options.CurrentValue.Returns(_settings);
        return this;
    }

    public ReadDriver WithMmolServerUnitType()
    {
        _settings.ServerUnitType = GlucoseUnitType.Mmol;
        _provider.Options.CurrentValue.Returns(_settings);
        return this;
    }

    public ReadDriver WithDarkMode()
    {
        _settings.IsDarkMode = true;
        _provider.Options.CurrentValue.Returns(_settings);
        return this;
    }

    public ReadDriver WithStaleData()
    {
        _settings.MinutesUntilStale = 15;
        _provider.Options.CurrentValue.Returns(_settings);
        _dexcomResult.ST = DateTime.UtcNow.AddMinutes(-30).Ticks.ToString();
        _nightScoutResult.Date = DateTime.UtcNow.AddMinutes(-30).Ticks;
        return this;
    }

    public ReadDriver WithCriticalLowValue()
    {
        _dexcomResult.Value = 50;
        _nightScoutResult.Sgv = 50;
        return this;
    }

    public ReadDriver WithLowValue()
    {
        _dexcomResult.Value = 65;
        _nightScoutResult.Sgv = 65;
        return this;
    }

    public ReadDriver WithHighValue()
    {
        _dexcomResult.Value = 260;
        _nightScoutResult.Sgv = 260;
        return this;
    }

    public ReadDriver WithCriticalHighValue()
    {
        _dexcomResult.Value = 300;
        _nightScoutResult.Sgv = 300;
        return this;
    }

    public ReadBehaviorDriver When => new(_provider, _dexcomResult, _nightScoutResult);
}
