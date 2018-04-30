using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using Second.Web.App.Models;
using Second.Web.MVC.Models;

namespace Second.Web.App.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
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

            return View(new AboutViewModel {
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
            HttpClient httpClient = new HttpClient();
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
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
                RedirectUri = "/Home/Index"
            }, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
