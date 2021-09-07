using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace EtherScanWebTest
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("chart")]
        public ActionResult CharterHelp()
        {
            new Chart(width: 500, height: 300, theme: ChartTheme.Blue)
            .AddTitle("Chart for languages")
                 .AddSeries(
                      chartType: "column",
                   xValue: new[] { "ASP.NET", "HTML5", "C Language", "C++" },
                     yValues: new[] { "90", "100", "80", "70" })
                   .Write("bmp");
            return null;
        }
    }
}
