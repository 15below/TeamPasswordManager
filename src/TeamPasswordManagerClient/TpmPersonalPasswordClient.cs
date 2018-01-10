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
        /// <summary>
        /// Create a new personal password.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<int> CreatePersonalPassword(PersonalPassword details);

        /// <summary>
        /// Delete personal password.
        /// </summary>
        /// <param name="personalPasswordId"></param>
        /// <returns></returns>
        Task DeletePersonalPassword(int personalPasswordId);

        /// <summary>
        /// Get personal password details.
        /// </summary>
        /// <param name="personalPasswordId"></param>
        /// <returns></returns>
        Task<PersonalPassword> GetPersonalPassword(int personalPasswordId);

        /// <summary>
        /// Get personal password details by name.
        /// </summary>
        /// <param name="passwordName"></param>
        /// <returns></returns>
        Task<PersonalPassword> GetPersonalPassword(string passwordName);

        /// <summary>
        /// List all personal passwords.
        /// </summary>
        /// <param name="pageSize">The amount of passwords that TPM is configured to return (defaults to 20). http://teampasswordmanager.com/docs/api/#pagination</param>
        /// <returns></returns>
        Task<IEnumerable<PersonalPasswordEntry>> ListAllPersonalPasswords(int pageSize = 20);

        /// <summary>
        /// Get a page of personal password entries.
        /// </summary>
        /// <param name="page">Page number starting from 1</param>
        /// <returns></returns>
        Task<IEnumerable<PersonalPasswordEntry>> ListPersonalPasswords(int page = 1);

        /// <summary>
        /// Updates the personal password with the supplied details. Only non-null values will be applied.
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        Task UpdatePersonalPassword(PersonalPassword details);
    }

    internal class TpmPersonalPasswordClient : TpmBase, ITpmPersonalPasswordClient
    {
        public TpmPersonalPasswordClient(TpmConfig config) : base(config)
        {
        }

        public async Task<IEnumerable<PersonalPasswordEntry>> ListAllPersonalPasswords(int pageSize = 20)
        {
            return await FetchAllPages(ListPersonalPasswords, pageSize);
        }

        public async Task<IEnumerable<PersonalPasswordEntry>> ListPersonalPasswords(int page = 1)
        {
            var response = (page == 1) ? await Get($"api/v4/my_passwords.json") : await Get($"api/v4/my_passwords/page/{page}.json");
            return JsonConvert.DeserializeObject<List<PersonalPasswordEntry>>(response);
        }

        public async Task<PersonalPassword> GetPersonalPassword(int personalPasswordId)
        {
            var response = await Get($"api/v4/my_passwords/{personalPasswordId}.json");
            return JsonConvert.DeserializeObject<PersonalPassword>(response);
        }

        public async Task<PersonalPassword> GetPersonalPassword(string passwordName)
        {
            var urlencodedPassword = HttpUtility.UrlEncode($"[{passwordName}]");
            var response = await Get($"api/v4/my_passwords/search/name:{urlencodedPassword}.json");
            var entry = JsonConvert.DeserializeObject<List<PersonalPassword>>(response).First();
            return await GetPersonalPassword(entry.Id);
        }

        public async Task<int> CreatePersonalPassword(PersonalPassword details)
        {
            var body = JsonConvert.SerializeObject(new
            {
                name = details.Name,
                tags = details.Tags,
                access_info = details.Access_Info,
                username = details.Username,
                email = details.Email,
                password = details.Password,
                notes = details.Notes
            });

            var response = await Post("api/v4/my_passwords.json", body);
            var created = JsonConvert.DeserializeObject<Created>(response);
            return Int32.Parse(created.Id);
        }

        public async Task UpdatePersonalPassword(PersonalPassword details)
        {
            var body = JsonConvert.SerializeObject(new
            {
                name = details.Name,
                tags = details.Tags,
                access_info = details.Access_Info,
                username = details.Username,
                email = details.Email,
                password = details.Password,
                notes = details.Notes
            });

            await Put($"api/v4/my_passwords/{details.Id}.json", body);
        }

        public async Task DeletePersonalPassword(int personalPasswordId)
        {
            await Delete($"api/v4/my_passwords/{personalPasswordId}.json");
        }
    }
}
