namespace TeamPasswordManagerClient.DTOs
{
    public class CreateLdapUserRequest
    {
        public string Username;
        public string EmailAddress;
        public string Name;
        public string Role;
        public string LoginDN;
    }
}
