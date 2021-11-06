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
    [Route("api/v1/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IConfiguration _config;

        public UploadController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        [ProducesResponseType(typeof(FileProcessResult), 200)]
        public async Task<IActionResult> SendAsync(IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    throw new Exception("Você precisa informar um arquivo.");
                }

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(ext) || ext != ".pdf")
                {
                    throw new Exception("Você precisa informar um arquivo PDF.");
                }

                string tempImagesFolder = _config.GetValue<string>("App:TempImagesFolder");
                var resultOfProccess = await new ApiHandleFile().GetResultAsync(file, tempImagesFolder);
                return StatusCode((int)HttpStatusCode.Accepted, resultOfProccess);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResponseApim(false, ex.Message));
            }
        }
    }
}
