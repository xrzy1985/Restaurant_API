namespace Restaurant_API.models
{
    public class Users
    {
        public int Id { get; set; }
        public string Uuid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime Dob { get; set; }

        public Users() { }

        public Users(int id, string uuid, string name, string email, DateTime dob)
        {
            Id = id;
            Uuid = uuid;
            Name = name;
            Email = email;
            Dob = dob;
        }

    }
}
