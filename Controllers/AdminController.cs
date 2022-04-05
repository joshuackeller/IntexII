using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IntexII.Controllers
{
    public class AdminController : Controller
    {
        [Authorize(Roles="Admin")]
        public IActionResult Crashes()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AddCrash()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteCrash()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult EditCrash()
        {
            return View();
        }

    }
}
