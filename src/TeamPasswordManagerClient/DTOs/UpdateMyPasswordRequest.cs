using Newtonsoft.Json;

namespace TeamPasswordManagerClient.DTOs
{
    public class UpdateMyPasswordRequest
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

        public static UpdateMyPasswordRequest FromDetails(MyPasswordDetails details)
        {
            return new UpdateMyPasswordRequest
            {
                Name = details.Name,
                Tags = details.Tags,
                Access_Info = details.Access_Info,
                Username = details.Username,
                Email = details.Email,
                Password = details.Password,
                Notes = details.Notes
            };
        }
    }
}
