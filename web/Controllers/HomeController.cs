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

namespace web.Controllers
{
    public class HomeController : Controller
    {
        private readonly EfDbContext _context;

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(EfDbContext context,
            SignInManager<IdentityUser> signInManager,
                   UserManager<IdentityUser> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }


        public async Task<IActionResult> IndexAsync()
        {
            var cookie = ControllerContext.HttpContext.Request.Cookies.FirstOrDefault(f=>f.Key.Contains("Antiforgery"));
            if(cookie.Value != null)
            {
                string antiforgeryCookie = cookie.Value.ToOnlyText();

                antiforgeryCookie = antiforgeryCookie.ToOnlyText();
                var user = await _userManager.FindByNameAsync(antiforgeryCookie);
                if(user == null)
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
            string filename = "";

            foreach (var formFile in theFile)
            {
                if (formFile.Length > 0)
                {
                    string fileFolder = @"Files";
                    var fileFolderPath = Path.GetFullPath(fileFolder);
                    if (!System.IO.File.Exists(fileFolderPath))
                        Directory.CreateDirectory(fileFolderPath);
                    using (var stream = System.IO.File.Create($"{fileFolderPath}/{formFile.FileName.ToOnlyText()}"))
                    {
                        await formFile.CopyToAsync(stream);
     
                        filename = formFile.FileName;
                        filename = filename.ToOnlyText();
                        var exists = _context.ReceivedFiles.FirstOrDefault(f => f.Hash.Equals(filename));
                        if(exists != null)
                        {
                            _context.ReceivedFiles.Remove(exists);
                        }
                        _context.ReceivedFiles.Add(new Database.DatabaseModels.ReceivedFile()
                        {
                            Hash = filename,
                            Progress = 0.1m
                        });
                        _context.SaveChanges();
                    }
                }
                break;
            }
            return Json(new { hash = filename });
        }

    }
}
