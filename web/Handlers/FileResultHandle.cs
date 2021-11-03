using System.Linq;

using web.Models;

namespace web.Handlers
{
    public static class FileResultHandle
    {
        public static FileProcessResult CalcResult(FileProcessResult fileProcessResult)
        {
            fileProcessResult.OCRSuccessAverageRate = float.Parse(fileProcessResult.PagesResult.Average(a => a.OCRSuccessRate).ToString("F0"));
            fileProcessResult.AverageDiffPercent = float.Parse(fileProcessResult.PagesResult.Average(a => a.DiffPercent).ToString("F0"));
            
            if(fileProcessResult.AverageDiffPercent < 60)
            {
                fileProcessResult.AverageDiffPercentDisplay = "Baixa";
            }
            else if (fileProcessResult.AverageDiffPercent >= 60 && fileProcessResult.AverageDiffPercent < 100)
            {
                fileProcessResult.AverageDiffPercentDisplay = "Média";
            }
            else
            {
                fileProcessResult.AverageDiffPercentDisplay = "Alta";
            }
            fileProcessResult.FinalResult = CalcFinalResult(fileProcessResult);
            return fileProcessResult;
        }

        public static string CalcFinalResult(FileProcessResult fileProcessResult)
        {
            var result = fileProcessResult.AverageDiffPercent / fileProcessResult.TotalPages;
            if (result < 20)
            {
                return "Excelente. Seu documento é acessível";
            }
            else if (result >= 20 && result < 60)
            {
                return "Bom. Mas pode melhorar";
            }
            else
            {
                return "Ruim. Seu documento precisa passar por uma revisão.";
            }
        }
    }
}
