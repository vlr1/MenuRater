using System.ComponentModel.Design;

namespace MenuRater.Interfaces
{
    public interface IHttpService
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage message);
    }
}
