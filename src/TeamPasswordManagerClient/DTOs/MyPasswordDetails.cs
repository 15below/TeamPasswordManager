using System;

namespace TeamPasswordManagerClient.DTOs
{
    public class MyPasswordDetails
    {
        public int Id;
        public string Name;
        public string Tags;
        public string Access_Info;
        public string Username;
        public string Email;
        public string Password;
        public string Notes;
        public DateTime Created_On;
        public User Created_By;
        public DateTime Updated_On;
        public User Updated_By;
    }
}
