namespace GlucoseTray.FetchResults.Contracts;

public interface IGlucoseFetchService
{
    Task GetLatestReadingsAsync();
}
