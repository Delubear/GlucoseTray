namespace GlucoseTray.FetchResults.Contracts;

public interface IDexcomService
{
    Task GetLatestReadingAsync();
}
