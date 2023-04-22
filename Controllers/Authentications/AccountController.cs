using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DST.Controllers.Authentications
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AccessDenied(string id)
        {
            return View();
        }


        public IActionResult Login(string id)
        {
            return RedirectToAction("AccessDenied", "Account");
        }


        public IActionResult ExpiredSession()
        {
            return View();
        }
    }
}