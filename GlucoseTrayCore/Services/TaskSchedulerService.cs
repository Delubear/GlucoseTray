using Microsoft.Win32.TaskScheduler;
using System;
using System.Diagnostics;

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
            var task = ts.GetTask(TaskName) ?? ts.AddTask(TaskName, new LogonTrigger(){ UserId = Environment.UserName }, new ExecAction("\"" + ExecutablePath + "\""));
            task.Enabled = enable;
        }
    }
}
