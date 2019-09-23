using Microsoft.AspNetCore.Mvc;

namespace Second.Web.MVC
{
    public class AuthorizationController : Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}