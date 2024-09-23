using GlucoseTray.Views.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;

namespace GlucoseTray;

public class Program
{
    private static IConfiguration? Configuration { get; set; }
    public static string SettingsFile { get; set; } = string.Empty;
    public static AppSettings AppSettings { get; set; } = new();

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

        AppSettings = GetAppSettings();

        var app = services.GetRequiredService<AppContext>();
        Application.Run(app);
    }

    private static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        Configuration = configuration;
        services.Configure<GlucoseTraySettings>(Configuration)
                .AddHttpClient()
                .AddScoped<AppContext, AppContext>()
                .AddScoped<IconService, IconService>()
                .AddScoped<UrlAssembler, UrlAssembler>()
                .AddScoped<UiService, UiService>()
                .AddScoped<TaskSchedulerService, TaskSchedulerService>()
                .AddScoped<INightscoutService, NightscoutService>()
                .AddScoped<IDexcomService, DexcomService>()
                .AddScoped<AlertService, AlertService>()
                .AddScoped<IExternalCommunicationAdapter, ExternalCommunicationAdapter>()
                .AddScoped<DebugService, DebugService>()
                .AddScoped<IGlucoseFetchService, GlucoseFetchService>();
    }

    private static AppSettings GetAppSettings()
    {
        var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("GlucoseTray.appsettings.json") ?? throw new NullReferenceException();
        var container = JsonSerializer.Deserialize<AppSettingsContainer>(resource) ?? new AppSettingsContainer();
        return container.AppSettings;
    }

    private static bool LoadApplicationSettings()
    {
        Environment.SetEnvironmentVariable("windir", Environment.GetEnvironmentVariable("SystemRoot"), EnvironmentVariableTarget.User);
        SettingsFile = Application.UserAppDataPath + @"\glucose_tray_settings.json";
        if (!File.Exists(SettingsFile) || SettingsService.ValidateSettings().Count != 0)
        {
            var settingsWindow = new SettingsWindow();
            if (settingsWindow.ShowDialog() != true) // Did not want to setup application.
            {
                Application.Exit();
                return false;
            }
        }
        return true;
    }

    private static void ApplicationThreadException(object sender, ThreadExceptionEventArgs e) => MessageBox.Show(e?.Exception?.Message + " --- " + e?.Exception?.InnerException?.Message, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
}