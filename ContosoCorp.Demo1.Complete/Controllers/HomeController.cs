using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContosoCorp.Demo1.Complete.Models;
using Microsoft.AspNetCore.Authorization;

namespace ContosoCorp.Demo1.Complete.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult About()
        {
            ViewData["Message"] = "Your Employee application description page.";

            return View();
        }

        [Authorize(Policy = "FoundersOnly")]
        public IActionResult AboutFounders()
        {
            ViewData["Message"] = "Your Founder application description page.";

            return View("About");
        }

        [Authorize(Policy = "Over21")]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
