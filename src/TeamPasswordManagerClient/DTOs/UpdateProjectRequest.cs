using Newtonsoft.Json;

namespace TeamPasswordManagerClient.DTOs
{
    public class UpdateProjectRequest
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("tags")]
        public string Tags;

        [JsonProperty("notes")]
        public string Notes;
    }
}
