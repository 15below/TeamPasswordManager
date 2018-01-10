using Newtonsoft.Json;

namespace TeamPasswordManagerClient.DTOs
{
    internal class UpdateGroupRequest
    {
        [JsonProperty("name")]
        public string Name;
    }
}
