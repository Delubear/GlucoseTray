using System.ComponentModel;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace GlucoseTray;

public enum DiagnosticOperation { HttpRequest, ReadGlucose, RefreshLoop, ConfigurationRefresh }

/// <summary>Allowlisted diagnostics only. Never pass exceptions or arbitrary text to a logging provider.</summary>
public static class SafeDiagnostics
{
    public static void LogFailure(ILogger logger, LogLevel level, DiagnosticOperation operation, Exception error)
    {
        var kind = error switch
        {
            HttpRequestException => "HTTP",
            OperationCanceledException or TimeoutException => "Timeout or cancellation",
            JsonException => "Invalid JSON",
            Win32Exception => "Windows API",
            CryptographicException => "Credential protection",
            _ => "Operation failed",
        };
        int? status = error is HttpRequestException { StatusCode: not null } http &&
            (int)http.StatusCode.Value is >= 100 and <= 599 ? (int)http.StatusCode.Value : null;
        logger.Log(level, new EventId(1000 + (int)operation),
            "Glucose operation {Operation} failed ({FailureKind}, HTTP {StatusCode}).", operation, kind, status);
    }
}
