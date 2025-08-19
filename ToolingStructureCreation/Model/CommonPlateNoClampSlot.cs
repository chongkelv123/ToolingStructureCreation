using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Services;

namespace ToolingStructureCreation.Model
{
    public class CommonPlateNoClampSlot : CommonPlateBase
    {
        public const string TEMPLATE_LOWCOMPLTNOSLOT_NAME = "3DA_Template_LOWCOMPLT-NOSLOT-V00.prt";
        public const string LOWCOMPLTNOSLOT_PRESENTATION_NAME = "LowCommonPlateNoClampSlot";

        public CommonPlateNoClampSlot(double length, double width, double thickness, string fileName = null) :
            base(length, width, thickness, fileName)
        {
        }
        public override void CreateNewCommonPlate(string folderPath, ProjectInfo projectInfo, string drawingCode, string itemName)
        {
            var config = ComponentCreationConfigs.CreateCommonPlateConfig(
                TEMPLATE_LOWCOMPLTNOSLOT_NAME,
                LOWCOMPLTNOSLOT_PRESENTATION_NAME,
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
