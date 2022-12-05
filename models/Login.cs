namespace Restaurant_API.models
{
    public class Login
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public Login() { }
        public Login(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
