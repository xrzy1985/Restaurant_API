namespace Restaurant_API.response
{
    public class ErrorResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }

        public ErrorResponse() { }

        public ErrorResponse(int status, string message) { Status = status; Message = message; }
    }
}
