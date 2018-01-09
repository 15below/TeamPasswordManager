using TeamPasswordManagerClient.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TeamPasswordManagerClient
{
    public interface ITpmGroupClient
    {
        void AddUserToGroup(int groupId, int userId);
        int CreateGroup(string name);
        void DeleteGroup(int groupId);
        GroupDetails GetGroup(int groupId);
        List<Group> ListAllGroups();
        List<Group> ListGroups(int page = 1);
        void RemoveUserFromGroup(int groupId, int userId);
        void UpdateGroup(int groupId, string name);
    }

    public class TpmGroupClient : TpmBase, ITpmGroupClient
    {
        public TpmGroupClient(TpmConfig config) : base(config)
        {
        }

        public List<Group> ListAllGroups()
        {
            return FetchAllPages((page) => ListGroups(page), 20);
        }

        public List<Group> ListGroups(int page = 1)
        {
            var response = (page == 1) ? Get("api/v4/groups.json") : Get($"api/v4/groups/page/{page}.json");
            return JsonConvert.DeserializeObject<List<Group>>(response);
        }

        public GroupDetails GetGroup(int groupId)
        {
            var response = Get($"api/v4/groups/{groupId}.json");
            return JsonConvert.DeserializeObject<GroupDetails>(response);
        }

        public int CreateGroup(string name)
        {
            var body = JsonConvert.SerializeObject(new
            {
                name = name
            });

            var response = Post("api/v4/groups.json", body);
            var created = JsonConvert.DeserializeObject<Created>(response);
            return Int32.Parse(created.Id);
        }

        public void UpdateGroup(int groupId, string name)
        {
            var body = JsonConvert.SerializeObject(new
            {
                name = name
            });

            Put($"api/v4/groups/{groupId}.json", body);
        }

        public void AddUserToGroup(int groupId, int userId)
        {
            Put($"api/v4/groups/{groupId}/add_user/{userId}.json");
        }

        public void RemoveUserFromGroup(int groupId, int userId)
        {
            Put($"api/v4/groups/{groupId}/delete_user/{userId}.json");
        }

        public void DeleteGroup(int groupId)
        {
            Delete($"api/v4/groups/{groupId}.json");
        }
    }
}
