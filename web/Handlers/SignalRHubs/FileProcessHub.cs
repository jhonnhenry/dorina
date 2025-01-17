﻿
using Domain.Extensions;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;

using System;
using System.IO;
using Docnet.Core;
using Docnet.Core.Models;
using System.Reflection;
using Tesseract;
using web.Models;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Extensions.Configuration;
using web.Models.DatabaseModels;

namespace web.Handlers.SignalRHubs
{
    public class FileProcessHub : Hub
    {
        private readonly EfDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;
        private readonly string _tempImagesFolder;

        public FileProcessHub(EfDbContext context,
            SignInManager<IdentityUser> signInManager,
                   UserManager<IdentityUser> userManager,
                   IConfiguration config)
        {
            _context = context;
            _signInManager = signInManager;
            _config = config;
            _userManager = userManager;
            _tempImagesFolder = _config.GetValue<string>("App:TempImagesFolder");
        }
        public async Task SendMessage(string message, string parameter)
        {
            try
            {
                switch (message)
                {
                    case "startFileProcess":
                        await StartFileProcess(parameter);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", CommunicationHandle.Send("Erro!", ex.Message, MessageType.error));
            }
        }

        public bool IsProcessInProgress(string filename)
        {
            filename = filename.ToOnlyText();
            return _context.ReceivedFiles.Any(f =>
            f.Username.Equals(Context.User.Identity.Name)
            && f.Filename.Equals(filename));
        }

        public bool FinalizeProcess(string filename)
        {
            filename = filename.ToOnlyText();
            var exists = _context.ReceivedFiles.Where(f => f.Filename.Equals(filename));
            if (exists.Count() > 0)
            {
                _context.ReceivedFiles.RemoveRange(exists);
            }
            _context.SaveChanges();
            return true;
        }

        public async Task StartFileProcess(string filename)
        {
            filename = filename.ToOnlyText();
            await Clients.User(Context.UserIdentifier)
                        .SendAsync("ReceiveMessage", CommunicationHandle
                        .Send("Ok", "Iniciamos a análise do seu arquivo! Acompanhe o progresso abaixo!"));

            string tempFileFolder = _config.GetValue<string>("App:TempFileFolder");
            var tempFileFolderPath = Path.GetFullPath(tempFileFolder);

            var tempImagesFolderPath = Path.GetFullPath(_tempImagesFolder);
            var fileFullPath = $"{tempFileFolderPath}/{filename.ToOnlyText()}";

            if(File.Exists(fileFullPath))
            {
                _context.ReceivedFiles.Add(new ReceivedFile()
                {
                    Filename = filename,
                    Username = Context.User.Identity.Name,
                });
                _context.SaveChanges();
            }
            else
            {
                throw new Exception($"Houve um erro ao analisar o arquivo.");
            }

            byte[] bytesOfFile = FileStreamHandle.GetAsByteArray(fileFullPath);

            var _docLib = DocLib.Instance;
            var docReader = _docLib.GetDocReader(bytesOfFile, new PageDimensions(1080, 1920));

            // var pdfVersion = docReader.GetPdfVersion();
            int totalPages = docReader.GetPageCount();

            var tesseractEngine = TesseractHandle.GetEngine();

            var fileProcessResult = new FileProcessResult()
            {
                Success = true,
                ImagesFound = CountImagesHandle.GetTotal(fileFullPath),
                Message = "Arquivo analisado com sucesso",
                TotalPages = totalPages
            };

            for (int i = 0; i < totalPages; i++)
            {
                try
                {
                    var pageReader = docReader.GetPageReader(i);

                    //Get the text of page
                    var pdfText = pageReader.GetText();


                    //Make a image from page
                    var width = pageReader.GetPageWidth();
                    var height = pageReader.GetPageHeight();
                    var rawImageBytes = pageReader.GetImage(RenderFlags.OptimizeTextForLcd);

                    pageReader.Dispose();

                    var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                    BitmapHandle.AddBytes(bmp, rawImageBytes);

                    string pageImagefilename = $"{tempImagesFolderPath}\\{DateTime.Now.Ticks.ToString()}.jpeg";
                    bmp.Save(pageImagefilename, System.Drawing.Imaging.ImageFormat.Png);

                    var imageBase64 = Convert.ToBase64String(File.ReadAllBytes(pageImagefilename));

                    using (var pix = Pix.LoadFromFile(pageImagefilename))
                    {
                        using (var page = tesseractEngine.Process(pix))
                        {
                            //Get the text of image
                            string theTextOfImage = page.GetText();


                            var diff = theTextOfImage.Length - (pdfText.Length == 0 ? 1 : pdfText.Length);
                            var diffPercent = (int)(((decimal)diff / (decimal)(pdfText.Length == 0 ? 1 : pdfText.Length)) * 100);
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
                                FinalResult = PageResultHandle.CalcResult(diffPercent),
                                Base64Image = imageBase64
                            };

                            fileProcessResult.PagesResult.Add(pageProcessResult);
                            fileProcessResult.Text += pageProcessResult.Text;
                            fileProcessResult.OCRText += pageProcessResult.OCRText;

                            int percent = (int)((((decimal)i + 1.0m) / (decimal)totalPages) * 100);

                            await Clients.User(Context.UserIdentifier).SendAsync("UpdateStatus",
                                percent,
                                $"Terminamos de analisar a página {i + 1}");

                            await Clients.User(Context.UserIdentifier).SendAsync("WritePageResult",
                                pageProcessResult);
                        }
                    }
                }
                catch
                {
                    fileProcessResult.PagesResult.Add(new PageProcessResult()
                    {
                        Page = i + 1,
                        OCRSuccessRate = 0,
                        Text = "Ocorreu um erro não esperado e não conseguimos ler esta página."
                    });
                    continue;
                }
            }

            fileProcessResult = FileResultHandle.CalcResult(fileProcessResult);
            await Clients.User(Context.UserIdentifier).SendAsync("WriteFileResult", fileProcessResult);

            await Clients.User(Context.UserIdentifier).SendAsync("UpdateStatus", 0, "A análise do seu arquivo foi finalizada!");
            FinalizeProcess(filename);
            await Clients.User(Context.UserIdentifier)
                            .SendAsync("ReceiveMessage", CommunicationHandle
                            .Send("Sucesso", "A análise do seu arquivo foi finalizada!"));
            await Clients.User(Context.UserIdentifier).SendAsync("FinalizeProcess");

            var exists = _context.ReceivedFiles.FirstOrDefault(f =>
                    f.Filename.Equals(filename)
                    && f.Username.Equals(Context.User.Identity.Name));
            if (exists != null)
            {
                _context.ReceivedFiles.Remove(exists);
                _context.SaveChanges();
            }

            //Free resources
            tesseractEngine.Dispose();
            docReader.Dispose();

            //For not create garbage
            if (File.Exists(tempImagesFolderPath))
                Directory.Delete(tempImagesFolderPath);

            if (!File.Exists(tempFileFolderPath))
                Directory.CreateDirectory(tempFileFolderPath);

            Directory.CreateDirectory(tempImagesFolderPath);
            Directory.CreateDirectory(tempFileFolderPath);
        }

    }
}

