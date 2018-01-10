using Newtonsoft.Json;

namespace TeamPasswordManagerClient.DTOs
{
    public class CreateMyPasswordRequest
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("tags")]
        public string Tags;

        [JsonProperty("access_info")]
        public string Access_Info;

        [JsonProperty("username")]
        public string Username;

        [JsonProperty("email")]
        public string Email;

        [JsonProperty("password")]
        public string Password;

        [JsonProperty("notes")]
        public string Notes;
    }
}
