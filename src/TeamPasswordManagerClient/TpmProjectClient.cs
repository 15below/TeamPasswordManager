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
        /// <param name="name"></param>
        /// <param name="parentId">ParentId of zero will place the project in the root</param>
        /// <param name="tags">Comma-separated list of tags</param>
        /// <param name="notes"></param>
        /// <returns></returns>
        Task<int> CreateProject(string name, int parentId = 0, string tags = null, string notes = null);

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
        /// <param name="details"></param>
        /// <returns></returns>
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
