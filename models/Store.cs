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
        public Dictionary<string, List<string>> StoreHours { get; set; }

        public Store() { }
        public Store(int storeId, string storeName, string address1, string address2, string city, string state, int postalCode, Dictionary<string, List<string>> storeHours)
        {
            StoreId = storeId;
            StoreName = storeName;
            Address1 = address1;
            Address2 = address2;
            City = city;
            State = state;
            PostalCode = postalCode;
            StoreHours = storeHours != null ? storeHours : new Dictionary<string, List<string>>();
        }
    }
}
