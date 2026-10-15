using Microsoft.Extensions.Logging;

namespace GlucoseTray.Read;

/// <summary>No URLs, headers, bodies, reason phrases, or exception text cross the logging boundary.</summary>
public sealed class SafeHttpDiagnosticsHandler(ILogger<SafeHttpDiagnosticsHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.RequestUri is not { IsAbsoluteUri: true, Scheme: "https", UserInfo.Length: 0 })
                throw new HttpRequestException("Glucose services require HTTPS URLs without embedded credentials.");
            var response = await base.SendAsync(request, cancellationToken);
            var method = request.Method == HttpMethod.Get ? "GET" : request.Method == HttpMethod.Post ? "POST" : "OTHER";
            logger.LogInformation("Glucose HTTP {Method} completed (HTTP {StatusCode}).", method, (int)response.StatusCode);
            return response;
        }
        catch (Exception ex)
        {
            SafeDiagnostics.LogFailure(logger, LogLevel.Warning, DiagnosticOperation.HttpRequest, ex);
            throw;
        }
    }
}
