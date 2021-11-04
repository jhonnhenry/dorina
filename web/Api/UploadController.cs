using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using web.Api.Handlers;
using web.Api.Model;
using web.Models;

namespace web.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IConfiguration _config;

        public UploadController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        [ProducesResponseType(typeof(FileProcessResult), 200)]
        public async Task<IActionResult> SendAsync(List<IFormFile> file)
        {
            try
            {
                if (file == null || file.Count == 0)
                {
                    throw new Exception("Você precisa informar um arquivo.");
                }

                if (file.Count > 1)
                {
                    throw new Exception("Você só pode enviar um arquivo por vez.");
                }

                var theFile = file[0];

                var ext = Path.GetExtension(theFile.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(ext) || ext != ".pdf")
                {
                    throw new Exception("Você precisa informar um arquivo PDF.");
                }

                string tempImagesFolder = _config.GetValue<string>("App:TempImagesFolder");
                var resultOfProccess = await new ApiHandleFile().GetResultAsync(theFile, tempImagesFolder);
                return StatusCode((int)HttpStatusCode.Accepted, resultOfProccess);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResponseApim(false, ex.Message));
            }
        }
    }
}
