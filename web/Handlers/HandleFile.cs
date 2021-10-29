using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Docnet.Core.Models;
using Docnet.Core;
using System.Drawing;
using System.Runtime.InteropServices;
using Tesseract;
using System.Reflection;
using System.Drawing.Imaging;
using web.Models;

namespace web.Handlers
{
    public class HandleFile
    {
        public async Task<List<FileProcessResult>> GetResultAsync(List<IFormFile> theFile)
        {
            List<FileProcessResult> uploadResult = new List<FileProcessResult>();
            try
            {
                foreach (var formFile in theFile)
                {
                    if (formFile.Length > 0)
                    {
                        var filePath = Path.GetTempFileName();
                        var _docLib = DocLib.Instance;

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            await formFile.CopyToAsync(memoryStream);

                            var docReader = _docLib.GetDocReader(memoryStream.ToArray(), new PageDimensions(1080, 1920));
                            var pdfVersion = docReader.GetPdfVersion();
                            int totalPages = docReader.GetPageCount();

                            //Tesseract
                            //string tesseractdatafileFolder = @"tesseractdatafile";
                            //var tesseractdatafilePatch = Path.GetFullPath(tesseractdatafileFolder);
                            var tesseractdatafilePatch = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\tesseractdatafile";

                            using (var engine = new TesseractEngine(tesseractdatafilePatch, "por", EngineMode.Default))
                            {
                                string pageImagesFolder = @"PageImages";
                                var pageImagesPath = Path.GetFullPath(pageImagesFolder);

                                if (File.Exists(pageImagesPath))
                                    Directory.Delete(pageImagesPath);

                                Directory.CreateDirectory(pageImagesPath);

                                var fileProcessResult = new FileProcessResult()
                                {
                                    Success = true,
                                    Filename = formFile.FileName,
                                    Message = "Arquivo processado com sucesso"
                                };

                                for (int i = 0; i < totalPages; i++)
                                {
                                    try
                                    {
                                        var pageReader = docReader.GetPageReader(i);










                                        //Pega o texto da página
                                        var pdfText = pageReader.GetText();

                                        //Transforma a página pra imagem
                                        var width = pageReader.GetPageWidth();
                                        var height = pageReader.GetPageHeight();
                                        var rawBytes = pageReader.GetImage();
                                        var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                                        AddBytes(bmp, rawBytes);

                                        string pageImagefilename = $"{pageImagesPath}\\{DateTime.Now.Ticks.ToString()}.png";
                                        bmp.Save(pageImagefilename, System.Drawing.Imaging.ImageFormat.Png);

                                        using (var pix = Pix.LoadFromFile(pageImagefilename))
                                        {
                                            using (var page = engine.Process(pix))
                                            {
                                                string rate = String.Format("{0:P2}", page.GetMeanConfidence());
                                                //Pega o texto da imagem
                                                string theTextOfImage = page.GetText();

                                                fileProcessResult.PagesResult.Add(new PageProcessResult()
                                                {
                                                    Page = i + 1,
                                                    Success = true,
                                                    SuccessRate = rate,
                                                    Text = pdfText,
                                                    OCRText = theTextOfImage
                                                });
                                            }
                                        }
                                    }
                                    catch (Exception pex)
                                    {
                                        fileProcessResult.PagesResult.Add(new PageProcessResult()
                                        {
                                            Page = i,
                                            Success = false,
                                            SuccessRate = "0%",
                                            Text = pex.Message + pex.StackTrace
                                        });
                                        continue;
                                    }
                                }

                                uploadResult.Add(fileProcessResult);
                            }
                        }
                        _docLib.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                uploadResult.Add(new FileProcessResult()
                {
                    Success = false,
                    Message = ex.Message + ex.StackTrace
                });
            }

            if (uploadResult.Count == 0)
            {
                uploadResult.Add(new FileProcessResult()
                {
                    Success = false,
                    Message = "Nenhum arquivo analizado"
                });
            }
            return uploadResult;
        }


        private static void AddBytes(Bitmap bmp, byte[] rawBytes)
        {
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

            var bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);
            var pNative = bmpData.Scan0;

            Marshal.Copy(rawBytes, 0, pNative, rawBytes.Length);
            bmp.UnlockBits(bmpData);
        }
    }





}
