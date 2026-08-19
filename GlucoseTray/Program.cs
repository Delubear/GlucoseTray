using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using GlucoseTray;
using GlucoseTray.Display;
using System.Text.Json;
using GlucoseTray.Read;
using System.Text.Json.Serialization;

public class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        var filePath = "appsettings.json";
        if (!File.Exists(filePath))
            CreateDefaultAppSettings(filePath);

        ProtectCredentialsAtRest(filePath);

        var host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, builder) => builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true))
            .ConfigureServices(static (context, services) => ConfigureServices(context.Configuration, services))
            .Build();

        var services = host.Services;
        var app = services.GetRequiredService<AppWrapper>();
        Application.Run(app);
    }

    private static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        services.AddHttpClient(ExternalCommunicationAdapter.HttpClientName, client => client.Timeout = TimeSpan.FromSeconds(15));

        services.AddSingleton<ICredentialProtector, DpapiCredentialProtector>();

        services.Configure<AppSettings>(configuration)
                .AddHttpClient()
                .AddSingleton<AppWrapper>()
                .AddSingleton<AppRunner>()
                .AddScoped<IGlucoseReader, GlucoseReader>()
                .AddScoped<IExternalCommunicationAdapter, ExternalCommunicationAdapter>()
                .AddScoped<ITray, Tray>()
                .AddScoped<ITrayIcon, NotificationIcon>()
                .AddScoped<IScheduler, TaskSchedulerService>()
                .AddScoped<IAlertService, AlertService>()
                .AddScoped<IGlucoseReadingMapper, GlucoseReadingMapper>()
                .AddScoped<IGlucoseDisplayMapper, GlucoseDisplayMapper>();
    }

    private static JsonSerializerOptions GetJsonSerializerOptions() => new()
    {
        Converters = { new JsonStringEnumConverter() },
        WriteIndented = true,
    };

    private static void CreateDefaultAppSettings(string filePath)
    {
        var settings = new AppSettings();
        var options = GetJsonSerializerOptions();
        var json = JsonSerializer.Serialize(settings, options);
        File.WriteAllText(filePath, json);
    }

    private static void ProtectCredentialsAtRest(string filePath)
    {
        var protector = new DpapiCredentialProtector();
        var options = GetJsonSerializerOptions();
        var settings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(filePath), options);
        if (settings is null)
            return;

        var changed = false;
        if (!string.IsNullOrEmpty(settings.DexcomPassword) && !protector.IsProtected(settings.DexcomPassword))
        {
            settings.DexcomPassword = protector.Protect(settings.DexcomPassword);
            changed = true;
        }
        if (!string.IsNullOrEmpty(settings.NightscoutToken) && !protector.IsProtected(settings.NightscoutToken))
        {
            settings.NightscoutToken = protector.Protect(settings.NightscoutToken);
            changed = true;
        }

        if (changed)
            File.WriteAllText(filePath, JsonSerializer.Serialize(settings, options));
    }
}
