using Dexcom.Fetch;
using Dexcom.Fetch.Models;
using GlucoseTrayCore.Data;
using GlucoseTrayCore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Sqlite.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.IO;
using System.Windows.Forms;

namespace GlucoseTrayCore
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, builder) => builder.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true))
                .ConfigureServices((context, services) => ConfigureServices(context.Configuration, services))
                .ConfigureLogging((context, logging) =>
                {
                    var switcher = new LoggingLevelSwitch(LogEventLevel.Verbose);
                    Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.ControlledBy(switcher)
                    .WriteTo.SQLite(Constants.DatabaseLocation, "Logs", storeTimestampInUtc: true, retentionPeriod: TimeSpan.FromDays(180))
                    .CreateLogger();
                    switcher.MinimumLevel = Constants.LogLevel;
                    logging.AddSerilog(Log.Logger);
                })
                .Build();

            var services = host.Services;
            var context = services.GetRequiredService<IGlucoseTrayDbContext>();
            context.Database.EnsureCreated();
            var creatorService = (SqliteDatabaseCreator) context.Database.GetService<IDatabaseCreator>();
            creatorService.CreateTables();
            var app = services.GetRequiredService<AppContext>();
            Application.Run(app);
        }

        private static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            Constants.config = configuration;
            
            services.AddLogging(o => o.AddSerilog())
            .AddScoped<AppContext, AppContext>()
            .AddScoped<IconService, IconService>()
            .AddScoped<IGlucoseFetchService, GlucoseFetchService>()
            .AddScoped<GlucoseFetchConfiguration, GlucoseFetchConfiguration>(o => new GlucoseFetchConfiguration
            {
                DexcomServer = Constants.DexcomServer,
                DexcomUsername = Constants.DexcomUsername,
                DexcomPassword = Constants.DexcomPassword,
                FetchMethod = Constants.FetchMethod,
                NightscoutUrl = Constants.NightscoutUrl,
                NightscoutAccessToken = Constants.AccessToken,
                UnitDisplayType = Constants.GlucoseUnitType
            })
            .AddDbContext<IGlucoseTrayDbContext, SQLiteDbContext>(o => o.UseSqlite("Data Source=" + Constants.DatabaseLocation));
        }
    }
}