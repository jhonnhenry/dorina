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

using org.pdfclown.bytes;
using org.pdfclown.documents;
using files = org.pdfclown.files;
using org.pdfclown.objects;

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
                                    ImagesFound = await CountImagesAsync(formFile),
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
                                        BitmapHandle.AddBytes(bmp, rawBytes);

                                        string pageImagefilename = $"{pageImagesPath}\\{DateTime.Now.Ticks.ToString()}.png";
                                        bmp.Save(pageImagefilename, System.Drawing.Imaging.ImageFormat.Png);

                                        using (var pix = Pix.LoadFromFile(pageImagefilename))
                                        {
                                            using (var page = engine.Process(pix))
                                            {
                                                //Pega o texto da imagem
                                                string theTextOfImage = page.GetText();

                                                fileProcessResult.PagesResult.Add(new PageProcessResult()
                                                {
                                                    Page = i + 1,
                                                    OCRSuccessRate = page.GetMeanConfidence(),
                                                    Text = pdfText.Replace("\n", " ").Replace("\r", ""),
                                                    OCRText = theTextOfImage.Replace("\n", " ").Replace("\r", "")
                                                });
                                            }
                                        }
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



        public async Task<int> CountImagesAsync(IFormFile formFile)
        {
            int count = 0;
            var filePath = Path.GetTempFileName();
            using (var stream = File.Create(filePath))
            {
                await formFile.CopyToAsync(stream);
            }
            using (files::File file = new files::File(filePath))
            {
                foreach (PdfIndirectObject indirectObject in file.IndirectObjects)
                {
                    // Get the data object associated to the indirect object!
                    PdfDataObject dataObject = indirectObject.DataObject;
                    // Is this data object a stream?
                    if (dataObject is PdfStream)
                    {
                        PdfDictionary header = ((PdfStream)dataObject).Header;
                        // Is this stream an image?
                        if (header.ContainsKey(PdfName.Type)
                          && header[PdfName.Type].Equals(PdfName.XObject)
                          && header[PdfName.Subtype].Equals(PdfName.Image))
                        {
                            count++;
                        }
                    }
                }
            }
            return count;
        }
    }





}
