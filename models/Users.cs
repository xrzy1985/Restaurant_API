namespace Restaurant_API.models
{
    public class Users
    {
        public List<User> users { get; set; }

        public Users() { }
        public Users(List<User> users)
        {
            this.users = users;
        }
    }
}
