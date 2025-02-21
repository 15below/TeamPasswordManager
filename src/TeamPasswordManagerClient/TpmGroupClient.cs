using TeamPasswordManagerClient.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TeamPasswordManagerClient
{
    public interface ITpmGroupClient
    {
        /// <summary>
        /// Add a user to the group.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task AddUserToGroup(int id, int userId);

        /// <summary>
        /// Create a new group. Will return a WebException (409) Conflict if there is an existing group with that name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<int> CreateGroup(string name);

        /// <summary>
        /// Delete a group.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteGroup(int id);

        /// <summary>
        /// Get group details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<GroupDetails> GetGroup(int id);

        /// <summary>
        /// List all groups.
        /// </summary>
        /// <param name="pageSize">The amount of passwords that TPM is configured to return (defaults to 20). http://teampasswordmanager.com/docs/api/#pagination</param>
        /// <returns></returns>
        Task<IEnumerable<Group>> ListAllGroups(int pageSize = 20);

        /// <summary>
        /// Get a page of groups.
        /// </summary>
        /// <param name="page">Page number starting from 1</param>
        /// <returns></returns>
        Task<IEnumerable<Group>> ListGroups(int page = 1);

        /// <summary>
        /// Remove a user from the group.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task RemoveUserFromGroup(int id, int userId);

        /// <summary>
        /// Update a group name.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Task UpdateGroup(int id, string name);
    }

    internal class TpmGroupClient : ITpmGroupClient
    {
        private readonly TpmHttp http;

        public TpmGroupClient(TpmHttp http)
        {
            this.http = http;
        }

        public async Task<IEnumerable<Group>> ListAllGroups(int pageSize = 20)
        {
            return await http.FetchAllPages(ListGroups, pageSize);
        }

        public async Task<IEnumerable<Group>> ListGroups(int page = 1)
        {
            var response = (page == 1) ? await http.Get("groups.json") : await http.Get($"groups/page/{page}.json");
            return Json.ToObject<List<Group>>(response);
        }

        public async Task<GroupDetails> GetGroup(int id)
        {
            var response = await http.Get($"groups/{id}.json");
            return Json.ToObject<GroupDetails>(response);
        }

        public async Task<int> CreateGroup(string name)
        {
            var body = Json.ToString(new CreateGroupRequest
            {
                Name = name
            });

            var response = await http.Post("groups.json", body);
            var created = Json.ToObject<Created>(response);
            return Int32.Parse(created.Id);
        }

        public async Task UpdateGroup(int id, string name)
        {
            var body = Json.ToString(new UpdateGroupRequest
            {
                Name = name
            });

            await http.Put($"groups/{id}.json", body);
        }

        public async Task AddUserToGroup(int id, int userId)
        {
            await http.Put($"groups/{id}/add_user/{userId}.json");
        }

        public async Task RemoveUserFromGroup(int id, int userId)
        {
            await http.Put($"groups/{id}/delete_user/{userId}.json");
        }

        public async Task DeleteGroup(int id)
        {
            await http.Delete($"groups/{id}.json");
        }
    }
}
