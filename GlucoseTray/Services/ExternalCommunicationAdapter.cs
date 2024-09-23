using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace GlucoseTray.Services
{
    public interface IExternalCommunicationAdapter
    {
        Task<string> PostApiResponseAsync(string url, string? content = null);
        Task<string> GetApiResponseAsync(string url, string? content = null);
    }

    public class ExternalCommunicationAdapter : IExternalCommunicationAdapter
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly List<string> DebugText = [];
        private readonly ILogger _logger;
        private readonly IOptionsMonitor<GlucoseTraySettings> _options;

        public ExternalCommunicationAdapter(IHttpClientFactory httpClientFactory, ILogger<ExternalCommunicationAdapter> logger, IOptionsMonitor<GlucoseTraySettings> options)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _options = options;
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
                var client = _httpClientFactory.CreateClient();

                DebugText.Add("Requesting: " + request.RequestUri);
                response = await client.SendAsync(request);
                DebugText.Add("Response received with status code: " + response.StatusCode);
                var result = await response.Content.ReadAsStringAsync();
                DebugText.Add("Result: " + result);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Invalid external response.");
                DebugText.Add("Error with external communication: " + ex.Message);
                if (_options.CurrentValue.IsDebugMode)
                    DebugService.ShowDebugAlert(ex, "External result fetch", string.Join(Environment.NewLine, DebugText));
                throw;
            }
            finally
            {
                request?.Dispose();
                response?.Dispose();
            }
        }
    }
}
