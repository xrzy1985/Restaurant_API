namespace Restaurant_API.models
{
    public class Disposition
    {
        public string StoreId { get; set; }
        public Boolean Curbside { get; set; }
        public Boolean Delivery { get; set; }
        public Boolean Pickup { get; set; }

        public Disposition() { }

        public Disposition(string storeId, bool curbside, bool delivery, bool pickup)
        {
            StoreId = storeId;
            Curbside = curbside;
            Delivery = delivery;
            Pickup = pickup;
        }
    }
}
