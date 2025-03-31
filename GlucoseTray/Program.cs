using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using GlucoseTray;
using GlucoseTray.Display;

public class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, builder) => builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true))
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
}