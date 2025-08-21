using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Services;

namespace ToolingStructureCreation.Model
{
    public class LowerShoe : ShoeBase
    {
        public LowerShoe(string fileName, double length, double width, double thickness) 
            : base(fileName, length, width, thickness)
        {
        }

        public override void Create(string folderPath, ProjectInfo projectInfo, string drawingCode, string itemName)
        {
            var config = ComponentCreationConfigs.CreateShoeConfig(
                TEMPLATE_LOWSHOE_NAME,
                LOWSHOE_PRESENTATION_NAME,
                folderPath,
                FileName,
                Length,
                Width,
                Thickness,
                projectInfo,
                drawingCode,
                itemName
            );

            var creationService = new ComponentCreationService();
            creationService.CreateComponent(config);
        }
    }
}
