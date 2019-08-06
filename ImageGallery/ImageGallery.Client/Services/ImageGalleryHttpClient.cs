using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ImageGallery.Client.Services
{
    public class ImageGalleryHttpClient : IImageGalleryHttpClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private HttpClient _httpClient = new HttpClient();

        public ImageGalleryHttpClient(
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<HttpClient> GetClient()
        {
            _httpClient.BaseAddress = new Uri(_configuration["GalleryAPIUrl"]);

            var accessToken = await GetAccessToken();
            if (!string.IsNullOrEmpty(accessToken))
                _httpClient.SetBearerToken(accessToken);

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            return _httpClient;
        }

        private async Task<string> GetAccessToken()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return await httpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
        }
    }
}