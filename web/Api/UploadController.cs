using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using web.Handlers;

namespace web.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        [HttpPost]
        public IActionResult Send(List<IFormFile> theFile)
        {
            return Ok(new HandleFile().GetResultAsync(theFile));
        }
    }
}
