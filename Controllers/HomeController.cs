using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IntexII.Models;
using IntexII.Models.ViewModels;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;


namespace IntexII.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public IRDSRepo repo;


        private InferenceSession _session;



        public HomeController(ILogger<HomeController> logger, IRDSRepo temp, InferenceSession session)
        {
            _session = session;
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
        
        [HttpPost]
        public IActionResult Records(CrashesViewModel cvm)
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


        [HttpGet]
        public IActionResult ModelInput()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Prediction(ModelData data)
        {
            var result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", data.AsTensor())
            });
            Tensor<string> score = result.First().AsTensor<string>();

            //ViewBag["prediction"] = score.First()


            var prediction = new Prediction { PredictedValue = score.First() };
            result.Dispose();
            return View(prediction);
        }




    }
}
