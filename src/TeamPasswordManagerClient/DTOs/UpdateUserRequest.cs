namespace TeamPasswordManagerClient.DTOs
{
    public class UpdateUserRequest
    {
        public string Username;
        public string EmailAddress;
        public string Name;

        /// <summary>
        /// See Roles class for valid enumerations
        /// </summary>
        public string Role;

        /// <summary>
        /// Only if the user is an LDAP user
        /// </summary>
        public string LoginDN;
    }
}
