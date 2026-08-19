using System.Net;
using GlucoseTray.Read;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace GlucoseTray.Tests;

public class ExternalCommunicationAdapterTests
{
    private static ExternalCommunicationAdapter CreateAdapter(FakeHttpMessageHandler handler)
    {
        var factory = Substitute.For<IHttpClientFactory>();
        factory.CreateClient(Arg.Any<string>()).Returns(_ => new HttpClient(handler));
        return new ExternalCommunicationAdapter(factory, NullLogger<ExternalCommunicationAdapter>.Instance);
    }

    [Test]
    public async Task ShouldReturnResponseBodyOnSuccessfulGet()
    {
        var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, "glucose-data");
        var adapter = CreateAdapter(handler);

        var result = await adapter.GetApiResponseAsync("https://example.com/api");

        Assert.That(result, Is.EqualTo("glucose-data"));
    }

    [Test]
    public async Task ShouldReturnResponseBodyOnSuccessfulPost()
    {
        var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, "session-id");
        var adapter = CreateAdapter(handler);

        var result = await adapter.PostApiResponseAsync("https://example.com/api", "{}");

        Assert.That(result, Is.EqualTo("session-id"));
    }

    [Test]
    public void ShouldThrowOnNonSuccessStatusCode()
    {
        var handler = new FakeHttpMessageHandler(HttpStatusCode.Unauthorized, "denied");
        var adapter = CreateAdapter(handler);

        Assert.ThrowsAsync<HttpRequestException>(() => adapter.GetApiResponseAsync("https://example.com/api"));
    }

    [Test]
    public async Task ShouldSendProvidedContentInPostBody()
    {
        var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, "ok");
        var adapter = CreateAdapter(handler);

        await adapter.PostApiResponseAsync("https://example.com/api", "my-payload");

        Assert.That(handler.LastRequestBody, Is.EqualTo("my-payload"));
    }

    private sealed class FakeHttpMessageHandler(HttpStatusCode statusCode, string content) : HttpMessageHandler
    {
        public string? LastRequestBody { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Content is not null)
                LastRequestBody = await request.Content.ReadAsStringAsync(cancellationToken);

            return new HttpResponseMessage(statusCode) { Content = new StringContent(content) };
        }
    }
}
