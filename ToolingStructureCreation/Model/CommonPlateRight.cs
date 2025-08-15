using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Services;

namespace ToolingStructureCreation.Model
{
    public class CommonPlateRight : CommonPlateBase
    {
        public const string TEMPLATE_LOWCOMPLTRIGHT_NAME = "3DA_Template_LOWCOMPLT_RIGHT-V00.prt";
        public const string LOWCOMPLT_RIGHT_PRESENTATION_NAME = "LowCommonPlateRight";

        public CommonPlateRight(double length, double width, double thickness, string fileName = null) : base(length, width, thickness, fileName)
        {
        }

        public override void CreateNewCommonPlate(string folderPath, ProjectInfo projectInfo, string drawingCode, string itemName)
        {
            var config = ComponentCreationConfigs.CreateCommonPlateConfig(
                TEMPLATE_LOWCOMPLTRIGHT_NAME,
                LOWCOMPLT_RIGHT_PRESENTATION_NAME,
                folderPath,
                GetFileName(),
                GetLength(),
                GetWidth(),
                GetThickness(),
                projectInfo,
                drawingCode,
                itemName
            );

            var creationService = new ComponentCreationService();
            creationService.CreateComponent(config);
        }
    }
}
