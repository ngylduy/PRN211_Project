using Microsoft.AspNetCore.Mvc;

namespace dStore.Controllers
{
    public class PageController : Controller
    {
        public IActionResult About()
        {
            ViewBag.About = true;
            return View();
        }
        public IActionResult Contact()
        {
            ViewBag.Contact = true;
            return View();
        }
    }
}
