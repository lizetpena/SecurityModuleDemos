using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace CSRFDemo.Controllers
{
    public class HomeController : Controller
    {
        private static string FileContent = string.Empty;
        private readonly IHostingEnvironment _appEnvironment;

        public HomeController(IHostingEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        [Authorize]
        public ActionResult Index()
        {
            ViewData["dataitems"] = FileContent;
            return View();
        }

        //[ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize]
        public ActionResult IndexAdd(string item)
        {
            if (String.IsNullOrEmpty(item))
                return View("Index");

            FileContent = FileContent + item;
            var content = FileContent;

            ViewData["dataitems"] = content.ToString();

            return View("Index");
        }

        [HttpPost]
        [Authorize]
        public ActionResult IndexClear()
        {
            FileContent = string.Empty;
            var content = FileContent;

            ViewData["dataitems"] = content.ToString();

            return View("Index");
        }
    }
}
