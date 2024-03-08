using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using web.Handlers;
using web.Models;
using System.IO;
using Domain.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using web.Models.DatabaseModels;

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

        public IActionResult Error()
        {
            return View();
        }

        public async Task<IActionResult> IndexAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                var randomUsername = Guid.NewGuid().ToString().ToOnlyText();
                var user = await _userManager.FindByNameAsync(randomUsername);
                if (user == null)
                {
                    user = new IdentityUser()
                    {
                        UserName = randomUsername,
                        Email = $"{randomUsername}@dorina.org"
                    };
                    var result = await _userManager.CreateAsync(user);
                }
                await _signInManager.SignInAsync(user, true);
            }
            return View();
        }

        [HttpPost]
        [Route("/Home/UploadFile")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<JsonResult> UploadFileAsync(List<IFormFile> theFile)
        {
            try
            {
                string tempFileFolder = _config.GetValue<string>("App:TempFileFolder");
                var fileFolderPath = Path.GetFullPath(tempFileFolder);

                if (theFile == null || theFile.Count == 0)
                {
                    throw new Exception("Você precisa informar um arquivo PDF");
                }

                var sendedFile = theFile[0];
                var normalizedFilename = sendedFile.FileName.ToOnlyText();

                var exists = _context.ReceivedFiles.FirstOrDefault(f =>
                    f.Username.Equals(User.Identity.Name));
                if (exists != null)
                {
                    throw new Exception("Você só pode analisar um arquivo por vez.");
                }

                if (!System.IO.File.Exists(fileFolderPath))
                    Directory.CreateDirectory(fileFolderPath);
                using (var stream = System.IO.File.Create($"{fileFolderPath}/{sendedFile.FileName.ToOnlyText()}"))
                {
                    await sendedFile.CopyToAsync(stream);
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
