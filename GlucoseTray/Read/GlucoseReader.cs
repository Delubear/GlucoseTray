using GlucoseTray.Enums;
using Microsoft.Extensions.Logging;

namespace GlucoseTray.Read;

public interface IGlucoseReader
{
    Task<GlucoseReading> GetLatestGlucoseAsync();
}

public class GlucoseReader(IReadStrategyFactory strategyFactory, ILogger<GlucoseReader> logger) : IGlucoseReader
{
    private GlucoseReading? _latestReading;
    private object? _cacheIdentity;
    private bool _hasIdentity;

    public async Task<GlucoseReading> GetLatestGlucoseAsync()
    {
        var subjectChanged = false;
        try
        {
            var strategy = strategyFactory.Create();
            subjectChanged = !_hasIdentity || !Equals(_cacheIdentity, strategy.CacheIdentity);
            if (subjectChanged)
                _latestReading = null;
            _cacheIdentity = strategy.CacheIdentity;
            _hasIdentity = true;

            var reading = await strategy.GetLatestGlucoseAsync();
            if (!Equals(_cacheIdentity, strategyFactory.Create().CacheIdentity))
                throw new GlucoseReadException("Glucose settings changed during refresh. Waiting for a reading from the current account and patient.", false);
            reading.SubjectChanged = subjectChanged || _latestReading?.SubjectId != reading.SubjectId;
            _latestReading = reading;
            return _latestReading;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to read latest glucose value. Falling back to the last known reading.");
            if (ex is GlucoseReadException { CanUseCachedReading: false } ||
                (ex is GlucoseReadException { SubjectId: not null } selected && selected.SubjectId != _latestReading?.SubjectId) ||
                !CurrentIdentityMatches())
            {
                _latestReading = null;
                subjectChanged = true;
            }
            // Copy the cached value; do not refresh its time or attach transient errors to the cache.
            return new GlucoseReading
            {
                MgValue = _latestReading?.MgValue ?? 0,
                MmolValue = _latestReading?.MmolValue ?? 0,
                TimestampUtc = _latestReading?.TimestampUtc ?? DateTime.MinValue,
                Trend = _latestReading?.Trend ?? Trend.Unknown,
                SubjectId = _latestReading?.SubjectId,
                SubjectChanged = subjectChanged,
                ConnectionError = ex is GlucoseReadException known ? known.Message : "Unable to update glucose. Check your data-source settings and internet connection.",
            };
        }
    }

    private bool CurrentIdentityMatches()
    {
        try { return _hasIdentity && Equals(_cacheIdentity, strategyFactory.Create().CacheIdentity); }
        catch { return false; }
    }
}
