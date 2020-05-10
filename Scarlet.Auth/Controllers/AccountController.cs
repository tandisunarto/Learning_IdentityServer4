using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Scarlet.Auth.Models.Account;

namespace Scarlet.Auth.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm, string returnUrl = "")
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, "Super Scarlet")
            };

            var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var claimPrincipal = new ClaimsPrincipal(claimIdentity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimPrincipal,
                new AuthenticationProperties { IsPersistent = vm.RememberMe }
            );

            return Redirect(returnUrl);
        }
    }
}