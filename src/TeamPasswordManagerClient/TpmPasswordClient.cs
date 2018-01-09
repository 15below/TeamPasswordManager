using TeamPasswordManagerClient.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamPasswordManagerClient
{
    public interface ITpmPasswordClient
    {
        int CreatePassword(string name, int projectId, string password);
        void DeletePassword(int id);
        string GeneratePassword();
        PasswordDetails GetPassword(int passwordId);
        PasswordDetails GetPassword(string projectName, string passwordName);
        List<PasswordEntry> ListAllPasswords(int projectId);
        List<PasswordEntry> ListPasswords(int projectId, int page = 1);
        void LockPassword(int id);
        void UnlockPassword(int id, string reason);
        void UpdatePassword(PasswordDetails details);
    }

    public class TpmPasswordClient : TpmBase, ITpmPasswordClient
    {
        public TpmPasswordClient(TpmConfig config) : base(config)
        {
        }

        public string GeneratePassword()
        {
            var response = Get($"api/v4/generate_password.json");
            return JsonConvert.DeserializeObject<GeneratedPassword>(response).Password;
        }

        public List<PasswordEntry> ListAllPasswords(int projectId)
        {
            return FetchAllPages((page) => ListPasswords(projectId, page), 20);
        }

        public List<PasswordEntry> ListPasswords(int projectId, int page = 1)
        {
            var response = (page == 1) ? Get($"api/v4/projects/{projectId}/passwords.json") : Get($"api/v4/projects/{projectId}/passwords/page/{page}.json");
            return JsonConvert.DeserializeObject<List<PasswordEntry>>(response);
        }

        public PasswordDetails GetPassword(int passwordId)
        {
            var response = Get($"api/v4/passwords/{passwordId}.json");
            return JsonConvert.DeserializeObject<PasswordDetails>(response);
        }

        public PasswordDetails GetPassword(string projectName, string passwordName)
        {
            var urlencodedProject = HttpUtility.UrlEncode($"[{projectName}]");
            var urlencodedPassword = HttpUtility.UrlEncode($"[{passwordName}]");
            var response = Get($"api/v4/passwords/search/in:{urlencodedProject}+name:{urlencodedPassword}.json");
            var entry = JsonConvert.DeserializeObject<List<PasswordEntry>>(response).First();
            return GetPassword(entry.Id);
        }

        public int CreatePassword(string name, int projectId, string password)
        {
            var body = JsonConvert.SerializeObject(new
            {
                name = name,
                project_id = projectId,
                password = password
            });

            var response = Post("api/v4/passwords.json", body);
            var created = JsonConvert.DeserializeObject<Created>(response);
            return Int32.Parse(created.Id);
        }

        public void UpdatePassword(PasswordDetails details)
        {
            var body = JsonConvert.SerializeObject(details);
            Put($"api/v4/passwords/{details.Id}.json", body);
        }

        public void LockPassword(int id)
        {
            Put($"api/v4/passwords/{id}/lock.json");
        }

        public void UnlockPassword(int id, string reason)
        {
            var request = BuildRequest("PUT", $"api/v4/passwords/{id}/unlock.json");
            request.Headers.Add("X-Unlock-Reason", reason);
            ReadResponse(request);
        }

        public void DeletePassword(int id)
        {
            Delete($"api/v4/passwords/{id}.json");
        }
    }
}
