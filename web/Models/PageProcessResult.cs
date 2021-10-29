using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.Models
{
    public class PageProcessResult
    {
        public bool Success { get; set; }
        public string SuccessRate { get; set; }
        public string Text { get; set; }
        public int Page { get; internal set; }
        public string OCRText { get; internal set; }
    }
}
