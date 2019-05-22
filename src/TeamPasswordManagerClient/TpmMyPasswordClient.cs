using TeamPasswordManagerClient.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace TeamPasswordManagerClient
{
    public interface ITpmMyPasswordClient
    {
        /// <summary>
        /// Create a new personal password.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<int> CreatePassword(CreateMyPasswordRequest request);

        /// <summary>
        /// Delete personal password.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeletePassword(int id);

        /// <summary>
        /// Get personal password details.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MyPasswordDetails> GetPassword(int id);

        /// <summary>
        /// Get personal password details by name.
        /// </summary>
        /// <param name="passwordName"></param>
        /// <returns></returns>
        Task<MyPasswordEntry> SearchPassword(string passwordName);

        /// <summary>
        /// List all personal passwords.
        /// </summary>
        /// <param name="pageSize">The amount of passwords that TPM is configured to return (defaults to 20). http://teampasswordmanager.com/docs/api/#pagination</param>
        /// <returns></returns>
        Task<IEnumerable<MyPasswordEntry>> ListAllPasswords(int pageSize = 20);

        /// <summary>
        /// Get a page of personal password entries.
        /// </summary>
        /// <param name="page">Page number starting from 1</param>
        /// <returns></returns>
        Task<IEnumerable<MyPasswordEntry>> ListPasswords(int page = 1);

        /// <summary>
        /// Updates the personal password with the supplied details. Only non-null values will be applied.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task UpdatePassword(int id, UpdateMyPasswordRequest request);
    }

    internal class TpmMyPasswordClient : ITpmMyPasswordClient
    {
        private readonly TpmHttp http;

        public TpmMyPasswordClient(TpmHttp http)
        {
            this.http = http;
        }

        public async Task<IEnumerable<MyPasswordEntry>> ListAllPasswords(int pageSize = 20)
        {
            return await http.FetchAllPages(ListPasswords, pageSize);
        }

        public async Task<IEnumerable<MyPasswordEntry>> ListPasswords(int page = 1)
        {
            var response = (page == 1) ? await http.Get($"api/v4/my_passwords.json") : await http.Get($"api/v4/my_passwords/page/{page}.json");
            return Json.ToObject<List<MyPasswordEntry>>(response);
        }

        public async Task<MyPasswordDetails> GetPassword(int id)
        {
            var response = await http.Get($"api/v4/my_passwords/{id}.json");
            return Json.ToObject<MyPasswordDetails>(response);
        }

        public async Task<MyPasswordEntry> SearchPassword(string passwordName)
        {
            var urlencodedPassword = HttpUtility.UrlEncode($"[{passwordName}]");
            var response = await http.Get($"api/v4/my_passwords/search/name:{urlencodedPassword}.json");
            var entry = Json.ToObject<List<MyPasswordEntry>>(response).FirstOrDefault();
            if (entry == null)
            {
                throw new Exception("Response did not contain a match");
            }
            return entry;
        }

        public async Task<int> CreatePassword(CreateMyPasswordRequest request)
        {
            var body = Json.ToString(request);
            var response = await http.Post("api/v4/my_passwords.json", body);
            var created = Json.ToObject<Created>(response);
            return Int32.Parse(created.Id);
        }

        public async Task UpdatePassword(int id, UpdateMyPasswordRequest request)
        {
            var body = Json.ToString(request);
            await http.Put($"api/v4/my_passwords/{id}.json", body);
        }

        public async Task DeletePassword(int id)
        {
            await http.Delete($"api/v4/my_passwords/{id}.json");
        }
    }
}
