using System.Configuration;

namespace GlucoseTray
{
    public static class Constants
    {
        public static string NightscoutUrl => ConfigurationManager.AppSettings["NightscoutUrl"];
        public static string ApiSecret => ConfigurationManager.AppSettings["ApiSecret"];
        public static int HighBg => int.Parse(ConfigurationManager.AppSettings["HighBg"]);
        public static int DangerHighBg => int.Parse(ConfigurationManager.AppSettings["DangerHighBg"]);
        public static int LowBg => int.Parse(ConfigurationManager.AppSettings["LowBg"]);
        public static int DangerLowBg => int.Parse(ConfigurationManager.AppSettings["DangerLowBg"]);
        public static int CriticalLowBg => int.Parse(ConfigurationManager.AppSettings["CriticalLowBg"]);
        public static string ErrorLogPath => ConfigurationManager.AppSettings["ErrorLogPath"];
    }
}
