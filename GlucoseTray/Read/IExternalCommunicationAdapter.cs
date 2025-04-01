
using System.Net.Http.Headers;
using System.Text;

namespace GlucoseTray.Read;

public interface IExternalCommunicationAdapter
{
    Task<string> PostApiResponseAsync(string url, string? content = null);
    Task<string> GetApiResponseAsync(string url, string? content = null);
}

public class ExternalCommunicationAdapter : IExternalCommunicationAdapter
{
    private readonly IHttpClientFactory _clientFactory;
    public ExternalCommunicationAdapter(IHttpClientFactory httpClientFactory)
    {
        _clientFactory = httpClientFactory;
    }

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

    public async Task<string> GetApiResponseAsync(string url, string? content = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        if (content is not null)
        {
            var requestContent = new StringContent(content, Encoding.UTF8, "application/json");
            request.Content = requestContent;
        }
        var result = await DoApiResponseAsync(request);
        return result;
    }

    private async Task<string> DoApiResponseAsync(HttpRequestMessage request)
    {
        HttpResponseMessage? response = null;
        try
        {
            var client = _clientFactory.CreateClient();

            response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();

            return result;
        }
        finally
        {
            request?.Dispose();
            response?.Dispose();
        }
    }
}