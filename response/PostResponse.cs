namespace Restaurant_API.response
{
    public class PostResponse
    {
        int Status { get; set; }
        string Message { get; set; }
        public PostResponse() { }
        public PostResponse(int status, string message) {
            Status = status;
            Message = message;
        }
    }
}
