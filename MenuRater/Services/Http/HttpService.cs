using MenuRater.Interfaces;

namespace MenuRater.Services.Http
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;

        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage message)
        {
            return await _httpClient.SendAsync(message);
        }
    }
}
