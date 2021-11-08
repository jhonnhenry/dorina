using System.Collections.Generic;

namespace web.Models
{
    public class FileProcessResult
    {
        public FileProcessResult()
        {
            PagesResult = new List<PageProcessResult>();
        }
        public bool Success { get; set; }
        public string Filename { get; set; }
        public string Message { get; set; }
        public List<PageProcessResult> PagesResult { get; set; }
        public int ImagesFound { get; set; }
        public float OCRSuccessAverageRate { get; set; }
        public float AverageDiffPercent { get; set; }
        public string FinalResult { get; set; }
        public int TotalPages { get; set; }
        public string AverageDiffPercentDisplay { get; set; }
        public string Text { get; set; }
        public string OCRText { get; set; }
    }
}
