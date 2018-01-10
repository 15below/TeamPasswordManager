using Newtonsoft.Json;

namespace TeamPasswordManagerClient.DTOs
{
    internal class CreateGroupRequest
    {
        [JsonProperty("name")]
        public string Name;
    }
}
