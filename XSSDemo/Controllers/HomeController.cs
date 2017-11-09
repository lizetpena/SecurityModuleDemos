using Microsoft.AspNetCore.Mvc;

namespace XSSDemo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string username)
        {
            ViewBag.Username = username;
            return View();
        }
    }
}