using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using GlucoseTray.Display;
using GlucoseTray.Read;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace GlucoseTray.Tests;

public class SecretLoggingTests
{
    private const string Secret = "SYNTHETIC-secret+/&=do-not-log";
    private const string Url = "https://example.test/private/SYNTHETIC-path?token=SYNTHETIC-secret%2B%2F%26%3Ddo-not-log&sessionId=SYNTHETIC-session";

    [TestCase(false)]
    [TestCase(true)]
    public async Task AdapterMustNotLogSecretUrlsBodiesOrExceptions(bool transportFailure)
    {
        using var logs = new CapturedLogs();
        var factory = Substitute.For<IHttpClientFactory>();
        factory.CreateClient(Arg.Any<string>()).Returns(_ => new HttpClient(new FakeHandler(transportFailure)));
        var adapter = new ExternalCommunicationAdapter(factory, logs.Factory.CreateLogger<ExternalCommunicationAdapter>());
        Assert.ThrowsAsync<HttpRequestException>(() => adapter.PostApiResponseAsync(Url, Secret));
        AssertNoSecrets(logs.Text);
        Assert.That(logs.Text, Is.Not.Empty);
        await Task.CompletedTask;
    }

    [Test]
    public async Task ReaderMustNotLogExceptionMessagesInnerExceptionsOrData()
    {
        using var logs = new CapturedLogs();
        var strategy = Substitute.For<IReadStrategy>();
        strategy.GetLatestGlucoseAsync().Returns(Task.FromException<GlucoseReading>(SecretException()));
        var factory = Substitute.For<IReadStrategyFactory>();
        factory.Create().Returns(strategy);
        var reader = new GlucoseReader(factory, logs.Factory.CreateLogger<GlucoseReader>());
        var reading = await reader.GetLatestGlucoseAsync();
        AssertNoSecrets(logs.Text);
    }

    [Test]
    public void RefreshLoopMustNotLogRawFatalException()
    {
        using var logs = new CapturedLogs();
        var tray = Substitute.For<ITray>();
        tray.When(t => t.Refresh(Arg.Any<GlucoseReading>())).Do(_ => throw SecretException());
        var reader = Substitute.For<IGlucoseReader>();
        reader.GetLatestGlucoseAsync().Returns(Task.FromResult(new GlucoseReading()));
        var options = Substitute.For<IOptionsMonitor<AppSettings>>();
        options.CurrentValue.Returns(new AppSettings());
        using var app = new AppRunner(tray, reader, options, Substitute.For<ICredentialMigrator>(), logs.Factory.CreateLogger<AppRunner>());
        Assert.ThrowsAsync<HttpRequestException>(() => app.Start());
        AssertNoSecrets(logs.Text);
        Assert.That(logs.Text, Is.Not.Empty);
    }

