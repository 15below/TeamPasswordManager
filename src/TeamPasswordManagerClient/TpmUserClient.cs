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
        /// <param name="userId"></param>
        /// <returns></returns>
        Task ActivateUser(int userId);

        /// <summary>
        /// Convert a normal user to an Active Directory login.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loginDN"></param>
        /// <returns></returns>
        Task ConvertToLdap(int userId, string loginDN);

        /// <summary>
        /// Convert an Active Directory user to a normal one.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task ConvertToNormal(int userId);

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
        /// <param name="userId"></param>
        /// <returns></returns>
        Task DeactivateUser(int userId);

        /// <summary>
        /// Delete a user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task DeleteUser(int userId);

        /// <summary>
        /// Get user details.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<UserDetails> GetUser(int userId);

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
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task UpdatePassword(int userId, string password);

        /// <summary>
        /// Update a users' details (excluding password changes, <see cref="UpdatePassword(int, string)"/>).
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task UpdateUser(int userId, UpdateUserRequest request);

        /// <summary>
        /// Return the current users' details.
        /// </summary>
        /// <returns></returns>
        Task<UserDetails> WhoAmI();
    }

    internal class TpmUserClient : TpmBase, ITpmUserClient
    {
        public TpmUserClient(TpmConfig config) : base(config)
        {
        }

        public async Task<IEnumerable<UserEntry>> ListAllUsers(int pageSize = 20)
        {
            return await FetchAllPages(ListUsers, pageSize);
        }

        public async Task<IEnumerable<UserEntry>> ListUsers(int page = 1)
        {
            var response = (page == 1) ? await Get("api/v4/users.json") : await Get($"api/v4/users/page/{page}.json");
            return JsonConvert.DeserializeObject<List<UserEntry>>(response);
        }

        public async Task<UserDetails> GetUser(int userId)
        {
            var response = await Get($"api/v4/users/{userId}.json");
            return JsonConvert.DeserializeObject<UserDetails>(response);
        }

        public async Task<UserDetails> WhoAmI()
        {
            var response = await Get($"api/v4/users/me.json");
            return JsonConvert.DeserializeObject<UserDetails>(response);
        }

        public async Task<int> CreateLdapUser(CreateLdapUserRequest request)
        {
            var body = JsonConvert.SerializeObject(new
            {
                username = request.Username,
                email_address = request.EmailAddress,
                name = request.Name,
                role = request.Role,
                login_dn = request.LoginDN
            });

            var response = await Post("api/v4/users.json", body);
            var created = JsonConvert.DeserializeObject<Created>(response);
            return Int32.Parse(created.Id);
        }

        public async Task<int> CreateNormalUser(CreateNormalUserRequest request)
        {
            var body = JsonConvert.SerializeObject(new
            {
                username = request.Username,
                email_address = request.EmailAddress,
                name = request.Name,
                role = request.Role,
                password = request.Password
            });

            var response = await Post("api/v4/users.json", body);
            var created = JsonConvert.DeserializeObject<Created>(response);
            return Int32.Parse(created.Id);
        }

        public async Task UpdateUser(int userId, UpdateUserRequest request)
        {
            var body = JsonConvert.SerializeObject(new
            {
                username = request.Username,
                email_address = request.EmailAddress,
                name = request.Name,
                role = request.Role,
                login_dn = request.LoginDN
            });

            await Put($"api/v4/users/{userId}.json", body);
        }

        public async Task UpdatePassword(int userId, string password)
        {
            var body = JsonConvert.SerializeObject(new
            {
                password = password
            });

            await Put($"api/v4/users/{userId}/change_password.json", body);
        }

        public async Task ActivateUser(int userId)
        {
            await Put($"api/v4/users/{userId}/activate.json");
        }

        public async Task DeactivateUser(int userId)
        {
            await Put($"api/v4/users/{userId}/deactivate.json");
        }

        public async Task ConvertToLdap(int userId, string loginDN)
        {
            var body = JsonConvert.SerializeObject(new
            {
                login_dn = loginDN
            });

            await Put($"api/v4/users/{userId}/convert_to_ldap.json", body);
        }

        public async Task ConvertToNormal(int userId)
        {
            await Put($"api/v4/users/{userId}/convert_to_normal.json");
        }

        public async Task DeleteUser(int userId)
        {
            await Delete($"api/v4/users/{userId}.json");
        }
    }
}
