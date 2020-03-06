using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
// using Newtonsoft.Json;

namespace NetCoreIdentity.API
{
    [Route("api/[controller]")]
    [Authorize] 
    public class ValuesController : Controller
    {
        [Route("")]
        [Route("[action]")]
        public IActionResult Index()
        {
            return Ok("Hello from value web api");
        }
    }
}