using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Globalization;
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

            var accessToken = await GetAccessTokenRenew();
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

        private async Task<string> GetAccessTokenRenew()
        {
            string accessToken = string.Empty;

            // get the current HttpContext to access the tokens
            var currentContext = _httpContextAccessor.HttpContext;

            // get access token
            //accessToken = await currentContext.GetTokenAsync(
            //    OpenIdConnectParameterNames.AccessToken);

            // should we renew access & refresh tokens?
            // get expires_at value
            var expires_at = await currentContext.GetTokenAsync("expires_at");

            // compare - make sure to use the exact date formats for comparison 
            // (UTC, in this case)
            if (string.IsNullOrWhiteSpace(expires_at)
                || ((DateTime.Parse(expires_at).AddSeconds(-60)).ToUniversalTime() < DateTime.UtcNow))
            {
                accessToken = await RenewTokens();
            }
            else
            {
                // get access token
                accessToken = await currentContext
                    .GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            }

            return accessToken;
        }

        private async Task<string> RenewTokens()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var discoClient = new DiscoveryClient(_configuration["Authority"]);
            var metadataResponse = await discoClient.GetAsync();

            var tokenClient = new TokenClient(metadataResponse.TokenEndpoint,
                "image_gallery", "secret");

            var refreshToken = await httpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            var tokenResult = await tokenClient.RequestRefreshTokenAsync(refreshToken);

            if (!tokenResult.IsError)
            {
                var updatedTokens = new List<AuthenticationToken>();
                updatedTokens.Add(new AuthenticationToken {
                    Name = OpenIdConnectParameterNames.IdToken,
                    Value = tokenResult.IdentityToken
                });
                updatedTokens.Add(new AuthenticationToken {
                    Name = OpenIdConnectParameterNames.AccessToken,
                    Value = tokenResult.AccessToken
                });
                updatedTokens.Add(new AuthenticationToken {
                    Name = OpenIdConnectParameterNames.RefreshToken,
                    Value = tokenResult.RefreshToken
                });

                var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResult.ExpiresIn);
                updatedTokens.Add(new AuthenticationToken {
                    Name = "expires_at",
                    Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)
                });

                var currentAuthenticateResult = await httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);  // "Cookies";

                currentAuthenticateResult.Properties.StoreTokens(updatedTokens);

                await httpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    currentAuthenticateResult.Principal,
                    currentAuthenticateResult.Properties
                );

                return tokenResult.AccessToken;
            }
            else 
            {
                throw new Exception("Problem encountered while refreshing tokens", tokenResult.Exception);
            }
        }
    }
}