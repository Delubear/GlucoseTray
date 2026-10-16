namespace GlucoseTray.Read.LibreLinkUp;

internal sealed class LibreLinkUpReadStrategy(AppSettings settings, ILibreLinkUpClient client) : IReadStrategy
{
    public object CacheIdentity { get; } = new Identity(settings.LibreLinkUpUsername.Trim().ToUpperInvariant(),
        settings.LibreLinkUpRegion.Trim().ToUpperInvariant(), settings.LibreLinkUpPatientId.Trim());

    public Task<GlucoseReading> GetLatestGlucoseAsync() => client.GetLatestGlucoseAsync(settings);

    private sealed record Identity(string Account, string Region, string Patient);
}
