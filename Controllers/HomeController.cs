using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IntexII.Models;
using IntexII.Models.ViewModels;

namespace IntexII.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public IRDSRepo repo;

        public HomeController(ILogger<HomeController> logger, IRDSRepo temp)
        {
            _logger = logger;
            repo = temp;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        public IActionResult Records(int pageNum = 1)
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

        public IActionResult SingleRecord(int id)
        {
            var crash = repo.crashes.FirstOrDefault(x => x.crash_id == id);


            return View(crash);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
