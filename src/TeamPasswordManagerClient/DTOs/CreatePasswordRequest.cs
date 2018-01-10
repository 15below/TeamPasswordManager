using Newtonsoft.Json;

namespace TeamPasswordManagerClient.DTOs
{
    public class CreatePasswordRequest
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("project_id")]
        public int Project_Id;

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

        [JsonProperty("expiry_date")]
        public string Expiry_Date;

        [JsonProperty("notes")]
        public string Notes;
    }
}
