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
        public IRDSRepo repo;
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


        //edit button
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var Crash = repo.crashes.Single(x => x.crash_id == id);
            return View("Records", Crash);
        }
        [HttpPost]
        public IActionResult Edit(Crash info)
        {
            if (ModelState.IsValid)
            {
                repo.SaveCrash(info);


                return RedirectToAction("Records");
            }
            else
            {
                return View(info);
            }
        }

    }
}
