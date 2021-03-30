using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILogRepository _cloudLogger;
        
        public HomeController(ILogger<HomeController> logger, IBlogsRepository _blogsRepo, ILogRepository cloudLogger)
        {
            _logger = logger;
            _cloudLogger = cloudLogger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("user accessed index method");
            _cloudLogger.Log("Home Index just accessed", Google.Cloud.Logging.Type.LogSeverity.Info);

            try
            {
                throw new Exception("with a custom message");
            }
            catch (Exception ex)
            {
                _cloudLogger.Log(ex.Message, Google.Cloud.Logging.Type.LogSeverity.Error);
            }


            return View();
        }

        public IActionResult Privacy()
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
