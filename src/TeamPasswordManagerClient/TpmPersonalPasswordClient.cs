using TeamPasswordManagerClient.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace TeamPasswordManagerClient
{
    public interface ITpmPersonalPasswordClient
    {
        Task<int> CreatePersonalPassword(string name, string password);
        Task DeletePersonalPassword(int personalPasswordId);
        Task<PasswordDetails> GetPersonalPassword(int personalPasswordId);
        Task<PasswordDetails> GetPersonalPassword(string passwordName);
        Task<IEnumerable<PasswordEntry>> ListAllPersonalPasswords(int pageSize = 20);
        Task<IEnumerable<PasswordEntry>> ListPersonalPasswords(int page = 1);
        Task UpdatePersonalPassword(PasswordDetails details);
    }

    internal class TpmPersonalPasswordClient : TpmBase, ITpmPersonalPasswordClient
    {
        public TpmPersonalPasswordClient(TpmConfig config) : base(config)
        {
        }

        public async Task<IEnumerable<PasswordEntry>> ListAllPersonalPasswords(int pageSize = 20)
        {
            return await FetchAllPages(ListPersonalPasswords, pageSize);
        }

        public async Task<IEnumerable<PasswordEntry>> ListPersonalPasswords(int page = 1)
        {
            var response = (page == 1) ? await Get($"api/v4/my_passwords.json") : await Get($"api/v4/my_passwords/page/{page}.json");
            return JsonConvert.DeserializeObject<List<PasswordEntry>>(response);
        }

        public async Task<PasswordDetails> GetPersonalPassword(int personalPasswordId)
        {
            var response = await Get($"api/v4/my_passwords/{personalPasswordId}.json");
            return JsonConvert.DeserializeObject<PasswordDetails>(response);
        }

        public async Task<PasswordDetails> GetPersonalPassword(string passwordName)
        {
            var urlencodedPassword = HttpUtility.UrlEncode($"[{passwordName}]");
            var response = await Get($"api/v4/my_passwords/search/name:{urlencodedPassword}.json");
            var entry = JsonConvert.DeserializeObject<List<PasswordDetails>>(response).First();
            return await GetPersonalPassword(entry.Id);
        }

        public async Task<int> CreatePersonalPassword(string name, string password)
        {
            var body = JsonConvert.SerializeObject(new
            {
                name = name,
                password = password
            });

            var response = await Post("api/v4/my_passwords.json", body);
            var created = JsonConvert.DeserializeObject<Created>(response);
            return Int32.Parse(created.Id);
        }

        public async Task UpdatePersonalPassword(PasswordDetails details)
        {
            var body = JsonConvert.SerializeObject(details);
            await Put($"api/v4/my_passwords/{details.Id}.json", body);
        }

        public async Task DeletePersonalPassword(int personalPasswordId)
        {
            await Delete($"api/v4/my_passwords/{personalPasswordId}.json");
        }
    }
}
