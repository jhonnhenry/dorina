namespace web.Api.Model
{
    public class ResponseApim
    {

        public ResponseApim(bool success, string message, object result = null)
        {
            this.Success = success;
            this.Message = message;
            this.Result = result;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
        public object Result { get; set; }
    }
}
