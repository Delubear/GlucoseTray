namespace GlucoseTray.FetchResults.Contracts;

public interface INightscoutService
{
    Task GetLatestReadingAsync();
}
