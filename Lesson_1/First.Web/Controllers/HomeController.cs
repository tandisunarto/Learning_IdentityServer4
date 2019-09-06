using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using First.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;

namespace First.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [Authorize]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/Home/Tokens"
            }, OpenIdConnectDefaults.AuthenticationScheme);
        }

        public IActionResult Logout()
        {
            return SignOut(new AuthenticationProperties
            {
                RedirectUri = "/Home/ThankYou"
            }, new string[] {
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme
            });
        }

        public IActionResult Tokens()
        {
            return View();
        }

        public IActionResult ThankYou()
        {
            return View();
        }

        public async Task<IActionResult> CallAPI()
        {
            IEnumerable<WeatherForecast> forecasts = null;
            HttpClient client = new HttpClient();
            try
            {
                var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                HttpResponseMessage response = await client.GetAsync("http://localhost:7002/api/WeatherForecast");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                forecasts = JsonConvert.DeserializeObject<IEnumerable<WeatherForecast>>(responseBody);
                // Above three lines can be replaced with new helper method below
                // responseBody = await client.GetStringAsync(uri);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return BadRequest(e.Message);
            }

            return View(forecasts);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
