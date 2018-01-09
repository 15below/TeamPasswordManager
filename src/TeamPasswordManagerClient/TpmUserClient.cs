using TeamPasswordManagerClient.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TeamPasswordManagerClient
{
    public interface ITpmUserClient
    {
        void ActivateUser(int userId);
        void ConvertToLdap(int userId, string loginDN);
        void ConvertToNormal(int userId);
        int CreateLdapUser(CreateLdapUserRequest request);
        int CreateNormalUser(CreateNormalUserRequest request);
        void DeactivateUser(int userId);
        void DeleteUser(int userId);
        UserDetails GetUser(int userId);
        List<UserEntry> ListAllUsers();
        List<UserEntry> ListUsers(int page = 1);
        void UpdatePassword(int userId, string password);
        void UpdateUser(int userId, UpdateUserRequest request);
        UserDetails WhoAmI();
    }

    public class TpmUserClient : TpmBase, ITpmUserClient
    {
        public TpmUserClient(TpmConfig config) : base(config)
        {
        }

        public List<UserEntry> ListAllUsers()
        {
            return FetchAllPages((page) => ListUsers(page), 20);
        }

        public List<UserEntry> ListUsers(int page = 1)
        {
            var response = (page == 1) ? Get("api/v4/users.json") : Get($"api/v4/users/page/{page}.json");
            return JsonConvert.DeserializeObject<List<UserEntry>>(response);
        }

        public UserDetails GetUser(int userId)
        {
            var response = Get($"api/v4/users/{userId}.json");
            return JsonConvert.DeserializeObject<UserDetails>(response);
        }

        public UserDetails WhoAmI()
        {
            var response = Get($"api/v4/users/me.json");
            return JsonConvert.DeserializeObject<UserDetails>(response);
        }

        public int CreateLdapUser(CreateLdapUserRequest request)
        {
            var body = JsonConvert.SerializeObject(new
            {
                username = request.Username,
                email_address = request.EmailAddress,
                name = request.Name,
                role = request.Role,
                login_dn = request.LoginDN
            });

            var response = Post("api/v4/users.json", body);
            var created = JsonConvert.DeserializeObject<Created>(response);
            return Int32.Parse(created.Id);
        }

        public int CreateNormalUser(CreateNormalUserRequest request)
        {
            var body = JsonConvert.SerializeObject(new
            {
                username = request.Username,
                email_address = request.EmailAddress,
                name = request.Name,
                role = request.Role,
                password = request.Password
            });

            var response = Post("api/v4/users.json", body);
            var created = JsonConvert.DeserializeObject<Created>(response);
            return Int32.Parse(created.Id);
        }

        public void UpdateUser(int userId, UpdateUserRequest request)
        {
            var body = JsonConvert.SerializeObject(new
            {
                username = request.Username,
                email_address = request.EmailAddress,
                name = request.Name,
                role = request.Role,
                login_dn = request.LoginDN
            });

            Put($"api/v4/users/{userId}.json", body);
        }

        public void UpdatePassword(int userId, string password)
        {
            var body = JsonConvert.SerializeObject(new
            {
                password = password
            });

            Put($"api/v4/users/{userId}/change_password.json", body);
        }

        public void ActivateUser(int userId)
        {
            Put($"api/v4/users/{userId}/activate.json");
        }

        public void DeactivateUser(int userId)
        {
            Put($"api/v4/users/{userId}/deactivate.json");
        }

        public void ConvertToLdap(int userId, string loginDN)
        {
            var body = JsonConvert.SerializeObject(new
            {
                login_dn = loginDN
            });

            Put($"api/v4/users/{userId}/convert_to_ldap.json", body);
        }

        public void ConvertToNormal(int userId)
        {
            Put($"api/v4/users/{userId}/convert_to_normal.json");
        }

        public void DeleteUser(int userId)
        {
            Delete($"api/v4/users/{userId}.json");
        }
    }
}
