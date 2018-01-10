using Newtonsoft.Json;
using System;

namespace TeamPasswordManagerClient.DTOs
{
    public class UpdatePasswordRequest
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

        [JsonProperty("expiry_date")]
        public DateTime? Expiry_Date;

        [JsonProperty("notes")]
        public string Notes;

        public static UpdatePasswordRequest FromDetails(PasswordDetails details)
        {
            return new UpdatePasswordRequest
            {
                Name = details.Name,
                Tags = details.Tags,
                Access_Info = details.Access_Info,
                Username = details.Username,
                Email = details.Email,
                Password = details.Password,
                Expiry_Date = details.Expiry_Date,
                Notes = details.Notes
            };
        }
    }
}
