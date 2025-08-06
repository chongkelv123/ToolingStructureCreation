using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.Enums;

namespace ToolingStructureCreation.Domain.Repositories
{
    public interface ITemplateRepository
    {
        Task<string> GetPlateTemplatePathAsync(PlateType plateType);
        Task<string> GetShoeTemplatePathAsync(ShoeType shoeType);
        Task<string> GetParallelBarTemplatePathAsync();
        Task<string> GetCommonPlateTemplatePathAsync(CommonPlateType plateType);
        Task<List<string>> GetAvailableTemplatesAsync();
        Task<bool> TemplateExistsAsync(string templatePath);
        Task ValidateTemplateAsync(string templatePath);
    }
}
