using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimplexMethod.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SimplexMethod.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Simplex()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Simplex(int sel1, int sel2)
        {
            TempData["Sel1"] = sel1;
            TempData["Sel2"] = sel2;
         
            return  RedirectToAction("SimplexData");
        }
        [HttpGet]
        public IActionResult SimplexData()
        {
            if (TempData["Sel1"] != null && TempData["Sel2"] != null)
            {
                ViewBag.Sel1 = TempData["Sel1"];
                ViewBag.Sel2 = TempData["Sel2"];   
                TempData.Keep("Sel1");
                TempData.Keep("Sel2");
            }
            else
            {
                ViewBag.Sel1 = 2;
                ViewBag.Sel2 = 2;
            }
            return View();
        }
        [HttpPost]
        public IActionResult SimplexData(SimplexDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Solve();
                return PartialView("SimplexDataPartial", model);
            }
            else 
            return PartialView("ValidateErrorPartial",model);
            
        }
        public IActionResult Privacy()
        {
            return View(); 
        }
        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
