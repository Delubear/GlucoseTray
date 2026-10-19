using GlucoseTray.Display;
using GlucoseTray.Read;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GlucoseTray;

public class AppRunner(ITray tray, IGlucoseReader reader, IOptionsMonitor<AppSettings> options, ICredentialMigrator credentialMigrator, ILogger<AppRunner> logger) : IDisposable
{
    private readonly SemaphoreSlim _processLock = new(1, 1);
    private IDisposable? _onChangeSubscription;

    public async Task Start()
    {
        _onChangeSubscription = options.OnChange(settings => { _ = RefreshOnConfigChangeAsync(); });

        while (true)
        {
            try
            {
                await Process();
                await Task.Delay(TimeSpan.FromMinutes(Math.Max(options.CurrentValue.RefreshIntervalInMinutes, 1)));
            }
            catch (Exception ex)
            {
                SafeDiagnostics.LogFailure(logger, LogLevel.Critical, DiagnosticOperation.RefreshLoop, ex);
                tray.Dispose();
                throw;
            }
        }
    }

    private async Task RefreshOnConfigChangeAsync()
    {
        try
        {
            credentialMigrator.ProtectFile(AppSettings.FileName);
            await Process();
        }
        catch (Exception ex)
        {
            SafeDiagnostics.LogFailure(logger, LogLevel.Error, DiagnosticOperation.ConfigurationRefresh, ex);
        }
    }

    public async Task Process()
    {
        await _processLock.WaitAsync();
        try
        {
            var result = await reader.GetLatestGlucoseAsync();
            tray.Refresh(result);
        }
        finally
        {
            _processLock.Release();
        }
    }

    public void Dispose()
    {
        _onChangeSubscription?.Dispose();
        _processLock.Dispose();
        GC.SuppressFinalize(this);
    }
}
