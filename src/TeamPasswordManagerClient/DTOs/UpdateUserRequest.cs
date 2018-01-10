using Newtonsoft.Json;

namespace TeamPasswordManagerClient.DTOs
{
    public class UpdateUserRequest
    {
        [JsonProperty("username")]
        public string Username;

        [JsonProperty("email_address")]
        public string EmailAddress;

        [JsonProperty("name")]
        public string Name;

        /// <summary>
        /// See Roles class for valid enumerations
        /// </summary>
        [JsonProperty("role")]
        public string Role;

        /// <summary>
        /// Only if the user is an LDAP user
        /// </summary>
        [JsonProperty("login_dn")]
        public string LoginDN;
    }
}
