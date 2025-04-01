using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using GlucoseTray;
using GlucoseTray.Display;
using System.Text.Json;

public class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        var filePath = "appsettings.json";
        if (!File.Exists(filePath))
            CreateDefaultAppSettings(filePath);

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
        services.Configure<AppSettings>(configuration)
                .AddHttpClient()
                .AddSingleton<AppWrapper>()
                .AddSingleton<AppRunner>()
                .AddScoped<ITray, Tray>()
                .AddScoped<ITrayIcon, NotificationIcon>()
                .AddScoped<IGlucoseReader, GlucoseReader>()
                .AddScoped<IGlucoseDisplayMapper, GlucoseDisplayMapper>();
    }

    private static void CreateDefaultAppSettings(string filePath)
    {
        var settings = new AppSettings();
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }
}