namespace GlucoseTray.DisplayResults.Contracts
{
    public interface ISchedulingAdapter
    {
        bool HasTaskEnabled();
        void ToggleTask(bool enable);
    }
}