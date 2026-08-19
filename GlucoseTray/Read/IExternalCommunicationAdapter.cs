
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Logging;

namespace GlucoseTray.Read;

public interface IExternalCommunicationAdapter
{
    Task<string> PostApiResponseAsync(string url, string? content = null);
    Task<string> GetApiResponseAsync(string url);
}

public class ExternalCommunicationAdapter(IHttpClientFactory httpClientFactory, ILogger<ExternalCommunicationAdapter> logger) : IExternalCommunicationAdapter
{
    public const string HttpClientName = "GlucoseTray";
    public async Task<string> PostApiResponseAsync(string url, string? content = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        if (content is not null)
        {
            var requestContent = new StringContent(content, Encoding.UTF8, "application/json");
            request.Content = requestContent;
        }
        var result = await DoApiResponseAsync(request);
        return result;
    }

    public async Task<string> GetApiResponseAsync(string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var result = await DoApiResponseAsync(request);
        return result;
    }

    private async Task<string> DoApiResponseAsync(HttpRequestMessage request)
    {
        HttpResponseMessage? response = null;
        try
        {
            var client = httpClientFactory.CreateClient(HttpClientName);

            response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "HTTP {Method} request to {Url} failed.", request.Method, request.RequestUri);
            throw;
        }
        finally
        {
            request?.Dispose();
            response?.Dispose();
        }
    }
}