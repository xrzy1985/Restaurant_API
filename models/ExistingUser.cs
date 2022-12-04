namespace Restaurant_API.models
{
    public class ExistingUser
    {
        public string Uuid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime Dob { get; set; }
        public ExistingUser() { }
        public ExistingUser(string uuid, string name, string email, DateTime dob)
        {
            Uuid = uuid;
            Name = name;
            Email = email;
            Dob = dob;
        }
    }
}
