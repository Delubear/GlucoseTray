using Microsoft.Win32.TaskScheduler;
using System.Diagnostics;

namespace GlucoseTrayCore.Services
{
    public class TaskSchedulerService
    {
        private static readonly string ExecutablePath = Process.GetCurrentProcess().MainModule.FileName; // Environment.ProcessPath should be available in the future for this

        public bool HasTaskEnabled()
        {
            using var ts = new TaskService();
            var existingTask = ts.GetTask("GlucoseTray");
            return existingTask?.Enabled == true;
        }

        public void ToggleTask(bool enable)
        {
            using var ts = new TaskService();
            var task = ts.GetTask("GlucoseTray") ?? ts.AddTask("GlucoseTray", QuickTriggerType.Logon, "\"" + ExecutablePath + "\"");
            task.Enabled = enable;
        }
    }
}
