using System;
namespace GlucoseTray.Models
{
    public interface ILogService
    {
        void Log(string logMessage);
        void Log(Exception exception);
    }
}
