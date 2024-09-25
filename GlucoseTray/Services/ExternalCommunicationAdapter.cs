using GlucoseTray.Settings;
using Microsoft.Extensions.Logging;
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
        private readonly DebugService _debug;
        private readonly ILogger _logger;
        private readonly ISettingsProxy _options;

        public ExternalCommunicationAdapter(IHttpClientFactory httpClientFactory, ILogger<ExternalCommunicationAdapter> logger, ISettingsProxy options, DebugService debug)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _options = options;
            _debug = debug;
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

                _debug.AddDebugText("Requesting: " + request.RequestUri);
                response = await client.SendAsync(request);
                _debug.AddDebugText("Response received with status code: " + response.StatusCode);
                var result = await response.Content.ReadAsStringAsync();
                _debug.AddDebugText("Result: " + result);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Invalid external response.");
                _debug.AddDebugText("Error with external communication: " + ex.Message);
                if (_options.IsDebugMode)
                    _debug.ShowDebugAlert(ex, "External result fetch");
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
