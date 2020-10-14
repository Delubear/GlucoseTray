using GlucoseTrayCore.Data;
using GlucoseTrayCore.Services;
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
using System.Windows.Forms;

namespace GlucoseTrayCore
{
    internal class Program
    {
        private static IConfigurationSection Configuration { get; set; }

        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, builder) => builder.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true))
                .ConfigureServices((context, services) => ConfigureServices(context.Configuration, services))
                .ConfigureLogging((context, logging) =>
                {
                    Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Is(Configuration.GetValue<LogEventLevel>(nameof(GlucoseTraySettings.LogLevel)))
                    .WriteTo.SQLite(Configuration.GetValue<string>(nameof(GlucoseTraySettings.DatabaseLocation)), "Logs", storeTimestampInUtc: true, retentionPeriod: TimeSpan.FromDays(180))
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // Limit EF/MS logging noise
                    .CreateLogger();
                    logging.AddSerilog(Log.Logger);
                })
                .Build();

            var services = host.Services;

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

            var app = services.GetRequiredService<AppContext>();
            Application.Run(app);
        }

        private static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            Configuration = configuration.GetSection("appsettings");
            services.Configure<GlucoseTraySettings>(Configuration)
                    .AddHttpClient()
                    .AddScoped<AppContext, AppContext>()
                    .AddScoped<IconService, IconService>()
                    .AddScoped<IGlucoseFetchService, GlucoseFetchService>()
                    .AddDbContext<IGlucoseTrayDbContext, SQLiteDbContext>(o => o.UseSqlite("Data Source=" + Configuration.GetValue<string>(nameof(GlucoseTraySettings.DatabaseLocation))));
        }
    }
}