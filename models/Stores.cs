namespace Restaurant_API.models
{
    public class Stores
    {
        public List<Store> StoresList { get; set; }

        public Stores() { }

        public Stores(List<Store> stores)
        {
            StoresList = stores;
        }
    }
}
