using Newtonsoft.Json;

namespace TeamPasswordManagerClient.DTOs
{
    internal class ConvertToLdapRequest
    {
        [JsonProperty("login_dn")]
        public string LoginDN;
    }
}
