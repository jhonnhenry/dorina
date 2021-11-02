using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using Docnet.Core.Models;
using Docnet.Core;
using System.Drawing;
using Tesseract;
using System.Reflection;
using System.Drawing.Imaging;
using web.Models;
using web.Handlers;
using Microsoft.Extensions.Configuration;

namespace web.Api.Handlers
{
    public class ApiHandleFile
    {
        public async Task<FileProcessResult> GetResultAsync(IFormFile theFile, string tempImagesFolderPath)
        {
            var fileProcessResult = new FileProcessResult()
            {
                Success = true,
                Filename = theFile.FileName,
                Message = "Arquivo processado com sucesso"
            };

            try
            {
                var docLib = DocLib.Instance;

                byte[] bytesOfFile = await FileStreamHandle.GetAsByteArrayAsync(theFile);

                fileProcessResult.ImagesFound = PdfClownHandle.CountImages(bytesOfFile);

                var docReader = docLib.GetDocReader(bytesOfFile, new PageDimensions(1080, 1920));

                var pdfVersion = docReader.GetPdfVersion();
                int totalPages = docReader.GetPageCount();

                var tesseractEngine = TesseractHandle.GetEngine();

                if (!File.Exists(tempImagesFolderPath))
                    Directory.CreateDirectory(tempImagesFolderPath);

                for (int i = 0; i < totalPages; i++)
                {
                    try
                    {
                        var pageReader = docReader.GetPageReader(i);

                        //Get the text of page
                        var pdfText = pageReader.GetText();
                        if (pdfText.Length == 0)
                            pdfText = "-";

                        //Make a image from page
                        var width = pageReader.GetPageWidth();
                        var height = pageReader.GetPageHeight();
                        var rawBytes = pageReader.GetImage();
                        var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                        BitmapHandle.AddBytes(bmp, rawBytes);

                        string pageImagefilename = $"{tempImagesFolderPath}\\{DateTime.Now.Ticks.ToString()}.png";
                        bmp.Save(pageImagefilename, System.Drawing.Imaging.ImageFormat.Png);

                        using (var pix = Pix.LoadFromFile(pageImagefilename))
                        {
                            using (var page = tesseractEngine.Process(pix))
                            {
                                //Get the text of image
                                string theTextOfImage = page.GetText();

                                var diff = theTextOfImage.Length - pdfText.Length;
                                var diffPercent = (int)(((decimal)diff / (decimal)pdfText.Length) * 100);
                                var OCRSuccessRate = page.GetMeanConfidence();

                                var pageProcessResult = new PageProcessResult()
                                {
                                    Page = i + 1,
                                    TotalCharacters = pdfText.Length,
                                    TotalCharactersFromOCR = theTextOfImage.Length,
                                    OCRSuccessRate = OCRSuccessRate * 100,
                                    Text = pdfText.Replace("\n", " ").Replace("\r", ""),
                                    OCRText = theTextOfImage.Replace("\n", " ").Replace("\r", ""),
                                    DiffPercent = diffPercent,
                                    FinalResult = PageResultHandle.CalcResult(diffPercent)
                                };
                                fileProcessResult.PagesResult.Add(pageProcessResult);
                            }
                        }

                        fileProcessResult = FileResultHandle.CalcResult(fileProcessResult);
                    }
                    catch (Exception pex)
                    {
                        fileProcessResult.PagesResult.Add(new PageProcessResult()
                        {
                            Page = i,
                            OCRSuccessRate = 0,
                            Text = pex.Message + pex.StackTrace
                        });
                        continue;
                    }
                }

                docLib.Dispose();
                tesseractEngine.Dispose();

                if (!File.Exists(tempImagesFolderPath))
                    Directory.Delete(tempImagesFolderPath);
            }
            catch (Exception ex)
            {
                fileProcessResult.Success = false;
                fileProcessResult.Message = ex.Message;
            }

            return fileProcessResult;
        }
    }
}
