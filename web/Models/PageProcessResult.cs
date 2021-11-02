namespace web.Models
{
    public class PageProcessResult
    {
        public int Page { get; internal set; }
        public string Text { get; set; }
        public string OCRText { get; set; }
        public int TotalCharacters { get; set; }
        public int TotalCharactersFromOCR { get; set; }
        public float OCRSuccessRate { get; set; }
        public float DiffPercent { get; set; }
        public string FinalResult { get; set; }
        public string Base64Image { get; internal set; }
    }
}
