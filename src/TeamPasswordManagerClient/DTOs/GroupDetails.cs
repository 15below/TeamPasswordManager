using System;
using System.Collections.Generic;

namespace TeamPasswordManagerClient.DTOs
{
    public class GroupDetails
    {
        public int Id;
        public string Name;
        public List<User> Users;
        public DateTime Created_On;
        public User Created_By;
        public DateTime Updated_On;
        public User Updated_By;
    }
}
