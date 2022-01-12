using GlucoseTrayCore.Services;
using GlucoseTrayCore.Views.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;

namespace GlucoseTrayCore
{
    public class Program
    {
        private static IConfiguration Configuration { get; set; }
        public static string SettingsFile { get; set; }

        [STAThread]
        private static void Main(string[] args)
        {
            if (!LoadApplicationSettings())
                return;

            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, builder) => builder.AddJsonFile(SettingsFile, optional: false, reloadOnChange: true))
                .ConfigureServices((context, services) => ConfigureServices(context.Configuration, services))
                .Build();

            Application.ThreadException += ApplicationThreadException;
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

            var services = host.Services;

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
                    .AddScoped<TaskSchedulerService, TaskSchedulerService>()
                    .AddScoped<IGlucoseFetchService, GlucoseFetchService>();
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

        private static void ApplicationThreadException(object sender, ThreadExceptionEventArgs e) => MessageBox.Show(JsonSerializer.Serialize(e), "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}