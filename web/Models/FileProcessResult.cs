using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using web.Handlers;

namespace web.Models
{
    public class FileProcessResult
    {
        public FileProcessResult()
        {
            PagesResult = new List<PageProcessResult>();
        }
        public bool Success { get; set; }
        public string Filename { get; internal set; }
        public string Message { get; internal set; }
        public List<PageProcessResult> PagesResult { get; set; }
    }
}
