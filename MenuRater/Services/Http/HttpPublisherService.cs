using MenuRater.Interfaces;

namespace MenuRater.Services.Http
{
    public class HttpPublisherService : IPublisherService
    {
        private readonly IHttpService _httpClient;
        public HttpPublisherService(IHttpService httpClient)
        {

            _httpClient = httpClient;
        }

        public async Task<string> CallAsync<T>(T message)
        {
            var response = await _httpClient.SendAsync(message as HttpRequestMessage);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
