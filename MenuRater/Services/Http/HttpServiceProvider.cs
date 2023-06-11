using MenuRater.Interfaces;

namespace MenuRater.Services.Http
{
    public class HttpServiceProvider : Interfaces.IServiceProvider
    {
        private readonly IHttpService _httpClient;
        public HttpServiceProvider(IHttpService httpClient)
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
