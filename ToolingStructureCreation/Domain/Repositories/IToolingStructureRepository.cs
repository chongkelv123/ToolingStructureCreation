using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.Aggregates;
using ToolingStructureCreation.Domain.ValueObjects;

namespace ToolingStructureCreation.Domain.Repositories
{
    public interface IToolingStructureRepository
    {
        Task<ToolingStructureAggregate> GetByDrawingCodeAsync(DrawingCode drawingCode);
        Task<List<ToolingStructureAggregate>> GetByProjectNameAsync(string projectName);
        Task<List<ToolingStructureAggregate>> GetByDesignerAsync(string designer);
        Task SaveAsync(ToolingStructureAggregate toolingStructure);
        Task UpdateAsync(ToolingStructureAggregate toolingStructure);
        Task DeleteAsync(DrawingCode drawingCode);
        Task<bool> ExistsAsync(DrawingCode drawingCode);
        Task<List<DrawingCode>> GetAllDrawingCodesAsync();
    }
}
