﻿namespace Restaurant_API.models
{
    public class NewUser
    {
        public string Uuid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime Dob { get; set; }
        public string Password { get; set; }

        public NewUser() { }

        public NewUser(string uuid, string name, string email, DateTime dob, string password)
        {
            Uuid = uuid;
            Name = name;
            Email = email;
            Dob = dob;
            Password = password;
        }
    }
}
