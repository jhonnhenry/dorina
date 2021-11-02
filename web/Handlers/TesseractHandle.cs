
using System.IO;
using System.Reflection;

using Tesseract;

namespace web.Handlers
{
    public static class TesseractHandle
    {
        public static TesseractEngine GetEngine()
        {
            var tesseractdatafilePatch = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\tesseractdatafile";
            return new TesseractEngine(tesseractdatafilePatch, "por", EngineMode.Default);
        }
    }
}
