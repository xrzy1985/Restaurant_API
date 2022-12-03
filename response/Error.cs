namespace Restaurant_API.response
{
    public class Error
    {
        public int Status { get; set; }
        public string Message { get; set; }

        public Error() { }

        public Error(int status, string message) { Status = status; Message = message; }
    }
}
