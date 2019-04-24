using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using TewiBoard.Web.Models;

namespace TewiBoard.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.LogError("\r\n-----------------------\r\n  An Error Occered!  \r\n-----------------------\r\n", Request.Body);
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult Report([FromForm] long pid)
        {
            _logger.LogInformation("\r\n[Info] Someone report a card\r\n");
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/report/reportlist.txt");
            try
            {
                using (var sr = System.IO.File.OpenText(path))
                {
                    string rpid;
                    while ((rpid = sr.ReadLine()) != null)
                    {
                        if (pid.ToString() == rpid)
                        {
                            return RedirectToAction("Index", "Timeline");
                        }
                    }
                }
                System.IO.File.AppendAllText(path, "\r\n" + pid.ToString());
            }
            catch (Exception)
            {
                _logger.LogError("\r\n-----------------------\r\n  Report Upload Occered!  \r\n-----------------------\r\n", pid);
            }

            return RedirectToAction("Index", "Timeline");
        }
    }
}
