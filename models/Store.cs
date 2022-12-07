namespace Restaurant_API.models
{
    public class Store
    {
        public string StoreId { get; set; } = "";
        public string StoreName { get; set; } = "";
        public string Address1 { get; set; } = "";
        public string? Address2 { get; set; } = null;
        public string City { get; set; } = "";
        public string State { get; set; } = "";
        public int PostalCode { get; set; }
        public Dictionary<string, List<string>> StoreHours { get; set; } = new Dictionary<string, List<string>>() { };

        public Store() { }
        public Store(string storeId, string storeName, string address1, string address2, string city, string state, int postalCode, Dictionary<string, List<string>> storeHours)
        {
            StoreId = storeId;
            StoreName = storeName;
            Address1 = address1;
            Address2 = string.IsNullOrEmpty(address2) ? null : address2;
            City = city;
            State = state;
            PostalCode = postalCode;
            StoreHours = storeHours != null ? storeHours : new Dictionary<string, List<string>>();
        }
    }
}
