using System;

namespace GlucoseTray.Interfaces
{
    public interface ILogService
    {
        void Log(string logMessage);
        void Log(Exception exception);
    }
}
