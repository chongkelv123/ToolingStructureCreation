using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Model;

namespace ToolingStructureCreation.Domain.Repositories
{
    public  interface IProjectRepository
    {
        Task<ProjectInfo> GetProjectInfoAsync(string projectName);
        Task SaveProjectInfoAsync(ProjectInfo projectInfo);
        Task<List<string>> GetRecentProjectsAsync(int maxCount = 10);
        Task DeleteProjectInfoAsync(string projectName);
        Task<bool> ProjectExistsAsync(string projectName);
        Task<ProjectInfo> GetLastUsedProjectAsync();
        Task SetLastUsedProjectAsync(string projectName);
    }
}
