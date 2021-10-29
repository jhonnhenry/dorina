using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using web.Handlers;
using web.Models;

namespace web.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("/Home/UploadFile")]
        [HttpPost]
        public IActionResult UploadFile(List<IFormFile> theFile)
        {
            return Ok(new HandleFile().GetResultAsync(theFile));
        }

    }
}
