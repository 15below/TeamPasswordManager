using TeamPasswordManagerClient.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Linq;

namespace TeamPasswordManagerClient
{
    public interface ITpmProjectClient
    {
        /// <summary>
        /// Archive a project.
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task ArchiveProject(int projectId);

        /// <summary>
        /// Move a project to a different location.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="newParentId"></param>
        /// <returns></returns>
        Task ChangeProjectParent(int projectId, int newParentId);

        /// <summary>
        /// Create a new project. Will return a WebException (409) Conflict if there is an existing project with that name in this location.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<int> CreateProject(CreateProjectRequest request);

        /// <summary>
        /// Delete a project. Will return a WebException (403) Forbidden if there are any sub-projects.
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task DeleteProject(int projectId);

        /// <summary>
        /// Get project details.
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<ProjectDetails> GetProject(int projectId);

        /// <summary>
        /// List all projects in the root.
        /// </summary>
        /// <param name="pageSize">The amount of passwords that TPM is configured to return (defaults to 20). http://teampasswordmanager.com/docs/api/#pagination</param>
        /// <returns></returns>
        Task<IEnumerable<ProjectEntry>> ListAllProjects(int pageSize = 20);

        /// <summary>
        /// List all sub-projects of a given project.
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<IEnumerable<ProjectEntry>> ListAllSubProjects(int projectId);

        /// <summary>
        /// Get a page of project entries in the root.
        /// </summary>
        /// <param name="page">Page number starting from 1</param>
        /// <returns></returns>
        Task<IEnumerable<ProjectEntry>> ListProjects(int page = 1);

        /// <summary>
        /// Unarchive a project.
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task UnarchiveProject(int projectId);

        /// <summary>
        /// Updates the project with the supplied details. Only non-null values will be applied.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task UpdateProject(int id, UpdateProjectRequest request);
    }

    internal class TpmProjectClient : ITpmProjectClient
    {
        private readonly TpmHttp http;

        public TpmProjectClient(TpmHttp http)
        {
            this.http = http;
        }

        public async Task<IEnumerable<ProjectEntry>> ListAllProjects(int pageSize = 20)
        {
            return await http.FetchAllPages(ListProjects, pageSize);
        }

        public async Task<IEnumerable<ProjectEntry>> ListProjects(int page = 1)
        {
            var response = (page == 1) ? await http.Get("api/v4/projects.json") : await http.Get($"api/v4/projects/page/{page}.json");
            return JsonConvert.DeserializeObject<List<ProjectEntry>>(response);
        }

        public async Task<IEnumerable<ProjectEntry>> ListAllSubProjects(int projectId)
        {
            var response = await http.Get($"api/v4/projects/{projectId}/subprojects.json");
            return JsonConvert.DeserializeObject<List<ProjectEntry>>(response);
        }

        public async Task<ProjectDetails> GetProject(int projectId)
        {
            var response = await http.Get($"api/v4/projects/{projectId}.json");
            return JsonConvert.DeserializeObject<ProjectDetails>(response);
        }

        public async Task<int> CreateProject(CreateProjectRequest request)
        {
            var body = JsonConvert.SerializeObject(request);
            var response = await http.Post("api/v4/projects.json", body);
            var created = JsonConvert.DeserializeObject<Created>(response);
            return Int32.Parse(created.Id);
        }

        public async Task UpdateProject(int id, UpdateProjectRequest request)
        {
            var body = JsonConvert.SerializeObject(request);
            await http.Put($"api/v4/projects/{id}.json", body);
        }

        public async Task ChangeProjectParent(int projectId, int newParentId)
        {
            var body = JsonConvert.SerializeObject(new ChangeProjectParent
            {
                Parent_Id = newParentId
            });

            await http.Put($"api/v4/projects/{projectId}/change_parent.json", body);
        }

        public async Task ArchiveProject(int projectId)
        {
            await http.Put($"api/v4/projects/{projectId}/archive.json");
        }

        public async Task UnarchiveProject(int projectId)
        {
            await http.Put($"api/v4/projects/{projectId}/unarchive.json");
        }

        public async Task DeleteProject(int projectId)
        {
            await http.Delete($"api/v4/projects/{projectId}.json");
        }
    }
}
