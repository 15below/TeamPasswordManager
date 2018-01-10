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
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task AddUserToGroup(int groupId, int userId);

        /// <summary>
        /// Create a new group. Will return a WebException (409) Conflict if there is an existing group with that name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<int> CreateGroup(string name);

        /// <summary>
        /// Delete a group.
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task DeleteGroup(int groupId);

        /// <summary>
        /// Get group details
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<GroupDetails> GetGroup(int groupId);

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
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task RemoveUserFromGroup(int groupId, int userId);

        /// <summary>
        /// Update a group name.
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Task UpdateGroup(int groupId, string name);
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
            var response = (page == 1) ? await http.Get("api/v4/groups.json") : await http.Get($"api/v4/groups/page/{page}.json");
            return JsonConvert.DeserializeObject<List<Group>>(response);
        }

        public async Task<GroupDetails> GetGroup(int groupId)
        {
            var response = await http.Get($"api/v4/groups/{groupId}.json");
            return JsonConvert.DeserializeObject<GroupDetails>(response);
        }

        public async Task<int> CreateGroup(string name)
        {
            var body = JsonConvert.SerializeObject(new CreateGroupRequest
            {
                Name = name
            });

            var response = await http.Post("api/v4/groups.json", body);
            var created = JsonConvert.DeserializeObject<Created>(response);
            return Int32.Parse(created.Id);
        }

        public async Task UpdateGroup(int groupId, string name)
        {
            var body = JsonConvert.SerializeObject(new UpdateGroupRequest
            {
                Name = name
            });

            await http.Put($"api/v4/groups/{groupId}.json", body);
        }

        public async Task AddUserToGroup(int groupId, int userId)
        {
            await http.Put($"api/v4/groups/{groupId}/add_user/{userId}.json");
        }

        public async Task RemoveUserFromGroup(int groupId, int userId)
        {
            await http.Put($"api/v4/groups/{groupId}/delete_user/{userId}.json");
        }

        public async Task DeleteGroup(int groupId)
        {
            await http.Delete($"api/v4/groups/{groupId}.json");
        }
    }
}
