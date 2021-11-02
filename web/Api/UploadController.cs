using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using web.Api.Handlers;
using web.Api.Model;

namespace web.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> SendAsync(List<IFormFile> file)
        {
            try
            {
                if (file.Count == 0)
                {
                    throw new Exception("Você precisa informar um arquivo.");
                }

                var theFile = file[0];

                var ext = Path.GetExtension(theFile.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(ext) || ext != ".pdf")
                {
                    throw new Exception("Você precisa informar um arquivo PDF.");
                }

                var resultOfProccess = await new ApiHandleFile().GetResultAsync(theFile);
                return Ok(resultOfProccess);
            }
            catch (Exception ex)
            {
                return Ok(new ResponseApim(false, ex.Message));
            }
        }
    }
}
