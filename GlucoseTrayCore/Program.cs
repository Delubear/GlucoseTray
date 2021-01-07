using GlucoseTrayCore.Data;
using GlucoseTrayCore.Services;
using GlucoseTrayCore.Views.Settings;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Sqlite.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace GlucoseTrayCore
{
    internal class Program
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
                .ConfigureLogging((context, logging) =>
                {
                    Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Is(Configuration.GetValue<LogEventLevel>(nameof(GlucoseTraySettings.LogLevel)))
                    .WriteTo.SQLite(Configuration.GetValue<string>(nameof(GlucoseTraySettings.DatabaseLocation)), "Logs", storeTimestampInUtc: true, retentionPeriod: TimeSpan.FromDays(180), maxDatabaseSize: 0)
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // Limit EF/MS logging noise
                    .CreateLogger();
                    logging.AddSerilog(Log.Logger);
                })
                .Build();

            Application.ThreadException += ApplicationThreadException;

            var services = host.Services;
            InitializeDatabase(services);

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
                    .AddScoped<IGlucoseFetchService, GlucoseFetchService>()
                    .AddDbContext<IGlucoseTrayDbContext, SQLiteDbContext>(o => o.UseSqlite("Data Source=" + Configuration.GetValue<string>(nameof(GlucoseTraySettings.DatabaseLocation))));
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

        private static void InitializeDatabase(IServiceProvider services)
        {
            // Logger setup creates Database before we have a chance to run EnsureCreated with no table which would normally handle the GlucoseResults table.
            // Manually call CreateTables afterwards to prevent crashes from table not existing since we check it before writing our first record.
            var context = services.GetRequiredService<IGlucoseTrayDbContext>();
            context.Database.EnsureCreated();
            try
            {
                var creatorService = (SqliteDatabaseCreator) context.Database.GetService<IDatabaseCreator>();
                creatorService.CreateTables();
            }
            catch (SqliteException)
            {
                Log.Logger.Verbose("Tables already created.");
            }
        }

        private static void ApplicationThreadException(object sender, ThreadExceptionEventArgs e) => 
            Log.Logger.Fatal($"Unhandled Exception Thrown. {e?.Exception?.Message} ---- {e?.Exception?.InnerException?.Message}", e);
    }
}