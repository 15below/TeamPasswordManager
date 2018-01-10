using System;

namespace TeamPasswordManagerClient.DTOs
{
    public class PasswordEntry
    {
        public int Id;
        public string Name;
        public ProjectEntry Project;
        public string Notes_Snippet;
        public string Tags;
        public string Access_Info;
        public string Username;
        public string Email;
    }
}
