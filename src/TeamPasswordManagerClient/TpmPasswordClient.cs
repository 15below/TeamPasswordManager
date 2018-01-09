using TeamPasswordManagerClient.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace TeamPasswordManagerClient
{
    public interface ITpmPasswordClient
    {
        Task<int> CreatePassword(string name, int projectId, string password);
        Task DeletePassword(int id);
        Task<string> GeneratePassword();
        Task<PasswordDetails> GetPassword(int passwordId);
        Task<PasswordEntry> SearchPassword(string projectName, string passwordName);
        Task<IEnumerable<PasswordEntry>> ListAllPasswords(int projectId, int pageSize = 20);
        Task<IEnumerable<PasswordEntry>> ListPasswords(int projectId, int page = 1);
        Task LockPassword(int id);
        Task UnlockPassword(int id, string reason);
        Task UpdatePassword(PasswordDetails details);
    }

    internal class TpmPasswordClient : TpmBase, ITpmPasswordClient
    {
        public TpmPasswordClient(TpmConfig config) : base(config)
        {
        }

        public async Task<string> GeneratePassword()
        {
            var response = await Get($"api/v4/generate_password.json");
            return JsonConvert.DeserializeObject<GeneratedPassword>(response).Password;
        }

        public async Task<IEnumerable<PasswordEntry>> ListAllPasswords(int projectId, int pageSize = 20)
        {
            return await FetchAllPages(async (page) => await ListPasswords(projectId, page), pageSize);
        }

        public async Task<IEnumerable<PasswordEntry>> ListPasswords(int projectId, int page = 1)
        {
            var response = (page == 1) ? await Get($"api/v4/projects/{projectId}/passwords.json") : await Get($"api/v4/projects/{projectId}/passwords/page/{page}.json");
            return JsonConvert.DeserializeObject<List<PasswordEntry>>(response);
        }

        public async Task<PasswordDetails> GetPassword(int passwordId)
        {
            var response = await Get($"api/v4/passwords/{passwordId}.json");
            return JsonConvert.DeserializeObject<PasswordDetails>(response);
        }

        public async Task<PasswordEntry> SearchPassword(string projectName, string passwordName)
        {
            var urlencodedProject = HttpUtility.UrlEncode($"[{projectName}]");
            var urlencodedPassword = HttpUtility.UrlEncode($"[{passwordName}]");
            var response = await Get($"api/v4/passwords/search/in:{urlencodedProject}+name:{urlencodedPassword}.json");
            var entry = JsonConvert.DeserializeObject<List<PasswordEntry>>(response).FirstOrDefault();
            if (entry == null)
            {
                throw new Exception("Response did not contain a match");
            }
            return entry;
        }

        public async Task<int> CreatePassword(string name, int projectId, string password)
        {
            var body = JsonConvert.SerializeObject(new
            {
                name = name,
                project_id = projectId,
                password = password
            });

            var response = await Post("api/v4/passwords.json", body);
            var created = JsonConvert.DeserializeObject<Created>(response);
            return Int32.Parse(created.Id);
        }

        public async Task UpdatePassword(PasswordDetails details)
        {
            var body = JsonConvert.SerializeObject(details);
            await Put($"api/v4/passwords/{details.Id}.json", body);
        }

        public async Task LockPassword(int id)
        {
            await Put($"api/v4/passwords/{id}/lock.json");
        }

        public async Task UnlockPassword(int id, string reason)
        {
            var request = BuildRequest("PUT", $"api/v4/passwords/{id}/unlock.json");
            request.Headers.Add("X-Unlock-Reason", reason);
            await ReadResponse(request);
        }

        public async Task DeletePassword(int id)
        {
            await Delete($"api/v4/passwords/{id}.json");
        }
    }
}
