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
        /// <summary>
        /// Create a new password under the given project. If you want a securely generated password, <see cref="GeneratePassword"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="projectId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<int> CreatePassword(string name, int projectId, string password);

        /// <summary>
        /// This deletes the password, its files, etc.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeletePassword(int id);

        /// <summary>
        /// Generate a secure password, that conforms to the required strength settings in TPM.
        /// </summary>
        /// <returns></returns>
        Task<string> GeneratePassword();

        /// <summary>
        /// Get the password details including the password value itself.
        /// </summary>
        /// <param name="passwordId"></param>
        /// <returns></returns>
        Task<PasswordDetails> GetPassword(int passwordId);

        /// <summary>
        /// Search for a password entry by name.
        /// </summary>
        /// <param name="projectName">NOTE: Spaces in a project name will fail to return any results</param>
        /// <param name="passwordName"></param>
        /// <returns></returns>
        Task<PasswordEntry> SearchPassword(string projectName, string passwordName);

        /// <summary>
        /// Get all passwords within a project.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="pageSize">The amount of passwords that TPM is configured to return (defaults to 20). http://teampasswordmanager.com/docs/api/#pagination</param>
        /// <returns></returns>
        Task<IEnumerable<PasswordEntry>> ListAllPasswords(int projectId, int pageSize = 20);

        /// <summary>
        /// Get a page of password entries for the given project.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="page">Page number starting from 1</param>
        /// <returns></returns>
        Task<IEnumerable<PasswordEntry>> ListPasswords(int projectId, int page = 1);

        /// <summary>
        /// This sets the locking status of a password to locked. From this moment on, any user who wants to use it will have to supply a reason to unlock it.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task LockPassword(int id);

        /// <summary>
        /// This sets the locking status of a password to unlocked.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        Task UnlockPassword(int id, string reason);

        /// <summary>
        /// Updates the password with the supplied details. Only non-null values will be applied.
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
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
