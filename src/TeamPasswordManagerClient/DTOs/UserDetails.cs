using System;
using System.Collections.Generic;

namespace TeamPasswordManagerClient.DTOs
{
    public class UserDetails
    {
        public int Id;
        public string Name;
        public string Username;
        public string Email_Address;
        public string Role;
        public bool Is_Active;
        public bool Is_LDAP;
        public bool Is_2FA_Enabled;
        public bool Valid_Hash;
        public int Num_Groups;
        public List<GroupEntry> Groups;
        public DateTime Last_Login;
        public DateTime Last_Api_Request;
        public DateTime Created_On;
        public User Created_By;
        public DateTime Updated_On;
        public User Updated_By;
    }
}
