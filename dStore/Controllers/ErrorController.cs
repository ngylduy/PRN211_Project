using Microsoft.AspNetCore.Mvc;

namespace dStore.Controllers
{

    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
