namespace TeamPasswordManagerClient.DTOs
{
    public class UserEntry
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
    }
}
