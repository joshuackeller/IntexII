using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntexII.Models;
using IntexII.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IntexII.Controllers
{

    public class AdminController : Controller
    {
        public IRDSRepo repo { get; set; }


        public AdminController (IRDSRepo temp)
        {
            repo = temp;
        }


        [Authorize(Roles="Admin")]
        public IActionResult Crashes(int pageNum = 1)
        {


            var length = 300;


            var x = new CrashesViewModel
            {
                crashes = repo.crashes
                    .Skip(length * (pageNum - 1))
                    .Take(length),

                PageInfo = new PageInfo
                {
                    TotalNumCrashes = repo.crashes.Count(),
                    CrashesPerPage = length,
                    CurrentPage = pageNum
                }

            };

            return View(x);
        }





        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult DeleteCrash(int id)
        {
            var Crash = repo.crashes.FirstOrDefault(x => x.crash_id == id);

            return View(Crash);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteCrash(Crash c)
        {
            repo.DeleteCrash(c);
            return RedirectToAction("Crashes");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult EditCrash()
        {
            return View();
        }





        [Authorize(Roles = "Admin")]
        
        public IActionResult AddCrash()
        {
            
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddCrash(Crash c)
        {
            if (ModelState.IsValid)
            {
                repo.CreateCrash(c);
                return RedirectToAction("Crashes");
            }
            else
            {
                return View(c);
            }

        }




        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult EditCrash(int id)
        {
            var crash = repo.crashes.FirstOrDefault(x => x.crash_id == id);
            return View(crash);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult EditCrash(Crash c)
        {
            if (ModelState.IsValid)
            {
                repo.SaveCrash(c);
                return RedirectToAction("Crashes");
            }
            else
            {
                return View(c);
            }
           
        }




    }
}
