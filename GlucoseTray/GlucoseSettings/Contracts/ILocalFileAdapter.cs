namespace GlucoseTray.GlucoseSettings.Contracts;

public interface ILocalFileAdapter<T> where T : class
{
    void WriteModelToJsonFile(T model, string file);
    T? ReadModelFromFile(string file);
    bool DoesFileExist(string filePath);
}
