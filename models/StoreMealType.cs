namespace Restaurant_API.models
{
    public class StoreMealType
    {
        public string Type { get; set; }
        public string Time { get; set; }

        public StoreMealType() { }

        public StoreMealType(string type, string time)
        {
            Type = type;
            Time = time;
        }
    }
}
