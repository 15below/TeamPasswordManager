using TeamPasswordManagerClient.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamPasswordManagerClient
{
    public interface ITpmPersonalPasswordClient
    {
        int CreatePersonalPassword(string name, string password);
        void DeletePersonalPassword(int personalPasswordId);
        PasswordDetails GetPersonalPassword(int personalPasswordId);
        PasswordDetails GetPersonalPassword(string passwordName);
        List<PasswordEntry> ListAllPersonalPasswords();
        List<PasswordEntry> ListPersonalPasswords(int page = 1);
        void UpdatePersonalPassword(PasswordDetails details);
    }

    public class TpmPersonalPasswordClient : TpmBase, ITpmPersonalPasswordClient
    {
        public TpmPersonalPasswordClient(TpmConfig config) : base(config)
        {
        }

        public List<PasswordEntry> ListAllPersonalPasswords()
        {
            return FetchAllPages((page) => ListPersonalPasswords(page), 20);
        }

        public List<PasswordEntry> ListPersonalPasswords(int page = 1)
        {
            var response = (page == 1) ? Get($"api/v4/my_passwords.json") : Get($"api/v4/my_passwords/page/{page}.json");
            return JsonConvert.DeserializeObject<List<PasswordEntry>>(response);
        }

        public PasswordDetails GetPersonalPassword(int personalPasswordId)
        {
            var response = Get($"api/v4/my_passwords/{personalPasswordId}.json");
            return JsonConvert.DeserializeObject<PasswordDetails>(response);
        }

        public PasswordDetails GetPersonalPassword(string passwordName)
        {
            var urlencodedPassword = HttpUtility.UrlEncode($"[{passwordName}]");
            var response = Get($"api/v4/my_passwords/search/name:{urlencodedPassword}.json");
            var entry = JsonConvert.DeserializeObject<List<PasswordDetails>>(response).First();
            return GetPersonalPassword(entry.Id);
        }

        public int CreatePersonalPassword(string name, string password)
        {
            var body = JsonConvert.SerializeObject(new
            {
                name = name,
                password = password
            });

            var response = Post("api/v4/my_passwords.json", body);
            var created = JsonConvert.DeserializeObject<Created>(response);
            return Int32.Parse(created.Id);
        }

        public void UpdatePersonalPassword(PasswordDetails details)
        {
            var body = JsonConvert.SerializeObject(details);
            Put($"api/v4/my_passwords/{details.Id}.json", body);
        }

        public void DeletePersonalPassword(int personalPasswordId)
        {
            Delete($"api/v4/my_passwords/{personalPasswordId}.json");
        }
    }
}
