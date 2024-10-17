using GlucoseTray.Domain.Enums;
using GlucoseTray.Domain.GlucoseSettings;
using GlucoseTray.GlucoseSettings;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace GlucoseTray.Tests;

public class SettingsProxyTests
{
    [Test]
    public void FetchMethod_Should_ReturnCorrectValue()
    {
        var settings = new GlucoseTraySettings
        {
            FetchMethod = FetchMethod.NightscoutApi,
        };
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        Assert.That(settingsProxy.FetchMethod, Is.EqualTo(FetchMethod.NightscoutApi));
    }

    [Test]
    public void FetchMethod_Should_SetCorrectValue()
    {
        var settings = new GlucoseTraySettings();
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        settingsProxy.FetchMethod = FetchMethod.NightscoutApi;
        Assert.That(settingsProxy.FetchMethod, Is.EqualTo(FetchMethod.NightscoutApi));
    }

    [Test]
    public void NightscoutUrl_Should_ReturnCorrectValue()
    {
        var settings = new GlucoseTraySettings
        {
            NightscoutUrl = "https://example.com",
        };
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        Assert.That(settingsProxy.NightscoutUrl, Is.EqualTo("https://example.com"));
    }

    [Test]
    public void NightscoutUrl_Should_SetCorrectValue()
    {
        var settings = new GlucoseTraySettings();
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        settingsProxy.NightscoutUrl = "https://example.com";
        Assert.That(settingsProxy.NightscoutUrl, Is.EqualTo("https://example.com"));
    }

    [Test]
    public void DexcomServer_Should_ReturnCorrectValue()
    {
        var settings = new GlucoseTraySettings
        {
            DexcomServer = DexcomServerLocation.DexcomShare2,
        };
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        Assert.That(settingsProxy.DexcomServer, Is.EqualTo(DexcomServerLocation.DexcomShare2));
    }

    [Test]
    public void DexcomServer_Should_SetCorrectValue()
    {
        var settings = new GlucoseTraySettings();
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        settingsProxy.DexcomServer = DexcomServerLocation.DexcomShare2;
        Assert.That(settingsProxy.DexcomServer, Is.EqualTo(DexcomServerLocation.DexcomShare2));
    }

    [Test]
    public void DexcomUsername_Should_ReturnCorrectValue()
    {
        var settings = new GlucoseTraySettings
        {
            DexcomUsername = "potato"
        };
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        Assert.That(settingsProxy.DexcomUsername, Is.EqualTo("potato"));
    }

    [Test]
    public void DexcomUsername_Should_SetCorrectValue()
    {
        var settings = new GlucoseTraySettings();
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        settingsProxy.DexcomUsername = "potato";
        Assert.That(settingsProxy.DexcomUsername, Is.EqualTo("potato"));
    }

    [Test]
    public void DexcomPassword_Should_ReturnCorrectValue()
    {
        var settings = new GlucoseTraySettings
        {
            DexcomPassword = "password"
        };
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        Assert.That(settingsProxy.DexcomPassword, Is.EqualTo("password"));
    }

    [Test]
    public void DexcomPassword_Should_SetCorrectValue()
    {
        var settings = new GlucoseTraySettings();
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        settingsProxy.DexcomPassword = "password";
        Assert.That(settingsProxy.DexcomPassword, Is.EqualTo("password"));
    }

    [Test]
    public void AccessToken_Should_ReturnCorrectValue()
    {
        var settings = new GlucoseTraySettings
        {
            AccessToken = "token"
        };
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        Assert.That(settingsProxy.AccessToken, Is.EqualTo("token"));
    }

    [Test]
    public void AccessToken_Should_SetCorrectValue()
    {
        var settings = new GlucoseTraySettings();
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        settingsProxy.AccessToken = "token";
        Assert.That(settingsProxy.AccessToken, Is.EqualTo("token"));
    }

    [Test]
    public void GlucoseUnit_Should_ReturnCorrectValue()
    {
        var settings = new GlucoseTraySettings
        {
            GlucoseUnit = GlucoseUnitType.MMOL
        };
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        Assert.That(settingsProxy.GlucoseUnit, Is.EqualTo(GlucoseUnitType.MMOL));
    }

    [Test]
    public void GlucoseUnit_Should_SetCorrectValue()
    {
        var settings = new GlucoseTraySettings();
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        settingsProxy.GlucoseUnit = GlucoseUnitType.MMOL;
        Assert.That(settingsProxy.GlucoseUnit, Is.EqualTo(GlucoseUnitType.MMOL));
    }

