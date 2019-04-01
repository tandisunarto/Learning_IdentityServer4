using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace jwt.API
{
    public class ValueController : Controller
    {
        public IActionResult Index()
        {
            return Ok("Hello from value web api");
        }
    }
}