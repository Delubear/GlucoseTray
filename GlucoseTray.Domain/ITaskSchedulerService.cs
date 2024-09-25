namespace GlucoseTray.Domain
{
    public interface ITaskSchedulerService
    {
        bool HasTaskEnabled();
        void ToggleTask(bool enable);
    }
}