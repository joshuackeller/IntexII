using System;
using System.Collections.Generic;
using System.Linq;
using IntexII.Models;
using Microsoft.AspNetCore.Mvc;

namespace IntexII.Components
{
    public class FilterViewComponent : ViewComponent
    {
        private IRDSRepo repo { get; set; }

        public FilterViewComponent(IRDSRepo temp)
        {
            repo = temp;
        }


        public IViewComponentResult Invoke()
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

            return View();
        }
    }
}
