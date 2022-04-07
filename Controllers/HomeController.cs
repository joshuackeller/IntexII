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


        public IActionResult Records( double severity = 0, int pageNum = 1)
        {

            var length = 300;


            var x = new CrashesViewModel
            {
                crashes = repo.crashes
                    .Where(x => x.crash_severity_id == severity || severity == 0)
                    .Skip(length * (pageNum - 1))
                    .Take(length),

                PageInfo = new PageInfo
                {
                    TotalNumCrashes = (severity == 0 ? repo.crashes.Count() : repo.crashes.Where(x => x.crash_severity_id == severity).Count()),
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
            Tensor<float> score = result.First().AsTensor<float>();
            var prediction = new Prediction { PredictedValue = score.First() };
            result.Dispose();
            return View("Score", prediction);
        }

    }
}
