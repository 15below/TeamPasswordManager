using Newtonsoft.Json;

namespace TeamPasswordManagerClient.DTOs
{
    public class CreateProjectRequest
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("parent_id")]
        public int Parent_Id;

        [JsonProperty("tags")]
        public string Tags;

        [JsonProperty("notes")]
        public string Notes;
    }
}
