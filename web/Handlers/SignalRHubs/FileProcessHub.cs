
using Domain.Extensions;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using web.Database;
using System;
using System.IO;
using Docnet.Core;
using Docnet.Core.Models;
using System.Reflection;
using Tesseract;
using web.Models;
using System.Drawing;
using System.Drawing.Imaging;

namespace web.Handlers.SignalRHubs
{
    public class FileProcessHub : Hub
    {
        private readonly EfDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public FileProcessHub(EfDbContext context,
            SignInManager<IdentityUser> signInManager,
                   UserManager<IdentityUser> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
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
            catch (ApplicationException aex)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", CommunicationHandle.Send("Opa!", aex.Message, "error"));
            }
        }

        public decimal? IsProcessInProgress(string filename)
        {
            filename = filename.ToOnlyText();
            var fileReceived = _context.ReceivedFiles.FirstOrDefault(f => f.Hash.Equals(filename));
            return fileReceived?.Progress;
        }

        public bool FinalizeProcess(string filename)
        {
            filename = filename.ToOnlyText();
            var exists = _context.ReceivedFiles.Where(f => f.Hash.Equals(filename));
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
                        .Send("Ok", "Iniciamos o processamento do seu arquivo! Acompanhe o progresso!"));

            string fileFolder = @"Files";
            var fileFolderPath = Path.GetFullPath(fileFolder);

            var _docLib = DocLib.Instance;
            using (FileStream fsSource = new FileStream($"{fileFolderPath}/{filename.ToOnlyText()}", FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new byte[fsSource.Length];
                int numBytesToRead = (int)fsSource.Length;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    int n = fsSource.Read(bytes, numBytesRead, numBytesToRead);
                    if (n == 0)
                        break;

                    numBytesRead += n;
                    numBytesToRead -= n;
                }
                numBytesToRead = bytes.Length;

                var docReader = _docLib.GetDocReader(bytes, new PageDimensions(1080, 1920));
                var pdfVersion = docReader.GetPdfVersion();
                int totalPages = docReader.GetPageCount();

                var tesseractdatafilePatch = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\tesseractdatafile";

                using (var engine = new TesseractEngine(tesseractdatafilePatch, "por", EngineMode.Default))
                {
                    string pageImagesFolder = @"PageImages";
                    var pageImagesPath = Path.GetFullPath(pageImagesFolder);

                    if (File.Exists(pageImagesPath))
                        Directory.Delete(pageImagesPath);

                    Directory.CreateDirectory(pageImagesPath);

                    if (!System.IO.File.Exists(fileFolderPath))
                        Directory.CreateDirectory(fileFolderPath);

                    var fileFullPath = $"{fileFolderPath}/{filename.ToOnlyText()}";

                    var fileProcessResult = new FileProcessResult()
                    {
                        Success = true,
                        ImagesFound = CountImagesHandle.GetTotal(fileFullPath),
                        Message = "Arquivo processado com sucesso",
                        TotalPages = totalPages
                    };

                    for (int i = 0; i < totalPages; i++)
                    {
                        try
                        {
                            var pageReader = docReader.GetPageReader(i);

                            //Pega o texto da página
                            var pdfText = pageReader.GetText();
                            if (pdfText.Length == 0)
                                pdfText = "x";

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

                                    int percent = (int)((((decimal)i + 1.0m) / (decimal)totalPages) * 100);

                                    await Clients.User(Context.UserIdentifier).SendAsync("UpdateStatus",
                                        percent,
                                        $"Terminamos de processar a página {i + 1}");

                                    await Clients.User(Context.UserIdentifier).SendAsync("WritePageResult",
                                        pageProcessResult);
                                }
                            }
                        }
                        catch (Exception pex)
                        {
                            fileProcessResult.PagesResult.Add(new PageProcessResult()
                            {
                                Page = i + 1,
                                OCRSuccessRate = 0,
                                Text = pex.Message + pex.StackTrace
                            });
                            continue;
                        }
                    }

                    fileProcessResult = FileResultHandle.CalcResult(fileProcessResult);
                    await Clients.User(Context.UserIdentifier).SendAsync("WriteFileResult", fileProcessResult);
                }
            }

            await Clients.User(Context.UserIdentifier).SendAsync("UpdateStatus", 0, "O processamento do seu arquivo foi finalizado!");
            FinalizeProcess(filename);
            await Clients.User(Context.UserIdentifier)
                            .SendAsync("ReceiveMessage", CommunicationHandle
                            .Send("Sucesso", "O processamento do seu arquivo foi finalizado!"));
            await Clients.User(Context.UserIdentifier).SendAsync("FinalizeProcess");
        }

    }
}

