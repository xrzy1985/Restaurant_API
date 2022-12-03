namespace Restaurant_API.models
{
    public class User
    {
        public string Uuid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime Dob { get; set; }

        public User() { }

        public User(string uuid, string name, string email, DateTime dob)
        {
            Uuid = uuid;
            Name = name;
            Email = email;
            Dob = dob;
        }
    }
}
