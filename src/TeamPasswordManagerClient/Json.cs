using Newtonsoft.Json;

namespace TeamPasswordManagerClient
{
    internal static class Json
    {
        private static JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public static string ToString<T>(T obj) => JsonConvert.SerializeObject(obj, Formatting.Indented, settings);

        public static T ToObject<T>(string json) => JsonConvert.DeserializeObject<T>(json);
    }
}
