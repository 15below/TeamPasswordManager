using TeamPasswordManagerClient.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TeamPasswordManagerClient
{
    public interface ITpmUserClient
    {
        /// <summary>
        /// Activate a deactivated user.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task ActivateUser(int id);

        /// <summary>
        /// Convert a normal user to an Active Directory login.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="loginDN"></param>
        /// <returns></returns>
        Task ConvertToLdap(int id, string loginDN);

        /// <summary>
        /// Convert an Active Directory user to a normal one.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task ConvertToNormal(int id);

        /// <summary>
        /// Create a new Active Directory user.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<int> CreateLdapUser(CreateLdapUserRequest request);

        /// <summary>
        /// Create a new normal user.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<int> CreateNormalUser(CreateNormalUserRequest request);

        /// <summary>
        /// Deactivate a user.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeactivateUser(int id);

        /// <summary>
        /// Delete a user.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteUser(int id);

        /// <summary>
        /// Get user details.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<UserDetails> GetUser(int id);

        /// <summary>
        /// List all users.
        /// </summary>
        /// <param name="pageSize">The amount of passwords that TPM is configured to return (defaults to 20). http://teampasswordmanager.com/docs/api/#pagination</param>
        /// <returns></returns>
        Task<IEnumerable<UserEntry>> ListAllUsers(int pageSize = 20);

        /// <summary>
        /// Get a page of users.
        /// </summary>
        /// <param name="page">Page number starting from 1</param>
        /// <returns></returns>
        Task<IEnumerable<UserEntry>> ListUsers(int page = 1);

        /// <summary>
        /// Update a normal users' password.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task UpdatePassword(int id, string password);

        /// <summary>
        /// Update a users' details (excluding password changes, <see cref="UpdatePassword(int, string)"/>).
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task UpdateUser(int id, UpdateUserRequest request);

        /// <summary>
        /// Return the current users' details.
        /// </summary>
        /// <returns></returns>
        Task<UserDetails> WhoAmI();
    }

    internal class TpmUserClient : ITpmUserClient
    {
        private readonly TpmHttp http;

        public TpmUserClient(TpmHttp http)
        {
            this.http = http;
        }

        public async Task<IEnumerable<UserEntry>> ListAllUsers(int pageSize = 20)
        {
            return await http.FetchAllPages(ListUsers, pageSize);
        }

        public async Task<IEnumerable<UserEntry>> ListUsers(int page = 1)
        {
            var response = (page == 1) ? await http.Get("api/v4/users.json") : await http.Get($"api/v4/users/page/{page}.json");
            return Json.ToObject<List<UserEntry>>(response);
        }

        public async Task<UserDetails> GetUser(int id)
        {
            var response = await http.Get($"api/v4/users/{id}.json");
            return Json.ToObject<UserDetails>(response);
        }

        public async Task<UserDetails> WhoAmI()
        {
            var response = await http.Get($"api/v4/users/me.json");
            return Json.ToObject<UserDetails>(response);
        }

        public async Task<int> CreateLdapUser(CreateLdapUserRequest request)
        {
            var body = Json.ToString(request);
            var response = await http.Post("api/v4/users.json", body);
            var created = Json.ToObject<Created>(response);
            return Int32.Parse(created.Id);
        }

        public async Task<int> CreateNormalUser(CreateNormalUserRequest request)
        {
            var body = Json.ToString(request);
            var response = await http.Post("api/v4/users.json", body);
            var created = Json.ToObject<Created>(response);
            return Int32.Parse(created.Id);
        }

        public async Task UpdateUser(int id, UpdateUserRequest request)
        {
            var body = Json.ToString(request);
            await http.Put($"api/v4/users/{id}.json", body);
        }

        public async Task UpdatePassword(int id, string password)
        {
            var body = Json.ToString(new
            {
                password = password
            });

            await http.Put($"api/v4/users/{id}/change_password.json", body);
        }

        public async Task ActivateUser(int id)
        {
            await http.Put($"api/v4/users/{id}/activate.json");
        }

        public async Task DeactivateUser(int id)
        {
            await http.Put($"api/v4/users/{id}/deactivate.json");
        }

        public async Task ConvertToLdap(int id, string loginDN)
        {
            var body = Json.ToString(new ConvertToLdapRequest
            {
                LoginDN = loginDN
            });

            await http.Put($"api/v4/users/{id}/convert_to_ldap.json", body);
        }

        public async Task ConvertToNormal(int id)
        {
            await http.Put($"api/v4/users/{id}/convert_to_normal.json");
        }

        public async Task DeleteUser(int id)
        {
            await http.Delete($"api/v4/users/{id}.json");
        }
    }
}
