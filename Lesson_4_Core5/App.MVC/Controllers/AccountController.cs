using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace App.MVC.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties {
                RedirectUri = ""
            },
            "oidc");
        }

        public IActionResult Logout()
        {
            return SignOut(new AuthenticationProperties {
                RedirectUri = "/home/privacy"
            },
            "oidc", "Cookies");
        }
    }
}