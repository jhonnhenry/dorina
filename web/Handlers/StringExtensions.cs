using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Domain.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Remove acentos de uma string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RemoveDiacritics(this string value)
        {
            return string.Concat(
                value.Normalize(NormalizationForm.FormD)
                .Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) !=
                                              UnicodeCategory.NonSpacingMark)
              ).Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Retorna apenas números
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToOnlyNumbers(this string source)
        {
            string sResult = Regex.Replace(source, @"[^0-9]", "").ToLower();
            return sResult;
        }

        /// <summary>
        /// Retorna apenas o texto
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToOnlyText(this string source)
        {
            string sResult = Regex.Replace(source, @"[^a-zA-Z]", "").ToLower();
            return sResult;
        }

        /// <summary>
        /// Retorna apenas texto e números
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string WithoutCharacters(this Guid source)
        {
            string sResult = Regex.Replace(source.ToString(), @"[^a-zA-Z0-9]", "").ToLower();
            return sResult;
        }

        /// <summary>
        /// Converte string com ponto pra decimal com vírgula
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this string source)
        {
            decimal sResult = decimal.Parse(source.Replace(",", "").Replace(".", ","));
            return sResult;
        }

        /// <summary>
        /// Formata números para CNPJ ou CPF
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToCNPJCPF(this string source)
        {
            if (source.ToOnlyNumbers().Length <= 11)
            {
                return Convert.ToUInt64(source.ToOnlyNumbers()).ToString(@"000\.000\.000\-00");
            }
            else
            {
                return Convert.ToUInt64(source.ToOnlyNumbers()).ToString(@"00\.000\.000\/0000\-00");
            }
        }


    }
}