    [Test]
    public void WarningHighBg_Should_ReturnCorrectValue()
    {
        var settings = new GlucoseTraySettings
        {
            WarningHighBg = 10.0
        };
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        Assert.That(settingsProxy.WarningHighBg, Is.EqualTo(10.0));
    }

    [Test]
    public void WarningHighBg_Should_SetCorrectValue()
    {
        var settings = new GlucoseTraySettings();
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        settingsProxy.WarningHighBg = 10.0;
        Assert.That(settingsProxy.WarningHighBg, Is.EqualTo(10.0));
    }

    [Test]
    public void HighBg_Should_ReturnCorrectValue()
    {
        var settings = new GlucoseTraySettings
        {
            HighBg = 10.0
        };
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        Assert.That(settingsProxy.HighBg, Is.EqualTo(10.0));
    }

    [Test]
    public void HighBg_Should_SetCorrectValue()
    {
        var settings = new GlucoseTraySettings();
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        settingsProxy.HighBg = 10.0;
        Assert.That(settingsProxy.HighBg, Is.EqualTo(10.0));
    }

    [Test]
    public void WarningLowBg_Should_ReturnCorrectValue()
    {
        var settings = new GlucoseTraySettings
        {
            WarningLowBg = 10.0
        };
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        Assert.That(settingsProxy.WarningLowBg, Is.EqualTo(10.0));
    }

    [Test]
    public void WarningLowBg_Should_SetCorrectValue()
    {
        var settings = new GlucoseTraySettings();
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        settingsProxy.WarningLowBg = 10.0;
        Assert.That(settingsProxy.WarningLowBg, Is.EqualTo(10.0));
    }

    [Test]
    public void LowBg_Should_ReturnCorrectValue()
    {
        var settings = new GlucoseTraySettings
        {
            LowBg = 10.0
        };
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        Assert.That(settingsProxy.LowBg, Is.EqualTo(10.0));
    }

    [Test]
    public void LowBg_Should_SetCorrectValue()
    {
        var settings = new GlucoseTraySettings();
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        settingsProxy.LowBg = 10.0;
        Assert.That(settingsProxy.LowBg, Is.EqualTo(10.0));
    }

    [Test]
    public void CriticalLowBg_Should_ReturnCorrectValue()
    {
        var settings = new GlucoseTraySettings
        {
            CriticalLowBg = 10.0
        };
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        Assert.That(settingsProxy.CriticalLowBg, Is.EqualTo(10.0));
    }

    [Test]
    public void CriticalLowBg_Should_SetCorrectValue()
    {
        var settings = new GlucoseTraySettings();
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        settingsProxy.CriticalLowBg = 10.0;
        Assert.That(settingsProxy.CriticalLowBg, Is.EqualTo(10.0));
    }

    [Test]
    public void PollingThreshold_Should_ReturnCorrectValue()
    {
        var settings = new GlucoseTraySettings
        {
            PollingThreshold = 10
        };
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        Assert.That(settingsProxy.PollingThreshold, Is.EqualTo(10));
    }

    [Test]
    public void PollingThreshold_Should_SetCorrectValue()
    {
        var settings = new GlucoseTraySettings();
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        settingsProxy.PollingThreshold = 10;
        Assert.That(settingsProxy.PollingThreshold, Is.EqualTo(10));
    }

    [Test]
    public void PollingThresholdTimeSpan_Should_ReturnCorrectValue()
    {
        var settings = new GlucoseTraySettings
        {
            PollingThreshold = 600
        };
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        Assert.That(settingsProxy.PollingThresholdTimeSpan, Is.EqualTo(TimeSpan.FromMinutes(10)));
    }

    [Test]
    public void StaleResultsThreshold_Should_ReturnCorrectValue()
    {
        var settings = new GlucoseTraySettings
        {
            StaleResultsThreshold = 10
        };
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        Assert.That(settingsProxy.StaleResultsThreshold, Is.EqualTo(10));
    }

    [Test]
    public void StaleResultsThreshold_Should_SetCorrectValue()
    {
        var settings = new GlucoseTraySettings();
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        settingsProxy.StaleResultsThreshold = 10;
        Assert.That(settingsProxy.StaleResultsThreshold, Is.EqualTo(10));
    }

