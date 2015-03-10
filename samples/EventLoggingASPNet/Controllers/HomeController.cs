using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;
using System;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace EventLoggingASPNet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger _logger;

        public HomeController(ILoggerFactory loggerFactory) {
            _logger = loggerFactory.Create("test");
        }

        // GET: /<controller>/
        [HttpGet("/")]
        public IActionResult Index() {

            // IIS Express host currently does not log the exceptions.
            // According to https://github.com/aspnet/Logging/issues/83#issuecomment-68151062 ,
            // I am to treat this as a bug, which I am working around to force the exception to be logged here.
            try {
                throw new InvalidOperationException("skhahksahj");
            } catch (Exception ex) {
                _logger.WriteError("xyzzy", ex);
            }

            return View();
        }
    }
}