    [TestCase(ExternalCommunicationAdapter.HttpClientName, 200, false)]
    [TestCase(ExternalCommunicationAdapter.HttpClientName, 401, false)]
    [TestCase(ExternalCommunicationAdapter.HttpClientName, 503, false)]
    [TestCase(ExternalCommunicationAdapter.HttpClientName, 503, true)]
    public async Task ProductionHttpPipelineMustNotLogRequestsOrHeadersEvenAtTrace(string clientName, int status, bool transportFailure)
    {
        using var logs = new CapturedLogs();
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace).AddProvider(logs));
        typeof(Program).GetMethod("ConfigureServices", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, [config, services]);
        services.AddHttpClient(clientName).ConfigurePrimaryHttpMessageHandler(() => new FakeHandler(transportFailure, (HttpStatusCode)status));
        using var provider = services.BuildServiceProvider();
        using var client = provider.GetRequiredService<IHttpClientFactory>().CreateClient(clientName);
        using var request = new HttpRequestMessage(HttpMethod.Post, Url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Secret)));
        request.Headers.Add("X-Private", Secret);
        request.Content = new StringContent(Secret);
        if (transportFailure) Assert.ThrowsAsync<HttpRequestException>(() => client.SendAsync(request));
        else
        {
            using var response = await client.SendAsync(request);
            Assert.That((int)response.StatusCode, Is.EqualTo(status));
        }
        AssertNoSecrets(logs.Text);
        Assert.That(logs.Text, Does.Not.Contain("System.Net.Http.HttpClient"));
        Assert.That(logs.Text, Does.Contain(status.ToString()), "Safe status diagnostics should remain useful.");
    }

    [TestCase("http://example.test/?token=SYNTHETIC-token")]
    [TestCase("https://SYNTHETIC-user:SYNTHETIC-password@example.test/")]
    public void UnsafeCredentialDestinationsMustBeRejectedBeforeTransport(string url)
    {
        using var logs = new CapturedLogs();
        var transport = new FakeHandler(false, HttpStatusCode.OK);
        using var client = new HttpClient(new SafeHttpDiagnosticsHandler(logs.Factory.CreateLogger<SafeHttpDiagnosticsHandler>()) { InnerHandler = transport });
        Assert.ThrowsAsync<HttpRequestException>(() => client.PostAsync(url, new StringContent(Secret)));
        Assert.That(transport.RequestCount, Is.Zero);
        AssertNoSecrets(logs.Text);
    }

    [TestCase(ExternalCommunicationAdapter.HttpClientName)]
    public void ProductionClientsMustNotAutomaticallyForwardCredentialsThroughRedirects(string name)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        typeof(Program).GetMethod("ConfigureServices", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null,
            [new ConfigurationBuilder().AddInMemoryCollection().Build(), services]);
        using var provider = services.BuildServiceProvider();
        var handler = provider.GetRequiredService<IHttpMessageHandlerFactory>().CreateHandler(name);
        while (handler is DelegatingHandler delegating) handler = delegating.InnerHandler!;
        Assert.That(handler, Is.TypeOf<HttpClientHandler>());
        Assert.That(((HttpClientHandler)handler).AllowAutoRedirect, Is.False);
    }

    [Test]
    public async Task ConfigurationRefreshMustNotLogRawMigrationExceptions()
    {
        using var logs = new CapturedLogs();
        var migrator = Substitute.For<ICredentialMigrator>();
        migrator.ProtectFile(Arg.Any<string>()).Returns(_ => throw SecretException());
        using var app = new AppRunner(Substitute.For<ITray>(), Substitute.For<IGlucoseReader>(),
            Substitute.For<IOptionsMonitor<AppSettings>>(), migrator, logs.Factory.CreateLogger<AppRunner>());
        var method = typeof(AppRunner).GetMethod("RefreshOnConfigChangeAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;
        await (Task)method.Invoke(app, null)!;
        Assert.That(logs.Text, Is.Not.Empty);
        AssertNoSecrets(logs.Text);
    }

    private static HttpRequestException SecretException()
    {
        var exception = new HttpRequestException(Secret + " " + Url, new Exception(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Secret))), HttpStatusCode.ServiceUnavailable);
        exception.Data["Authorization"] = Secret;
        return exception;
    }

    private static void AssertNoSecrets(string text)
    {
        Assert.That(text, Does.Not.Contain("SYNTHETIC"));
        Assert.That(text, Does.Not.Contain(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Secret))));
        Assert.That(text, Does.Not.Contain("example.test"));
    }

    private sealed class FakeHandler(bool transportFailure, HttpStatusCode status = HttpStatusCode.ServiceUnavailable) : HttpMessageHandler
    {
        public int RequestCount { get; private set; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            RequestCount++;
            if (transportFailure) throw SecretException();
            var response = new HttpResponseMessage(status) { Content = new StringContent(Secret), ReasonPhrase = "SYNTHETIC-private-reason" };
            response.Headers.Add("X-Private", Secret);
            return Task.FromResult(response);
        }
    }

    private sealed class CapturedLogs : ILoggerProvider
    {
        private readonly List<string> _messages = [];
        public ILoggerFactory Factory { get; }
        public string Text { get { lock (_messages) return string.Join("\n", _messages); } }
        public CapturedLogs() => Factory = LoggerFactory.Create(b => b.SetMinimumLevel(LogLevel.Trace).AddProvider(this));
        public ILogger CreateLogger(string categoryName) => new Capture(this, categoryName);
        public void Dispose() { }
        private sealed class Capture(CapturedLogs owner, string category) : ILogger
        {
            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
            public bool IsEnabled(LogLevel level) => true;
            public void Log<TState>(LogLevel level, EventId id, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                var fields = state is IEnumerable<KeyValuePair<string, object?>> values ?
                    string.Join(";", values.Select(v => v.Key + "=" + v.Value)) : string.Empty;
                lock (owner._messages) owner._messages.Add(category + " " + formatter(state, exception) + " " + fields + " " + exception);
            }
        }
    }
}