    [Test]
    public void HighAlert_Should_ReturnCorrectValue()
    {
        var settings = new GlucoseTraySettings
        {
            HighAlert = true
        };
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        Assert.That(settingsProxy.HighAlert, Is.True);
    }

    [Test]
    public void HighAlert_Should_SetCorrectValue()
    {
        var settings = new GlucoseTraySettings();
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        settingsProxy.HighAlert = true;
        Assert.That(settingsProxy.HighAlert, Is.True);
    }

    [Test]
    public void WarningHighAlert_Should_ReturnCorrectValue()
    {
        var settings = new GlucoseTraySettings
        {
            WarningHighAlert = true
        };
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        Assert.That(settingsProxy.WarningHighAlert, Is.True);
    }

    [Test]
    public void WarningHighAlert_Should_SetCorrectValue()
    {
        var settings = new GlucoseTraySettings();
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        settingsProxy.WarningHighAlert = true;
        Assert.That(settingsProxy.WarningHighAlert, Is.True);
    }

    [Test]
    public void WarningLowAlert_Should_ReturnCorrectValue()
    {
        var settings = new GlucoseTraySettings
        {
            WarningLowAlert = true
        };
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        Assert.That(settingsProxy.WarningLowAlert, Is.True);
    }

    [Test]
    public void WarningLowAlert_Should_SetCorrectValue()
    {
        var settings = new GlucoseTraySettings();
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        settingsProxy.WarningLowAlert = true;
        Assert.That(settingsProxy.WarningLowAlert, Is.True);
    }

    [Test]
    public void LowAlert_Should_ReturnCorrectValue()
    {
        var settings = new GlucoseTraySettings
        {
            LowAlert = true
        };
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        Assert.That(settingsProxy.LowAlert, Is.True);
    }

    [Test]
    public void LowAlert_Should_SetCorrectValue()
    {
        var settings = new GlucoseTraySettings();
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        settingsProxy.LowAlert = true;
        Assert.That(settingsProxy.LowAlert, Is.True);
    }

    [Test]
    public void CriticalLowAlert_Should_ReturnCorrectValue()
    {
        var settings = new GlucoseTraySettings
        {
            CriticallyLowAlert = true
        };
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        Assert.That(settingsProxy.CriticalLowAlert, Is.True);
    }

    [Test]
    public void CriticalLowAlert_Should_SetCorrectValue()
    {
        var settings = new GlucoseTraySettings();
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        settingsProxy.CriticalLowAlert = true;
        Assert.That(settingsProxy.CriticalLowAlert, Is.True);
    }

    [Test]
    public void IsServerDataUnitTypeMmol_Should_ReturnCorrectValue()
    {
        var settings = new GlucoseTraySettings
        {
            IsServerDataUnitTypeMmol = true
        };
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        Assert.That(settingsProxy.IsServerDataUnitTypeMmol, Is.True);
    }

    [Test]
    public void IsServerDataUnitTypeMmol_Should_SetCorrectValue()
    {
        var settings = new GlucoseTraySettings();
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        settingsProxy.IsServerDataUnitTypeMmol = true;
        Assert.That(settingsProxy.IsServerDataUnitTypeMmol, Is.True);
    }

    [Test]
    public void IsDebugMode_Should_ReturnCorrectValue()
    {
        var settings = new GlucoseTraySettings
        {
            IsDebugMode = true
        };
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        Assert.That(settingsProxy.IsDebugMode, Is.True);
    }

    [Test]
    public void IsDebugMode_Should_SetCorrectValue()
    {
        var settings = new GlucoseTraySettings();
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        settingsProxy.IsDebugMode = true;
        Assert.That(settingsProxy.IsDebugMode, Is.True);
    }

    [Test]
    public void IsDarkMode_Should_ReturnCorrectValue()
    {
        var settings = new GlucoseTraySettings
        {
            IsDarkMode = true
        };
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        Assert.That(settingsProxy.IsDarkMode, Is.True);
    }

    [Test]
    public void IsDarkMode_Should_SetCorrectValue()
    {
        var settings = new GlucoseTraySettings();
        var options = Substitute.For<IOptionsMonitor<GlucoseTraySettings>>();
        options.CurrentValue.Returns(settings);
        var settingsProxy = new SettingsProxy(options);

        settingsProxy.IsDarkMode = true;
        Assert.That(settingsProxy.IsDarkMode, Is.True);
    }
}

