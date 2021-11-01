using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using web.Database;
using web.Handlers;
using web.Models;
using System.IO;
using Domain.Extensions;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace web.Controllers
{
    public class HomeController : Controller
    {
        private readonly EfDbContext _context;
        private readonly IConfiguration _config;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(EfDbContext context,
            SignInManager<IdentityUser> signInManager,
                   UserManager<IdentityUser> userManager,
                   IConfiguration config)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
        }


        public async Task<IActionResult> IndexAsync()
        {
            var cookie = ControllerContext.HttpContext.Request.Cookies.FirstOrDefault(f => f.Key.Contains("Antiforgery"));
            if (cookie.Value != null)
            {
                string antiforgeryCookie = cookie.Value.ToOnlyText();

                antiforgeryCookie = antiforgeryCookie.ToOnlyText();
                var user = await _userManager.FindByNameAsync(antiforgeryCookie);
                if (user == null)
                {
                    user = new IdentityUser()
                    {
                        UserName = antiforgeryCookie
                    };
                    var asd = await _userManager.CreateAsync(user);
                }
                await _signInManager.SignInAsync(user, true);
            }

            return View();
        }

        [Route("/Home/UploadFile")]
        [HttpPost]
        public async Task<JsonResult> UploadFileAsync(List<IFormFile> theFile)
        {
            try
            {
                string tempFileFolder = _config.GetValue<string>("App:TempFileFolder");
                var fileFolderPath = Path.GetFullPath(tempFileFolder);

                if (theFile[0] == null)
                {
                    throw new Exception("Você precisa informar um arquivo PDF");
                }

                var sendedFile = theFile[0];
                var normalizedFilename = sendedFile.FileName.ToOnlyText();

                if (!System.IO.File.Exists(fileFolderPath))
                    Directory.CreateDirectory(fileFolderPath);
                using (var stream = System.IO.File.Create($"{fileFolderPath}/{sendedFile.FileName.ToOnlyText()}"))
                {
                    await sendedFile.CopyToAsync(stream);

                    var exists = _context.ReceivedFiles.FirstOrDefault(f => f.Filename.Equals(sendedFile.FileName));
                    if (exists != null)
                    {
                        _context.ReceivedFiles.Remove(exists);
                    }
                    _context.ReceivedFiles.Add(new Database.DatabaseModels.ReceivedFile()
                    {
                        Filename = sendedFile.FileName,
                        Progress = 0.1m
                    });
                    _context.SaveChanges();
                }
                return Json(new { FileName = normalizedFilename });
            }
            catch (Exception ex)
            {
                return Json(CommunicationHandle.Send("Erro!", ex.Message, MessageType.error));
            }
        }
    }
}
