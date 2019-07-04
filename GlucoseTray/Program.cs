using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace GlucoseTray
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var switcher = new LoggingLevelSwitch(LogEventLevel.Verbose);
            Log.Logger = new LoggerConfiguration()
                //.Enrich.FromLogContext()
                .MinimumLevel.ControlledBy(switcher)
                .WriteTo.File(Constants.ErrorLogPath, rollingInterval: RollingInterval.Day)
                .Enrich.WithProperty("process", "Worker")
                .CreateLogger();
            var loggerFactory = new LoggerFactory().AddSerilog(Log.Logger);

            // TO-DO: Enable appsettings.json and serilog configs there.
            //IConfigurationRoot config = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile(path: "AppSettings.json", optional: false, reloadOnChange: true).Build();

            var provider = new ServiceCollection()
                .AddOptions()
                .AddSingleton(loggerFactory)
                .BuildServiceProvider();

            var logger = provider.GetService<ILoggerFactory>().CreateLogger("Worker.Program");
            logger.LogDebug("Current directory:{CurrentDirectory}", Directory.GetCurrentDirectory());

            var configFile = Application.ExecutablePath + ".config";
            if (!File.Exists(configFile))
            {
                MessageBox.Show("ERROR: Configuration File is missing.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.LogCritical("ERROR: Configuration File is missing.");
            }

            Constants.LogCurrentConfig(logger);
            switcher.MinimumLevel = Constants.LogLevel;

            Application.Run(new AppContext(logger));
        }
    }
}
