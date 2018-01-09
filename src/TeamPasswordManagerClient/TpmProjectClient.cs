using TeamPasswordManagerClient.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TeamPasswordManagerClient
{
    public interface ITpmProjectClient
    {
        void ArchiveProject(int projectId);
        void ChangeProjectParent(int projectId, int newParentId);
        int CreateProject(string name, int parentId = 0, string tags = null, string notes = null);
        void DeleteProject(int projectId);
        ProjectDetails GetProject(int projectId);
        List<ProjectEntry> ListAllProjects();
        List<ProjectEntry> ListAllSubProjects(int projectId);
        List<ProjectEntry> ListProjects(int page = 1);
        void UnarchiveProject(int projectId);
        void UpdateProject(ProjectDetails details);
    }

    public class TpmProjectClient : TpmBase, ITpmProjectClient
    {
        public TpmProjectClient(TpmConfig config) : base(config)
        {
        }

        public List<ProjectEntry> ListAllProjects()
        {
            return FetchAllPages((page) => ListProjects(page), 20);
        }

        public List<ProjectEntry> ListProjects(int page = 1)
        {
            var response = (page == 1) ? Get("api/v4/projects.json") : Get($"api/v4/projects/page/{page}.json");
            return JsonConvert.DeserializeObject<List<ProjectEntry>>(response);
        }

        public List<ProjectEntry> ListAllSubProjects(int projectId)
        {
            var response = Get($"api/v4/projects/{projectId}/subprojects.json");
            return JsonConvert.DeserializeObject<List<ProjectEntry>>(response);
        }

        public ProjectDetails GetProject(int projectId)
        {
            var response = Get($"api/v4/projects/{projectId}.json");
            return JsonConvert.DeserializeObject<ProjectDetails>(response);
        }

        public int CreateProject(string name, int parentId = 0, string tags = null, string notes = null)
        {
            var body = JsonConvert.SerializeObject(new
            {
                name = name,
                parent_id = parentId,
                tags = tags,
                notes = notes
            });

            var response = Post("api/v4/projects.json", body);
            var created = JsonConvert.DeserializeObject<Created>(response);
            return Int32.Parse(created.Id);
        }

        public void UpdateProject(ProjectDetails details)
        {
            var body = JsonConvert.SerializeObject(new
            {
                name = details.Name,
                tags = details.Tags,
                notes = details.Notes
            });

            Put($"api/v4/projects/{details.Id}.json", body);
        }

        public void ChangeProjectParent(int projectId, int newParentId)
        {
            var body = JsonConvert.SerializeObject(new
            {
                parent_id = newParentId
            });

            Put($"api/v4/projects/{projectId}/change_parent.json", body);
        }

        public void ArchiveProject(int projectId)
        {
            Put($"api/v4/projects/{projectId}/archive.json");
        }

        public void UnarchiveProject(int projectId)
        {
            Put($"api/v4/projects/{projectId}/unarchive.json");
        }

        public void DeleteProject(int projectId)
        {
            Delete($"api/v4/projects/{projectId}.json");
        }
    }
}
