using Newtonsoft.Json;

namespace TeamPasswordManagerClient.DTOs
{
    internal class ChangeProjectParent
    {
        [JsonProperty("parent_id")]
        public int Parent_Id;
    }
}
