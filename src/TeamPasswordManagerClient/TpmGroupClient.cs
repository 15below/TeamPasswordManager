using TeamPasswordManagerClient.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TeamPasswordManagerClient
{
    public interface ITpmGroupClient
    {
        Task AddUserToGroup(int groupId, int userId);
        Task<int> CreateGroup(string name);
        Task DeleteGroup(int groupId);
        Task<GroupDetails> GetGroup(int groupId);
        Task<IEnumerable<Group>> ListAllGroups(int pageSize = 20);
        Task<IEnumerable<Group>> ListGroups(int page = 1);
        Task RemoveUserFromGroup(int groupId, int userId);
        Task UpdateGroup(int groupId, string name);
    }

    internal class TpmGroupClient : TpmBase, ITpmGroupClient
    {
        public TpmGroupClient(TpmConfig config) : base(config)
        {
        }

        public async Task<IEnumerable<Group>> ListAllGroups(int pageSize = 20)
        {
            return await FetchAllPages(ListGroups, pageSize);
        }

        public async Task<IEnumerable<Group>> ListGroups(int page = 1)
        {
            var response = (page == 1) ? await Get("api/v4/groups.json") : await Get($"api/v4/groups/page/{page}.json");
            return JsonConvert.DeserializeObject<List<Group>>(response);
        }

        public async Task<GroupDetails> GetGroup(int groupId)
        {
            var response = await Get($"api/v4/groups/{groupId}.json");
            return JsonConvert.DeserializeObject<GroupDetails>(response);
        }

        public async Task<int> CreateGroup(string name)
        {
            var body = JsonConvert.SerializeObject(new
            {
                name = name
            });

            var response = await Post("api/v4/groups.json", body);
            var created = JsonConvert.DeserializeObject<Created>(response);
            return Int32.Parse(created.Id);
        }

        public async Task UpdateGroup(int groupId, string name)
        {
            var body = JsonConvert.SerializeObject(new
            {
                name = name
            });

            await Put($"api/v4/groups/{groupId}.json", body);
        }

        public async Task AddUserToGroup(int groupId, int userId)
        {
            await Put($"api/v4/groups/{groupId}/add_user/{userId}.json");
        }

        public async Task RemoveUserFromGroup(int groupId, int userId)
        {
            await Put($"api/v4/groups/{groupId}/delete_user/{userId}.json");
        }

        public async Task DeleteGroup(int groupId)
        {
            await Delete($"api/v4/groups/{groupId}.json");
        }
    }
}
