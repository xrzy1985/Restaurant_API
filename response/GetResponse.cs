namespace Restaurant_API.response
{
    public class GetResponse
    {
        public int StatusCode { get; set; }
        public object? Data { get; set; }

        public GetResponse() { }
        public GetResponse(int status, object data)
        {
            StatusCode = status;
            Data = data;
        }
    }
}
