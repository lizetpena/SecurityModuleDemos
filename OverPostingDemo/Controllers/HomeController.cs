using Microsoft.AspNetCore.Mvc;

namespace OverPostingDemo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Review review)
        {
            // Put in a breakpoint
            int likesCount = review.NumberOfLikes;

            // Review save logic

            return View();
        }
    }
}