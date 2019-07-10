using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace GlucoseTrayCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var switcher = new LoggingLevelSwitch(LogEventLevel.Verbose);
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(switcher)
                .WriteTo.File(Constants.ErrorLogPath, rollingInterval: RollingInterval.Day)
                .Enrich.WithProperty("process", "Worker")
                .CreateLogger();
            var loggerFactory = new LoggerFactory().AddSerilog(Log.Logger);

            var provider = new ServiceCollection()
                .AddOptions()
                .AddSingleton(loggerFactory)
                .BuildServiceProvider();

            var logger = provider.GetService<ILoggerFactory>().CreateLogger("Worker.Program");
            logger.LogDebug("Current directory:{CurrentDirectory}", Directory.GetCurrentDirectory());

            string configFile = string.Empty;

            foreach( var file in Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.dll.config"))
            {
                configFile = file;
            }

            if (string.IsNullOrWhiteSpace(configFile))
            {
                MessageBox.Show("ERROR: Configuration File is missing.  Create or Add GlucoseTraycore.dll.config to executable directory.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.LogCritical("ERROR: Configuration File is missing.");
                throw new InvalidOperationException("Missing config file.");
            }

            Constants.config = ConfigurationManager.OpenExeConfiguration(configFile);
            Constants.LogCurrentConfig(logger);
            logger.LogDebug(configFile);
            switcher.MinimumLevel = Constants.LogLevel;
            Application.Run(new AppContext(logger));
        }
    }
}
