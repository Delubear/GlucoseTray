using GlucoseTray.Services;
using GlucoseTray.Views.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;

namespace GlucoseTray
{
    public class Program
    {
        static IConfiguration Configuration { get; set; }
        public static string SettingsFile { get; set; }
        public static AppSettings AppSettings { get; set; }

        [STAThread]
        static void Main(string[] args)
        {
            if (!LoadApplicationSettings())
                return;

            IHost host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, builder) => builder.AddJsonFile(SettingsFile, optional: false, reloadOnChange: true))//.AddJsonFile("GlucoseTray.Properties.Resources.appsettings.json", optional: false))
                .ConfigureServices((context, services) => ConfigureServices(context.Configuration, services))
                .Build();

            Application.ThreadException += ApplicationThreadException;
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

            IServiceProvider services = host.Services;

            AppSettings = GetAppSettings();

            AppContext app = services.GetRequiredService<AppContext>();
            Application.Run(app);
        }

        static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            Configuration = configuration;
            services.Configure<GlucoseTraySettings>(Configuration)
                    .AddHttpClient()
                    .AddScoped<AppContext, AppContext>()
                    .AddScoped<IconService, IconService>()
                    .AddScoped<UrlAssembler, UrlAssembler>()
                    .AddScoped<UiService, UiService>()
                    .AddScoped<TaskSchedulerService, TaskSchedulerService>()
                    .AddScoped<IGlucoseFetchService, GlucoseFetchService>();
        }

        static AppSettings GetAppSettings()
        {
            Stream resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("GlucoseTray.appsettings.json");
            AppSettingsContainer container = JsonSerializer.Deserialize<AppSettingsContainer>(resource);
            return container.AppSettings;
        }

        static bool LoadApplicationSettings()
        {
            Environment.SetEnvironmentVariable("windir", Environment.GetEnvironmentVariable("SystemRoot"), EnvironmentVariableTarget.User);
            SettingsFile = Application.UserAppDataPath + @"\glucose_tray_settings.json";

            if (!File.Exists(SettingsFile) || SettingsService.ValidateSettings().Count != 0)
            {
                SettingsWindow settingsWindow = new SettingsWindow();
                if (settingsWindow.ShowDialog() != true) // Did not want to setup application.
                {
                    Application.Exit();
                    return false;
                }
            }
            return true;
        }

        static void ApplicationThreadException(object sender, ThreadExceptionEventArgs e) => MessageBox.Show(JsonSerializer.Serialize(e), "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}