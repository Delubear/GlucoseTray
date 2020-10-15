using Microsoft.Win32.TaskScheduler;
using System;
using System.Diagnostics;
using System.IO;

namespace GlucoseTrayCore.Services
{
    public class TaskSchedulerService
    {
        private static readonly string ExecutablePath = Process.GetCurrentProcess().MainModule.FileName; // Environment.ProcessPath should be available in the future for this
        private static readonly string TaskName = "GlucoseTray-" + Environment.UserName;

        public bool HasTaskEnabled()
        {
            using var ts = new TaskService();
            var existingTask = ts.GetTask(TaskName);
            return existingTask?.Enabled == true;
        }

        public void ToggleTask(bool enable)
        {
            using var ts = new TaskService();
            var task = ts.GetTask(TaskName);
            if (task is null)
            {
                var trigger = new LogonTrigger(){ UserId = Environment.UserName };
                var action = new ExecAction("\"" + ExecutablePath + "\"", workingDirectory: Directory.GetCurrentDirectory());
                var description = "GlucoseTray task for " + Environment.UserName;
                task = ts.AddTask(TaskName, trigger, action, description: description);
                task.Definition.Settings.ExecutionTimeLimit = TimeSpan.FromSeconds(0); // Don't end task after 72 hour default runtime.
                task.RegisterChanges();
            }
            task.Enabled = enable;
        }
    }
}
