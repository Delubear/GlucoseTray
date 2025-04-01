using GlucoseTray.DisplayResults;
using GlucoseTray.FetchResults;
using GlucoseTray.GlucoseSettings;
using GlucoseTray.Settings;
using GlucoseTray.Views.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;

namespace GlucoseTray;

public class Program
{
    public static string SettingsFile { get; set; } = string.Empty; // TODO: This is primarily used in the GlucoseSettings namespace and should be moved there and see if we can remove the static.
    public static AppSettings AppSettings { get; set; } = new(); // TODO: This is only accessed from IconService and should be moved closer to there.

    [STAThread]
    private static void Main(string[] args)
    {
        if (!LoadApplicationSettings())
            return;

        var host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, builder) => builder.AddJsonFile(SettingsFile, optional: false, reloadOnChange: true))//.AddJsonFile("GlucoseTray.Properties.Resources.appsettings.json", optional: false))
            .ConfigureServices((context, services) => ConfigureServices(context.Configuration, services))
            .Build();

        Application.ThreadException += ApplicationThreadException;
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

        var services = host.Services;

        AppSettings = GetAppSettings(); // TODO: This doesn't need to be assigned here. It should be moved closer to the IconService.

        var app = services.GetRequiredService<AppContext>();
        Application.Run(app);
    }

    private static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        services.Configure<GlucoseTraySettings>(configuration)
                .AddHttpClient()
                .AddScoped<AppContext, AppContext>()
                .AddScoped<IRunner, Runner>()
                .AddScoped<GlucoseResult, GlucoseResult>()
                .RegisterGlucoseSettingsServices()
                .RegisterDisplayResultServices()
                .RegisterFetchResultsServices();
    }

    private static AppSettings GetAppSettings()
    {
        var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("GlucoseTray.appsettings.json") ?? throw new NullReferenceException();
        var container = JsonSerializer.Deserialize<AppSettingsContainer>(resource) ?? new AppSettingsContainer();
        return container.AppSettings;
    }

    private static bool LoadApplicationSettings() // TODO: This should mostly be moved to a service in the GlucoseSettings area.
    {
        Environment.SetEnvironmentVariable("windir", Environment.GetEnvironmentVariable("SystemRoot"), EnvironmentVariableTarget.User);
        SettingsFile = Application.UserAppDataPath + @"\glucose_tray_settings.json";
        var fileService = new FileService<GlucoseTraySettings>();
        var settingsService = new SettingsService(fileService);
        var dialogService = new DialogService();
        if (!fileService.DoesFileExist(SettingsFile) || settingsService.ValidateSettings().Count != 0)
        {
            var settingsWindowService = new SettingsWindowService(fileService, settingsService, dialogService);
            var settingsWindow = new SettingsWindow(settingsWindowService, dialogService);
            if (settingsWindow.ShowDialog() != true) // Did not want to setup application.
            {
                Application.Exit();
                return false;
            }
        }
        return true;
    }

    private static void ApplicationThreadException(object sender, ThreadExceptionEventArgs e)
    {
        var dialogService = new DialogService();
        dialogService.ShowCriticalAlert("An unexpected error occurred. Please restart the application. " + e?.Exception?.Message + " --- " + e?.Exception?.InnerException?.Message, "Fatal Error");
    }
}