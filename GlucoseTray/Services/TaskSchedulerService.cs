using Microsoft.Win32.TaskScheduler;
using System.IO;

namespace GlucoseTray.Services;

public class TaskSchedulerService
{
    private readonly string ExecutablePath = "\"" + Environment.ProcessPath + "\"";
    private readonly string TaskName = "GlucoseTray-" + Environment.UserName;
    private readonly string WorkingDirectory = Directory.GetCurrentDirectory();

    public bool HasTaskEnabled()
    {
        using var ts = new TaskService();
        var existingTask = ts.GetTask(TaskName);
        if (existingTask is null)
            return false;

        var action = (ExecAction)existingTask.Definition.Actions[0];
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
        using var ts = new TaskService();
        var task = ts.GetTask(TaskName);
        if (task is null)
        {
            var trigger = new LogonTrigger() { UserId = Environment.UserName };
            var action = new ExecAction(ExecutablePath, workingDirectory: WorkingDirectory);
            var description = "GlucoseTray task for " + Environment.UserName;
            task = ts.AddTask(TaskName, trigger, action, description: description);
            task.Definition.Settings.ExecutionTimeLimit = TimeSpan.FromSeconds(0); // Don't end task after 72 hour default runtime.
            task.RegisterChanges();
        }
        task.Enabled = enable;
    }
}
