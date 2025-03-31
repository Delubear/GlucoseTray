using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using GlucoseTray;

public class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(static (context, services) => ConfigureServices(context.Configuration, services))
            .Build();

        var services = host.Services;
        var app = services.GetRequiredService<AppWrapper>();
        Application.Run(app);
    }

    private static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        services.AddSingleton<AppWrapper>()
                .AddSingleton<AppRunner>()
                .AddScoped<ITray, Tray>()
                .AddScoped<ITrayIcon, NotificationIcon>()
                .AddScoped<IGlucoseReader, GlucoseReader>();

    }
}