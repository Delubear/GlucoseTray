using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text.Json;
using GlucoseTray.Display;
using GlucoseTray.Enums;
using GlucoseTray.Read;
using GlucoseTray.Read.LibreLinkUp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace GlucoseTray.Tests;

public class LibreLinkUpPipelineTests
{
    private static string Login() => JsonSerializer.Serialize(new { status = 0, data = new { user = new { id = "synthetic-review-user" }, authTicket = new { token = "synthetic-token", expires = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds() } } });
    private static string Reading(int mg, int ageMinutes = 0, string patient = "patient-a") => JsonSerializer.Serialize(new {
        status = 0, data = new[] { new { patientId = patient, glucoseMeasurement = new {
            FactoryTimestamp = DateTime.UtcNow.AddMinutes(-ageMinutes).ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
            ValueInMgPerDl = mg, TrendArrow = 4
        } } }
    });

    private static (ServiceProvider Services, IConfigurationRoot Config, RecordingIcon Icon, FakeCloud Cloud) Build(params string[] payloads)
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?> {
            ["DataSource"] = "LibreLinkUp",
            ["LibreLinkUpUsername"] = "synthetic-review@example.test",
            ["LibreLinkUpRegion"] = "EU",
            ["DisplayUnitType"] = "Mmol",
            ["MinutesUntilStale"] = "15",
            ["EnableAlerts"] = "true",
        }).Build();
        var services = new ServiceCollection();
        services.AddLogging();
        typeof(Program).GetMethod("ConfigureServices",BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, [config, services]);
        var icon = new RecordingIcon();
        var cloud = new FakeCloud();
        foreach(var payload in payloads) cloud.Payloads.Enqueue(payload);
        services.AddSingleton<ITrayIcon>(icon);
        services.AddSingleton(Substitute.For<IScheduler>());
        var credentials = Substitute.For<ILibreLinkUpCredentialStore>();
        credentials.GetPassword(Arg.Any<string>()).Returns("synthetic-password");
        services.AddSingleton(credentials);
        services.AddHttpClient(LibreLinkUpClient.HttpClientName).ConfigurePrimaryHttpMessageHandler(() => cloud);
        return (services.BuildServiceProvider(), config, icon, cloud);
    }

    [Test]
    public async Task NormalReadingThroughProductionDIShouldReachTray()
    {
        var (services,_,icon,_) = Build(Login(), Reading(180));
        using(services) {
            await services.GetRequiredService<AppRunner>().Process();
            using(Assert.EnterMultipleScope()) {
                Assert.That(double.Parse(icon.Last!.DisplayValue.Replace('\'', '.'), CultureInfo.InvariantCulture), Is.EqualTo(10.0));
                Assert.That(icon.Last.IsStale, Is.False);
                Assert.That(icon.Last.Trend, Is.EqualTo(Trend.FortyFiveUp));
                Assert.That(icon.Notifications, Is.Empty);
            }
        }
    }

    [Test]
    public async Task HighReadingThroughProductionDIShouldProduceWindowsNotification()
    {
        var (services,_,icon,_) = Build(Login(), Reading(280));
        using(services) {
            await services.GetRequiredService<AppRunner>().Process();
            Assert.That(icon.Notifications, Is.EqualTo(new[] { "High Glucose Alert" }));
            Assert.That(icon.Last!.IsStale, Is.False);
        }
    }

    [Test]
    public async Task StaleHighReadingThroughProductionDIShouldNotAlert()
    {
        var (services,_,icon,_) = Build(Login(), Reading(280, ageMinutes: 16));
        using(services) {
            await services.GetRequiredService<AppRunner>().Process();
            Assert.That(icon.Last!.IsStale, Is.True);
            Assert.That(icon.Notifications, Is.Empty);
        }
    }

    [Test]
    public async Task CloudFailureForSamePatientShouldPreserveValueAndTimestamp()
    {
        var (services,_,icon,_) = Build(Login(), Reading(180, ageMinutes:16), "HTTP_503");
        using(services) {
            var app = services.GetRequiredService<AppRunner>();
            await app.Process(); var original = icon.Last!;
            await app.Process();
            Assert.That(icon.Last!.TimestampUtc, Is.EqualTo(original.TimestampUtc));
            Assert.That(icon.Last.DisplayValue, Is.EqualTo(original.DisplayValue));
            Assert.That(icon.Last.IsStale, Is.True);
        }
    }

    [Test]
    public async Task SwitchingPatientToMissingConnectionMustNotDisplayPreviousPatientsValue()
    {
        var (services,config,icon,_) = Build(Login(), Reading(180), Reading(90));
        using(services) {
            var app = services.GetRequiredService<AppRunner>();
            await app.Process();
            config["LibreLinkUpPatientId"] = "patient-b"; config.Reload();
            await app.Process();
            Assert.That(icon.Last!.DisplayValue, Is.EqualTo("NUL"), "The new patient has no reading; do not display patient-a as their reading.");
        }
    }

    [Test]
    public async Task AddingSecondPatientWithoutSelectionMustNotReusePreviousPatientsValue()
    {
        const string ambiguous = """{"status":0,"data":[{"patientId":"patient-a"},{"patientId":"patient-b"}]}""";
        var (services,_,icon,_) = Build(Login(), Reading(180), ambiguous);
        using(services) {
            var app = services.GetRequiredService<AppRunner>();
            await app.Process(); await app.Process();
            Assert.That(icon.Last!.DisplayValue, Is.EqualTo("NUL"), "Selection is now ambiguous; do not silently keep the former patient's reading.");
        }
    }

    [Test]
    public async Task SwitchingAccountWithFailedLoginMustNotReusePreviousAccountsValue()
    {
        var (services,config,icon,_) = Build(Login(), Reading(180), """{"status":2}""", """{"status":2}""");
        using(services) {
            var app = services.GetRequiredService<AppRunner>();
            await app.Process();
            config["LibreLinkUpUsername"] = "second-synthetic@example.test"; config.Reload();
            await app.Process();
            Assert.That(icon.Last!.DisplayValue, Is.EqualTo("NUL"), "The new account has no verified reading.");
        }
    }

    [Test]
    public async Task FailedLoginShouldGiveActionableTrayFeedback()
    {
        var (services,_,icon,_) = Build("""{"status":2}""", """{"status":2}""");
        using(services) {
            await services.GetRequiredService<AppRunner>().Process();
            Assert.That(icon.Notifications.Any(n => n.Contains("LibreLinkUp", StringComparison.OrdinalIgnoreCase)), Is.True,
                "The WinExe has no visible console; NUL alone cannot explain login/terms/patient-selection failures.");
        }
    }

    [Test]
    public async Task CurrentAlertsOnlyNotifyOnTransitionAndNeverRepeatWhileHigh()
    {
        var (services,_,icon,_) = Build(Login(), Reading(280), Reading(280));
        using(services) {
            var app=services.GetRequiredService<AppRunner>();
            await app.Process(); await app.Process();
            Assert.That(icon.Notifications, Has.Count.EqualTo(1));
        }
    }

    [Test]
    public async Task RepeatedLoginFailuresShouldNotifyOnceAndRemainVisibleInStatus()
    {
        var (services,_,icon,_) = Build("""{"status":2}""", """{"status":2}""", """{"status":2}""", """{"status":2}""");
        using (services)
        {
            var app = services.GetRequiredService<AppRunner>();
            await app.Process(); await app.Process();
            Assert.That(icon.Notifications, Has.Count.EqualTo(1));
            Assert.That(icon.Last!.GetDisplayMessage(DateTime.UtcNow), Does.Contain("LibreLinkUp sign-in failed"));
            Assert.That(icon.Last.IsStale, Is.True);
        }
    }

    [Test]
    public async Task RecoveryShouldClearConnectionError()
    {
        var (services,_,icon,_) = Build("""{"status":2}""", """{"status":2}""", Login(), Reading(180));
        using (services)
        {
            var app = services.GetRequiredService<AppRunner>();
            await app.Process(); await app.Process();
            Assert.That(icon.Last!.ConnectionError, Is.Null);
            Assert.That(double.Parse(icon.Last.DisplayValue.Replace('\'', '.'), CultureInfo.InvariantCulture), Is.EqualTo(10.0));
            Assert.That(icon.Notifications, Has.Count.EqualTo(1));
        }
    }

    [Test]
    public async Task SuccessfulAutomaticPatientChangeShouldResetHighAlertState()
    {
        var (services,_,icon,_) = Build(Login(), Reading(280), Reading(280, patient: "patient-b"));
        using (services)
        {
            var app = services.GetRequiredService<AppRunner>();
            await app.Process(); await app.Process();
            Assert.That(icon.Notifications, Is.EqualTo(new[] { "High Glucose Alert", "High Glucose Alert" }));
        }
    }

    [Test]
    public async Task RemovingAllConnectionsShouldDiscardCachedReading()
    {
        var (services,_,icon,_) = Build(Login(), Reading(180), """{"status":0,"data":[]}""");
        using (services)
        {
            var app = services.GetRequiredService<AppRunner>();
            await app.Process(); await app.Process();
            Assert.That(icon.Last!.DisplayValue, Is.EqualTo("NUL"));
            Assert.That(icon.Last.ConnectionError, Does.Contain("No LibreLinkUp connections"));
        }
    }

    [Test]
    public async Task PatientChangeDuringInFlightRequestShouldDiscardItsResult()
    {
        var (services,config,icon,cloud) = Build(Login(), Reading(180));
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        cloud.BeforeResponse = async request =>
        {
            if (request.Method == HttpMethod.Get)
            {
                entered.SetResult();
                await release.Task;
            }
        };
        using (services)
        {
            var pending = services.GetRequiredService<AppRunner>().Process();
            try
            {
                await entered.Task.WaitAsync(TimeSpan.FromSeconds(5));
                config["LibreLinkUpPatientId"] = "patient-b";
                config.Reload();
            }
            finally { release.TrySetResult(); }
            await pending;
            Assert.That(icon.Last!.DisplayValue, Is.EqualTo("NUL"));
            Assert.That(icon.Last.ConnectionError, Does.Contain("settings changed"));
        }
    }

    [Test]
    public async Task AutomaticPatientChangeWithMissingMeasurementMustDiscardOldPatient()
    {
        var (services,_,icon,_) = Build(Login(), Reading(180), """{"status":0,"data":[{"patientId":"patient-b"}]}""");
        using (services)
        {
            var app = services.GetRequiredService<AppRunner>();
            await app.Process(); await app.Process();
            Assert.That(icon.Last!.DisplayValue, Is.EqualTo("NUL"));
        }
    }

    [Test]
    public async Task SamePatientWithMissingMeasurementMayRetainOriginalReading()
    {
        var (services,_,icon,_) = Build(Login(), Reading(180), """{"status":0,"data":[{"patientId":"patient-a"}]}""");
        using (services)
        {
            var app = services.GetRequiredService<AppRunner>();
            await app.Process(); var original = icon.Last!.TimestampUtc;
            await app.Process();
            Assert.That(double.Parse(icon.Last!.DisplayValue.Replace('\'', '.'), CultureInfo.InvariantCulture), Is.EqualTo(10.0));
            Assert.That(icon.Last.TimestampUtc, Is.EqualTo(original));
            Assert.That(icon.Last.ConnectionError, Is.Not.Empty);
        }
    }

    [Test]
    public async Task RemovingStoredCredentialMustInvalidateSessionAndCachedReading()
    {
        var (services,_,icon,_) = Build(Login(), Reading(180));
        using (services)
        {
            var app = services.GetRequiredService<AppRunner>();
            await app.Process();
            services.GetRequiredService<ILibreLinkUpCredentialStore>().GetPassword(Arg.Any<string>()).Returns((string?)null);
            await app.Process();
            Assert.That(icon.Last!.DisplayValue, Is.EqualTo("NUL"));
            Assert.That(icon.Last.ConnectionError, Does.Contain("signed out"));
        }
    }

    private sealed class FakeCloud : HttpMessageHandler
    {
        public Queue<string> Payloads { get; } = new();
        public Func<HttpRequestMessage, Task>? BeforeResponse { get; set; }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            if (BeforeResponse is not null)
                await BeforeResponse(request);
            var payload = Payloads.Dequeue();
            return new HttpResponseMessage(payload == "HTTP_503" ? HttpStatusCode.ServiceUnavailable : HttpStatusCode.OK)
            {
                Content = new StringContent(payload == "HTTP_503" ? "" : payload),
            };
        }
    }

    private sealed class RecordingIcon : ITrayIcon
    {
        public GlucoseDisplay? Last { get; private set; }
        public List<string> Notifications { get; } = [];
        public void RefreshIcon(GlucoseDisplay display) => Last = display;
        public void ShowNotification(string text) => Notifications.Add(text);
        public void ClearMenu() {}
        public void AddAutoRunMenu(bool enabled, EventHandler handler) {}
        public void AddSettingsMenu() {}
        public void AddExitMenu() {}
        public void Dispose() {}
    }
}
