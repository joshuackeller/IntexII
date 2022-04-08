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

            var cities = repo.crashes
                               .Select(x => new City() { city = x.city });

            ViewBag.city = cities.Select(x => x.city)
                                .Distinct()
                                .OrderBy(x => x)
                                .ToList();


            var severity = repo.crashes
                                .Select(x => new Severity() { severity = x.crash_severity_id });

            ViewBag.severity = severity.Select(x => x.severity)
                                .Distinct()
                                .OrderBy(x => x)
                                .ToList();

            var length = 30;


            var x = new CrashesViewModel
            {
                crashes = repo.crashes
                    .Take(length),

                PageInfo = new PageInfo
                {
                    TotalNumCrashes = repo.crashes.Count(),
                    CrashesPerPage = length,
                    CurrentPage = 1,

                }

            };

            return View(x);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Crashes(CrashesViewModel cvm)
        {

            var cities = repo.crashes
                               .Select(x => new City() { city = x.city });

            ViewBag.city = cities.Select(x => x.city)
                                .Distinct()
                                .OrderBy(x => x)
                                .ToList();


            var severity = repo.crashes
                                .Select(x => new Severity() { severity = x.crash_severity_id });

            ViewBag.severity = severity.Select(x => x.severity)
                                .Distinct()
                                .OrderBy(x => x)
                                .ToList();

            var length = 30;


            var x = new CrashesViewModel
            {
                crashes = repo.crashes
                    .Where(x => x.crash_severity_id == cvm.Filter.severity || cvm.Filter.severity == 0)
                    .Where(x => x.city == cvm.Filter.city || cvm.Filter.city == null)
                    .Skip(length * (cvm.PageInfo.CurrentPage - 1))
                    .Take(length),

                PageInfo = new PageInfo
                {
                    TotalNumCrashes = ((cvm.Filter.severity == 0 && cvm.Filter.city == null) ?
                    repo.crashes.Count()
                    : (cvm.Filter.severity == 0) ?
                    repo.crashes.Where(x => x.city == cvm.Filter.city).Count()
                    : (cvm.Filter.city == null) ?
                    repo.crashes.Where(x => x.crash_severity_id == cvm.Filter.severity).Count()
                    : repo.crashes.Where(x => x.city == cvm.Filter.city).Where(x => x.crash_severity_id == cvm.Filter.severity).Count()
                    ),

                    CrashesPerPage = length,
                    CurrentPage = cvm.PageInfo.CurrentPage,
                    Page2 = cvm.PageInfo.CurrentPage - 1,
                    Page4 = cvm.PageInfo.CurrentPage + 1
                },

                Filter = cvm.Filter

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

       
        [Authorize(Roles = "Admin")]
         [HttpPost]
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

        [Authorize(Roles = "Admin")]
        [HttpGet]
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




    }
}
