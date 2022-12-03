namespace Restaurant_API.response
{
    public class Response
    {
        public int StatusCode { get; set; }
        public object? Data { get; set; }

        public Response() { }
        public Response(int status, object data)
        {
            StatusCode = status;
            Data = data;
        }
    }
}
