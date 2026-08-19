using GlucoseTray;
using GlucoseTray.Display;
using GlucoseTray.Enums;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace GlucoseTray.Tests;

public class AlertServiceTests
{
    private static AlertService CreateService()
    {
        var options = Substitute.For<IOptionsMonitor<AppSettings>>();
        options.CurrentValue.Returns(new AppSettings
        {
            DisplayUnitType = GlucoseUnitType.Mg,
            CriticalLowMgThreshold = 55,
            LowMgThreshold = 70,
            HighMgThreshold = 250,
            CriticalHighMgThreshold = 300,
        });
        return new AlertService(options);
    }

    private const float NoMmol = 0f;

    [Test]
    public void ShouldReturnEmptyWhenReadingIsWithinNormalRange()
    {
        var service = CreateService();
        Assert.That(service.GetAlertMessage(120, 6.7f, isStale: false), Is.Empty);
    }

    [Test]
    public void ShouldReturnEmptyWhenStale()
    {
        var service = CreateService();
        Assert.That(service.GetAlertMessage(400, 22f, isStale: true), Is.Empty);
    }

    [Test]
    public void ShouldReturnEmptyWhenReadingIsZero()
    {
        var service = CreateService();
        Assert.That(service.GetAlertMessage(0, NoMmol, isStale: false), Is.Empty);
    }

    [Test]
    public void ShouldAlertOnceForHighThenSuppressRepeat()
    {
        var service = CreateService();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(service.GetAlertMessage(260, 14.4f, isStale: false), Is.EqualTo("High Glucose Alert"));
            Assert.That(service.GetAlertMessage(260, 14.4f, isStale: false), Is.Empty);
        }
    }

    [Test]
    public void ShouldEscalateFromHighToCriticalHigh()
    {
        var service = CreateService();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(service.GetAlertMessage(260, 14.4f, isStale: false), Is.EqualTo("High Glucose Alert"));
            Assert.That(service.GetAlertMessage(310, 17.2f, isStale: false), Is.EqualTo("Critical High Glucose Alert"));
        }
    }

    [Test]
    public void ShouldAlertOnceForLowThenSuppressRepeat()
    {
        var service = CreateService();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(service.GetAlertMessage(65, 3.6f, isStale: false), Is.EqualTo("Low Glucose Alert"));
            Assert.That(service.GetAlertMessage(65, 3.6f, isStale: false), Is.Empty);
        }
    }

    [Test]
    public void ShouldEscalateFromLowToCriticalLow()
    {
        var service = CreateService();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(service.GetAlertMessage(65, 3.6f, isStale: false), Is.EqualTo("Low Glucose Alert"));
            Assert.That(service.GetAlertMessage(50, 2.7f, isStale: false), Is.EqualTo("Critical Low Glucose Alert"));
        }
    }

    [Test]
    public void ShouldReArmAlertAfterReturningToNormal()
    {
        var service = CreateService();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(service.GetAlertMessage(260, 14.4f, isStale: false), Is.EqualTo("High Glucose Alert"));
            Assert.That(service.GetAlertMessage(120, 6.7f, isStale: false), Is.Empty);
            Assert.That(service.GetAlertMessage(260, 14.4f, isStale: false), Is.EqualTo("High Glucose Alert"));
        }
    }
}
