using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DST.Helpers;

namespace DST.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        GeneralClass generalClass = new GeneralClass();


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        public IActionResult Error(string message)
        {
            var msg = generalClass.Decrypt(message);

            ViewData["Message"] = msg;
            return View();
        }

        public IActionResult Errorr(string message)
        {
            var msg = generalClass.Decrypt(message);

            ViewData["Message"] = msg;
            return View();
        }
    }
}
