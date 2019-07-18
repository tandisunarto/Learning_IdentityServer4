using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using Second.Web.App.Models;
using Second.Web.MVC.Models;

namespace Second.Web.App.Controllers
{
    public class MyAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //throw new NotImplementedException();
            var user = context.HttpContext.User;
        }
    }

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [MyAuthorize(Roles = "Superman")]
        public async Task<IActionResult> About()
        {

            ViewData["Message"] = "Your application description page.";

            var discoveryClient = new DiscoveryClient("http://localhost:8000");
            var metadataResponse = await discoveryClient.GetAsync();

            var userinfoClient = new UserInfoClient(metadataResponse.UserInfoEndpoint);
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var response = await userinfoClient.GetAsync(accessToken);

            if (response.IsError)
            {
                throw new Exception("Error access UserInfo Endpoint", response.Exception);
            }

            List<string> roles = response.Claims.Where(c => c.Type == JwtClaimTypes.Role).Select(c => c.Value).ToList();

            return View(new AboutViewModel
            {
                Roles = roles
            });
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public async Task<IActionResult> CallAPI()
        {
            var expiresAt = await HttpContext.GetTokenAsync("expires_at");
            string accessToken = string.Empty;

            if (string.IsNullOrEmpty(expiresAt) ||
                ((DateTime.Parse(expiresAt).AddSeconds(-60)).ToUniversalTime() < DateTime.UtcNow))
                accessToken = await RenewTokens();
            else
                accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            HttpClient httpClient = new HttpClient();
            httpClient.SetBearerToken(accessToken);

            var response = await httpClient.GetAsync("http://localhost:8020/api/values");

            string values = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                values = await response.Content.ReadAsStringAsync();
            }
            else
            {
                values = $"[\"{response.ReasonPhrase}\"]";
            }

            return View(JsonConvert.DeserializeObject<IEnumerable<string>>(values));
        }

        private async Task<string> RenewTokens()
        {
            var discoClient = new DiscoveryClient("http://localhost:8000");
            var metadataResponse = await discoClient.GetAsync();

            // create token client to get new tokens
            var tokenClient = new TokenClient(metadataResponse.TokenEndpoint,
                "mvc", "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0");

            var currentRefreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            var tokenResult = await tokenClient.RequestRefreshTokenAsync(currentRefreshToken);

            if (!tokenResult.IsError)
            {
                var authenticateInfo = await HttpContext.AuthenticateAsync();

                var expireAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResult.ExpiresIn);
                //authenticateInfo.Properties.UpdateTokenValue(OpenIdConnectParameterNames.AccessToken, tokenResult.AccessToken);
                //authenticateInfo.Properties.UpdateTokenValue(OpenIdConnectParameterNames.RefreshToken, tokenResult.RefreshToken);
                //authenticateInfo.Properties.UpdateTokenValue(OpenIdConnectParameterNames.IdToken, tokenResult.IdentityToken);
                //authenticateInfo.Properties.UpdateTokenValue("expires_at", expireAt.ToString("o", CultureInfo.InvariantCulture));

                authenticateInfo.Properties.StoreTokens(new List<AuthenticationToken>
                {
                    new AuthenticationToken
                    {
                        Name = OpenIdConnectParameterNames.IdToken,
                        Value = tokenResult.IdentityToken
                    },
                    new AuthenticationToken
                    {
                        Name = OpenIdConnectParameterNames.AccessToken,
                        Value = tokenResult.AccessToken
                    },
                    new AuthenticationToken
                    {
                        Name = OpenIdConnectParameterNames.RefreshToken,
                        Value = tokenResult.RefreshToken
                    },
                    new AuthenticationToken
                    {
                        Name = OpenIdConnectParameterNames.TokenType,
                        Value = "Bearer"
                    },
                    new AuthenticationToken
                    {
                        Name = "expires_at",
                        Value = expireAt.ToString("o", CultureInfo.InvariantCulture)
                    },
                });

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    authenticateInfo.Principal, authenticateInfo.Properties);

                return tokenResult.AccessToken;
            }
            else
            {
                throw new Exception("Problem encountered while refreshing tokens", tokenResult.Exception);
            }
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Login()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/Home/Index"
            }, OpenIdConnectDefaults.AuthenticationScheme);
        }


        public IActionResult Logout()
        {
            return SignOut(new AuthenticationProperties
            {
                // RedirectUri = "/Home/Contact"
            }, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
