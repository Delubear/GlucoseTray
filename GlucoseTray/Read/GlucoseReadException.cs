namespace GlucoseTray.Read;

/// <summary>A safe, actionable status message for the tray, without response bodies or credentials.</summary>
public sealed class GlucoseReadException(string message, bool canUseCachedReading = true, string? subjectId = null) : Exception(message)
{
    public bool CanUseCachedReading { get; } = canUseCachedReading;
    public string? SubjectId { get; } = subjectId;
}
