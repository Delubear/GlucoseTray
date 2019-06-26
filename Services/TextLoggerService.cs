using System;
using System.IO;
using GlucoseTray.Models;

namespace GlucoseTray.Services
{
    public class TextLoggerService : ILogService
    {
        private string LogPath => Constants.ErrorLogPath;

        public void Log(string logMessage)
        {
            File.AppendAllText(LogPath, DateTime.Now.ToString() + logMessage + Environment.NewLine + Environment.NewLine);
        }

        public void Log(Exception exception)
        {
            File.AppendAllText(LogPath, DateTime.Now.ToString() + exception.Message + exception?.InnerException + exception.StackTrace + Environment.NewLine + Environment.NewLine);
        }
    }
}
