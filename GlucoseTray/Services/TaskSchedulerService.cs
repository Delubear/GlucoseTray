using Microsoft.Win32.TaskScheduler;
using System;
using System.IO;

namespace GlucoseTray.Services
{
    public class TaskSchedulerService
    {
        readonly string ExecutablePath = "\"" + Environment.ProcessPath + "\"";
        readonly string TaskName = "GlucoseTray-" + Environment.UserName;
        readonly string WorkingDirectory = Directory.GetCurrentDirectory();

        public bool HasTaskEnabled()
        {
            using TaskService ts = new TaskService();
            Task existingTask = ts.GetTask(TaskName);

            if (existingTask is null)
                return false;

            ExecAction action = (ExecAction)existingTask.Definition.Actions[0];

            if (action.Path != ExecutablePath || action.WorkingDirectory != Directory.GetCurrentDirectory()) // File has been moved since task was created. Update values.
            {
                action.Path = ExecutablePath;
                action.WorkingDirectory = WorkingDirectory;
                existingTask.RegisterChanges();
            }

            return existingTask.Enabled;
        }

        public void ToggleTask(bool enable)
        {
            using TaskService ts = new TaskService();
            Task task = ts.GetTask(TaskName);

            if (task is null)
            {
                LogonTrigger trigger = new LogonTrigger() { UserId = Environment.UserName };
                ExecAction action = new ExecAction(ExecutablePath, workingDirectory: WorkingDirectory);
                string description = $"GlucoseTray task for {Environment.UserName}";
                task = ts.AddTask(TaskName, trigger, action, description: description);
                task.Definition.Settings.ExecutionTimeLimit = TimeSpan.FromSeconds(0); // Don't end task after 72 hour default runtime.
                task.RegisterChanges();
            }
            task.Enabled = enable;
        }
    }
}
