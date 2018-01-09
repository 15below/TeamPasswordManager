using System;

namespace TeamPasswordManagerClient.DTOs
{
    public class ProjectDetails
    {
        public int Id;
        public string Name;
        public string Tags;
        public string Notes;
        public User Managed_By;
        public bool Archived;
        public bool Favorite;
        public int Num_Files;
        public DateTime Updated_On;
    }
}
