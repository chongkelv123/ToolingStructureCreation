using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Services;

namespace ToolingStructureCreation.Model
{
    public class CommonPlateLeft : CommonPlateBase
    {
        public const string TEMPLATE_LOWCOMPLTLEFT_NAME = "3DA_Template_LOWCOMPLT_LEFT-V00.prt";
        public const string LOWCOMPLT_LEFT_PRESENTATION_NAME = "LowCommonPlateLeft";

        public CommonPlateLeft(double length, double width, double thickness, string fileName = null) : base(length, width, thickness, fileName)
        {
        }

        public override void CreateNewCommonPlate(string folderPath, ProjectInfo projectInfo, string drawingCode, string itemName)
        {
            var config = ComponentCreationConfigs.CreateCommonPlateConfig(
                TEMPLATE_LOWCOMPLTLEFT_NAME,
                LOWCOMPLT_LEFT_PRESENTATION_NAME,
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
