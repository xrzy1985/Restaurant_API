namespace Restaurant_API.models
{
    public class Store
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string Address1 { get; set; }
        public string? Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int PostalCode { get; set; }
    }
}
