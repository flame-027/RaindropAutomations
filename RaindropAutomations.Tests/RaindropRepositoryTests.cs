using Microsoft.Extensions.Configuration;
using Moq;
using RaindropAutomations.Models.Saving;
using System.Net;
using System.Text.Json;

namespace RaindropAutomations.Tests
{
    public class RaindropRepositoryTests
    {
        [Fact]
        public async Task RaindropRepository_CreateMultipleBookmarks()
        {
            // Arrange: Mock Configuration
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                { "RaindropConfig:ApiToken", "test-token" },
                { "RaindropConfig:ApiBaseUrl", "https://fake-api.com" }
                })
                .Build();

            string capturedPayload = null;

            // Use custom handler to capture payload
            var handler = new TestHttpMessageHandler((request, cancellationToken) =>
            {
                capturedPayload = request.Content.ReadAsStringAsync().Result;
                return Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });
            });

            var httpClient = new HttpClient(handler);

            // Act: Call the repository
            var repo = new RaindropRepository(config, httpClient);

            repo.CreateMultipleBookmarks(new List<BookmarkSaveModel>
            {
                new BookmarkSaveModel 
                { 
                    Link = @"https://google.com",
                    Collection = new CollectionIdSaveModel { Id = 123 },
                    PleaseParse = new() 
                }
            });

            // Assert: Verify captured payload
            Assert.NotNull(capturedPayload);
            Assert.Contains("Bookmark 1", capturedPayload);
        }
    }

    // Custom HttpMessageHandler to Override SendAsync
    public class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handlerFunc;

        public TestHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc)
        {
            _handlerFunc = handlerFunc;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _handlerFunc(request, cancellationToken);
        }
    }
}