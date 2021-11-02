namespace web.Handlers
{
    public static class PageResultHandle
    {
        public static string CalcResult(int result)
        {
            if(result < 20)
            {
                return "Excelente";
            } else if (result >= 20 && result < 50)
            {
                return "Bom";
            }
            else if (result >= 50 && result < 100)
            {
                return "Regular";
            }
            else
            {
                return "Ruim";
            }
        }
    }
}
