using IntexII.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntexII.Components
{
    public class CrashViewComponent : ViewComponent
    {
        private IRDSRepo repo { get; set; }
        public CrashViewComponent(IRDSRepo temp)
        {
            repo = temp;
        }

        public IViewComponentResult Invoke()
        {
            ViewBag.SelectedSeverity = RouteData?.Values["severity"];

            var Crash = repo.crashes
                .Select(x => x.crash_severity_id)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            return View(Crash);
        }
    }
}
