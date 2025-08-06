using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.Services;

namespace ToolingStructureCreation.Domain.Repositories
{
    public interface ISketchRepository
    {
        Task<List<SketchInfo>> GetAvailableSketchesAsync();
        Task<SketchInfo> GetSketchByNameAsync(string sketchName);
        Task<SketchValidationResult> ValidateSketchAsync(string sketchName);
        Task<List<SketchGeometry>> GetValidatedSketchGeometriesAsync(List<string> sketchNames);
        Task RefreshSketchCacheAsync();
    }
}
