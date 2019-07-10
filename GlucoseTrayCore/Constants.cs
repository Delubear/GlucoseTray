using System;
using System.Configuration;
using Dexcom.Fetch.Enums;
using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace GlucoseTrayCore
{
    public static class Constants
    {
        public static Configuration config { get; set; }

        public static FetchMethod FetchMethod => (FetchMethod)Convert.ToInt32(config.AppSettings.Settings["FetchMethod"].ToString());
        public static string NightscoutUrl => config.AppSettings.Settings["NightscoutUrl"].ToString();
        public static string DexcomUsername => config.AppSettings.Settings["DexcomUsername"].ToString();
        public static string DexcomPassword => config.AppSettings.Settings["DexcomPassword"].ToString();
        public static string AccessToken => config.AppSettings.Settings["AccessToken"].ToString();
        public static int HighBg => int.Parse(config.AppSettings.Settings["HighBg"].ToString());
        public static int DangerHighBg => int.Parse(config.AppSettings.Settings["DangerHighBg"].ToString());
        public static int LowBg => int.Parse(config.AppSettings.Settings["LowBg"].ToString());
        public static int DangerLowBg => int.Parse(config.AppSettings.Settings["DangerLowBg"].ToString());
        public static int CriticalLowBg => int.Parse(config.AppSettings.Settings["CriticalLowBg"].ToString());
        public static TimeSpan PollingThreshold => TimeSpan.FromSeconds(Convert.ToInt32(config.AppSettings.Settings["PollingThreshold"].ToString()));
        public static string ErrorLogPath => config.AppSettings.Settings["ErrorLogPath"].ToString();
        public static bool EnableDebugMode => Convert.ToBoolean(config.AppSettings.Settings["EnableDebugMode"].ToString());
        public static LogEventLevel LogLevel => (LogEventLevel)Convert.ToInt32(config.AppSettings.Settings["LogLevel"].ToString());

        public static void LogCurrentConfig(ILogger logger)
        {
            logger.LogDebug($"{nameof(FetchMethod)}: {FetchMethod}");
            logger.LogDebug($"{nameof(NightscoutUrl)}: {NightscoutUrl}");
            logger.LogDebug($"{nameof(DexcomUsername)}: {DexcomUsername}");
            logger.LogDebug($"{nameof(DexcomPassword)}: {DexcomPassword}");
            logger.LogDebug($"{nameof(AccessToken)}: {AccessToken}");
            logger.LogDebug($"{nameof(HighBg)}: {HighBg}");
            logger.LogDebug($"{nameof(DangerHighBg)}: {DangerHighBg}");
            logger.LogDebug($"{nameof(LowBg)}: {LowBg}");
            logger.LogDebug($"{nameof(DangerLowBg)}: {DangerLowBg}");
            logger.LogDebug($"{nameof(CriticalLowBg)}: {CriticalLowBg}");
            logger.LogDebug($"{nameof(PollingThreshold)}: {PollingThreshold}");
            logger.LogDebug($"{nameof(ErrorLogPath)}: {ErrorLogPath}");
            logger.LogDebug($"{nameof(EnableDebugMode)}: {EnableDebugMode}");
            logger.LogDebug($"{nameof(LogLevel)}: {LogLevel}");
        }
    }
}
