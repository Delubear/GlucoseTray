using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using GlucoseTray;
using GlucoseTray.Display;
using System.Text.Json;
using GlucoseTray.Read;
using GlucoseTray.Read.LibreLinkUp;

public class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        var filePath = AppSettings.FileName;
        if (!File.Exists(filePath))
            CreateDefaultAppSettings(filePath);

        var credentialStore = new WindowsLibreLinkUpCredentialStore();
        LibreLinkUpAccountSettings.MigrateLegacyPassword(filePath, credentialStore, new DpapiCredentialProtector());
        ProtectCredentialsAtRest(filePath);

        var host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, builder) => builder.AddJsonFile(AppSettings.FileName, optional: false, reloadOnChange: true))
            .ConfigureServices(static (context, services) => ConfigureServices(context.Configuration, services))
            .Build();

        var services = host.Services;
        var settings = services.GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<AppSettings>>().CurrentValue;
        if (settings.DataSource == GlucoseTray.Enums.GlucoseSource.LibreLinkUp &&
            string.IsNullOrEmpty(services.GetRequiredService<ILibreLinkUpCredentialStore>().GetPassword(settings.LibreLinkUpUsername)))
            services.GetRequiredService<ILibreLinkUpSignIn>().ShowDialog();
        var app = services.GetRequiredService<AppWrapper>();
        Application.Run(app);
    }

    private static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        services.AddHttpClient(ExternalCommunicationAdapter.HttpClientName, client => client.Timeout = TimeSpan.FromSeconds(15));
        services.AddHttpClient(LibreLinkUpClient.HttpClientName, client => client.Timeout = TimeSpan.FromSeconds(15))
            .RemoveAllLoggers()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false });
        services.AddSingleton<ILibreLinkUpClient, LibreLinkUpClient>();
        services.AddSingleton<ILibreLinkUpCredentialStore, WindowsLibreLinkUpCredentialStore>();
        services.AddSingleton<ILibreLinkUpSignIn, LibreLinkUpSignIn>();

        services.AddSingleton<ICredentialProtector, DpapiCredentialProtector>();
        services.AddSingleton<ICredentialMigrator, CredentialMigrator>();

        services.Configure<AppSettings>(configuration)
                .AddSingleton<AppWrapper>()
                .AddSingleton<AppRunner>()
                .AddSingleton<IGlucoseReader, GlucoseReader>()
                .AddSingleton<IReadStrategyFactory, ReadStrategyFactory>()
                .AddSingleton<IExternalCommunicationAdapter, ExternalCommunicationAdapter>()
                .AddSingleton<ITray, Tray>()
                .AddSingleton<ITrayIcon, NotificationIcon>()
                .AddSingleton<IScheduler, TaskSchedulerService>()
                .AddSingleton<IAlertService, AlertService>()
                .AddSingleton<IGlucoseReadingMapper, GlucoseReadingMapper>()
                .AddSingleton<IGlucoseDisplayMapper, GlucoseDisplayMapper>();
    }

    private static void CreateDefaultAppSettings(string filePath)
    {
        var settings = new AppSettings();
        var json = JsonSerializer.Serialize(settings, AppSettingsJson.SerializerOptions);
        File.WriteAllText(filePath, json);
    }

    private static void ProtectCredentialsAtRest(string filePath) => new CredentialMigrator(new DpapiCredentialProtector()).ProtectFile(filePath);
}
