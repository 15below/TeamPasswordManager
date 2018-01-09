using System;

namespace TeamPasswordManagerClient.DTOs
{
    public class PasswordDetails
    {
        public int Id;
        public string Name;
        public string Password;
        public ProjectEntry Project;
        public string Notes_Snippet;
        public string Tags;
        public string Access_Info;
        public string Username;
        public string Email;
        public DateTime? Expiry_Date;
        public int Expiry_Status;
        public bool Archived;
        public bool Favorite;
        public int Num_Files;
        public bool Locked;
        public bool External_Sharing;
        public DateTime Updated_On;
    }
}
