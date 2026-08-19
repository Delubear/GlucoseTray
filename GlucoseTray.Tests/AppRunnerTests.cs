using GlucoseTray.Display;
using GlucoseTray.Read;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace GlucoseTray.Tests;

public class AppRunnerTests
{
    [Test]
    public async Task ShouldSerializeConcurrentProcessCalls()
    {
        var reader = new ConcurrencyTrackingReader();
        var tray = Substitute.For<ITray>();
        var options = new TestOptionsMonitor(new AppSettings());
        var runner = new AppRunner(tray, reader, options, Substitute.For<ICredentialMigrator>(), NullLogger<AppRunner>.Instance);

        var calls = Enumerable.Range(0, 10).Select(_ => runner.Process());
        await Task.WhenAll(calls);

        Assert.That(reader.MaxConcurrency, Is.EqualTo(1));
    }

    [Test]
    public async Task ShouldRefreshTrayWhenProcessRuns()
    {
        var reading = new GlucoseReading();
        var reader = Substitute.For<IGlucoseReader>();
        reader.GetLatestGlucoseAsync().Returns(reading);
        var tray = Substitute.For<ITray>();
        var options = new TestOptionsMonitor(new AppSettings());
        var runner = new AppRunner(tray, reader, options, Substitute.For<ICredentialMigrator>(), NullLogger<AppRunner>.Instance);

        await runner.Process();

        tray.Received(1).Refresh(reading);
    }

    [Test]
    public async Task ShouldRefreshTrayWhenConfigurationChanges()
    {
        var reader = Substitute.For<IGlucoseReader>();
        reader.GetLatestGlucoseAsync().Returns(new GlucoseReading());
        var tray = Substitute.For<ITray>();
        var options = new TestOptionsMonitor(new AppSettings());
        var runner = new AppRunner(tray, reader, options, Substitute.For<ICredentialMigrator>(), NullLogger<AppRunner>.Instance);

        _ = runner.Start();
        options.TriggerChange(new AppSettings());
        await WaitForAsync(() => tray.ReceivedCalls().Any(c => c.GetMethodInfo().Name == nameof(ITray.Refresh)));

        tray.Received().Refresh(Arg.Any<GlucoseReading>());
        runner.Dispose();
    }

    [Test]
    public async Task ShouldReEncryptCredentialsWhenConfigurationChanges()
    {
        var reader = Substitute.For<IGlucoseReader>();
        reader.GetLatestGlucoseAsync().Returns(new GlucoseReading());
        var tray = Substitute.For<ITray>();
        var migrator = Substitute.For<ICredentialMigrator>();
        var options = new TestOptionsMonitor(new AppSettings());
        var runner = new AppRunner(tray, reader, options, migrator, NullLogger<AppRunner>.Instance);

        _ = runner.Start();
        options.TriggerChange(new AppSettings());
        await WaitForAsync(() => migrator.ReceivedCalls().Any(c => c.GetMethodInfo().Name == nameof(ICredentialMigrator.ProtectFile)));

        migrator.Received().ProtectFile(AppSettings.FileName);
        runner.Dispose();
    }

    [Test]
    public void ShouldStopHandlingConfigChangesAfterDispose()
    {
        var reader = Substitute.For<IGlucoseReader>();
        reader.GetLatestGlucoseAsync().Returns(new GlucoseReading());
        var tray = Substitute.For<ITray>();
        var options = new TestOptionsMonitor(new AppSettings());
        var runner = new AppRunner(tray, reader, options, Substitute.For<ICredentialMigrator>(), NullLogger<AppRunner>.Instance);

        _ = runner.Start();
        runner.Dispose();
        Assert.That(options.ListenerCount, Is.Zero);
    }

    private static async Task WaitForAsync(Func<bool> condition, int timeoutMs = 2000)
    {
        var start = DateTime.UtcNow;
        while (!condition() && (DateTime.UtcNow - start).TotalMilliseconds < timeoutMs)
            await Task.Delay(10);
    }

    private sealed class ConcurrencyTrackingReader : IGlucoseReader
    {
        private int _current;
        public int MaxConcurrency { get; private set; }

        public async Task<GlucoseReading> GetLatestGlucoseAsync()
        {
            var now = Interlocked.Increment(ref _current);
            MaxConcurrency = Math.Max(MaxConcurrency, now);
            await Task.Delay(20);
            Interlocked.Decrement(ref _current);
            return new GlucoseReading();
        }
    }

    private sealed class TestOptionsMonitor(AppSettings value) : IOptionsMonitor<AppSettings>
    {
        private readonly List<Action<AppSettings, string?>> _listeners = [];

        public AppSettings CurrentValue { get; private set; } = value;
        public int ListenerCount => _listeners.Count;

        public AppSettings Get(string? name) => CurrentValue;

        public IDisposable OnChange(Action<AppSettings, string?> listener)
        {
            _listeners.Add(listener);
            return new Unsubscriber(_listeners, listener);
        }

        public void TriggerChange(AppSettings newValue)
        {
            CurrentValue = newValue;
            foreach (var listener in _listeners.ToArray())
                listener(newValue, null);
        }

        private sealed class Unsubscriber(List<Action<AppSettings, string?>> listeners, Action<AppSettings, string?> listener) : IDisposable
        {
            public void Dispose() => listeners.Remove(listener);
        }
    }
}
