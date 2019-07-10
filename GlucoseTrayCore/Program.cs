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

            string configFile = string.Empty;
            if(Directory.GetFiles(Directory.GetCurrentDirectory(), "*.dll.config").Length < 1 && Directory.GetFiles(Directory.GetCurrentDirectory(), "*.exe.config").Length < 1)
            {
                MessageBox.Show("ERROR: Configuration File is missing.  Create or Add GlucoseTraycore.dll.config to executable directory.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new InvalidOperationException("Missing config file.");
            }
            else
            {
                foreach (var file in Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.dll.config"))
                {
                    if(Directory.GetFiles(Directory.GetCurrentDirectory(), "*.exe.config").Length < 1)
                    {
                        var exeConfigPath = file.Replace(".dll.", ".exe.");
                        File.Copy(file, exeConfigPath);
                        configFile = exeConfigPath.Replace(".config", "");
                    }
                    Constants.config = ConfigurationManager.OpenExeConfiguration(configFile);
                }
            }

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

            Constants.LogCurrentConfig(logger);
            logger.LogDebug(configFile);
            switcher.MinimumLevel = Constants.LogLevel;
            Application.Run(new AppContext(logger));
        }
    }
}
