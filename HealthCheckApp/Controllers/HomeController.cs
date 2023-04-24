using Microsoft.AspNetCore.Mvc;

namespace HealthCheckApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        public IActionResult Index()
        {
            return Redirect("/healthchecks-ui");
        }
    }
}