using Newtonsoft.Json;

namespace TeamPasswordManagerClient.DTOs
{
    public class CreateNormalUserRequest
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

        [JsonProperty("password")]
        public string Password;
    }
}
