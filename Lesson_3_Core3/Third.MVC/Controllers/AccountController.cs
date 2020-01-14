using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Third.MVC
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties{
                RedirectUri = "/home/tokens"
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