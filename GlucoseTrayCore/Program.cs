using System.IO;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
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

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            Constants.config = builder.Build();

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
            switcher.MinimumLevel = Constants.LogLevel;
            Application.Run(new AppContext(logger));
        }
    }
}
