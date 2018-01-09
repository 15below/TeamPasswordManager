using TeamPasswordManagerClient.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TeamPasswordManagerClient
{
    public interface ITpmProjectClient
    {
        Task ArchiveProject(int projectId);
        Task ChangeProjectParent(int projectId, int newParentId);
        Task<int> CreateProject(string name, int parentId = 0, string tags = null, string notes = null);
        Task DeleteProject(int projectId);
        Task<ProjectDetails> GetProject(int projectId);
        Task<IEnumerable<ProjectEntry>> ListAllProjects(int pageSize = 20);
        Task<IEnumerable<ProjectEntry>> ListAllSubProjects(int projectId);
        Task<IEnumerable<ProjectEntry>> ListProjects(int page = 1);
        Task UnarchiveProject(int projectId);
        Task UpdateProject(ProjectDetails details);
    }

    internal class TpmProjectClient : TpmBase, ITpmProjectClient
    {
        public TpmProjectClient(TpmConfig config) : base(config)
        {
        }

        public async Task<IEnumerable<ProjectEntry>> ListAllProjects(int pageSize = 20)
        {
            return await FetchAllPages(ListProjects, pageSize);
        }

        public async Task<IEnumerable<ProjectEntry>> ListProjects(int page = 1)
        {
            var response = (page == 1) ? await Get("api/v4/projects.json") : await Get($"api/v4/projects/page/{page}.json");
            return JsonConvert.DeserializeObject<List<ProjectEntry>>(response);
        }

        public async Task<IEnumerable<ProjectEntry>> ListAllSubProjects(int projectId)
        {
            var response = await Get($"api/v4/projects/{projectId}/subprojects.json");
            return JsonConvert.DeserializeObject<List<ProjectEntry>>(response);
        }

        public async Task<ProjectDetails> GetProject(int projectId)
        {
            var response = await Get($"api/v4/projects/{projectId}.json");
            return JsonConvert.DeserializeObject<ProjectDetails>(response);
        }

        public async Task<int> CreateProject(string name, int parentId = 0, string tags = null, string notes = null)
        {
            var body = JsonConvert.SerializeObject(new
            {
                name = name,
                parent_id = parentId,
                tags = tags,
                notes = notes
            });

            var response = await Post("api/v4/projects.json", body);
            var created = JsonConvert.DeserializeObject<Created>(response);
            return Int32.Parse(created.Id);
        }

        public async Task UpdateProject(ProjectDetails details)
        {
            var body = JsonConvert.SerializeObject(new
            {
                name = details.Name,
                tags = details.Tags,
                notes = details.Notes
            });

            await Put($"api/v4/projects/{details.Id}.json", body);
        }

        public async Task ChangeProjectParent(int projectId, int newParentId)
        {
            var body = JsonConvert.SerializeObject(new
            {
                parent_id = newParentId
            });

            await Put($"api/v4/projects/{projectId}/change_parent.json", body);
        }

        public async Task ArchiveProject(int projectId)
        {
            await Put($"api/v4/projects/{projectId}/archive.json");
        }

        public async Task UnarchiveProject(int projectId)
        {
            await Put($"api/v4/projects/{projectId}/unarchive.json");
        }

        public async Task DeleteProject(int projectId)
        {
            await Delete($"api/v4/projects/{projectId}.json");
        }
    }
}
