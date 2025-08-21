using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Services;

namespace ToolingStructureCreation.Model
{
    public class MatGuideFullFront : MatGuideFullBase
    {        
        public MatGuideFullFront(double length, double width, double thickness, string fileName = null) 
            : base(length, width, thickness, fileName)
        {
        }

        public override void Create(string folderPath, ProjectInfo projectInfo, string drawingCode, string itemName)
        {
            var config = ComponentCreationConfigs.CreateMaterialGuideFullConfig(
                TEMPLATE_MATGUIDEFULLFRONT_NAME,
                MATGUIDEFULLFRONT_PRESENTATION_NAME,
                folderPath,
                FileName,
                Length,
                Width,
                Thickness,
                projectInfo,
                drawingCode,
                itemName,
                isMatGuideFull: true
            );

            var creationService = new ComponentCreationService();
            creationService.CreateComponent(config);
        }
    }
}
