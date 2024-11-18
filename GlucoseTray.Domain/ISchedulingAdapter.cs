namespace GlucoseTray.Domain
{
    public interface ISchedulingAdapter
    {
        bool HasTaskEnabled();
        void ToggleTask(bool enable);
    }
}