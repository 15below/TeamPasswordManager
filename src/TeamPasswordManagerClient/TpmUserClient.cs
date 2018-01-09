using TeamPasswordManagerClient.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TeamPasswordManagerClient
{
    public interface ITpmUserClient
    {
        Task ActivateUser(int userId);
        Task ConvertToLdap(int userId, string loginDN);
        Task ConvertToNormal(int userId);
        Task<int> CreateLdapUser(CreateLdapUserRequest request);
        Task<int> CreateNormalUser(CreateNormalUserRequest request);
        Task DeactivateUser(int userId);
        Task DeleteUser(int userId);
        Task<UserDetails> GetUser(int userId);
        Task<IEnumerable<UserEntry>> ListAllUsers(int pageSize = 20);
        Task<IEnumerable<UserEntry>> ListUsers(int page = 1);
        Task UpdatePassword(int userId, string password);
        Task UpdateUser(int userId, UpdateUserRequest request);
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
