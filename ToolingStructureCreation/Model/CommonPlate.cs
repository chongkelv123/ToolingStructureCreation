using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Services;
using static NXOpen.Motion.HydrodynamicBearingBuilder;

namespace ToolingStructureCreation.Model
{
    public class CommonPlate : CommonPlateBase
    {
        public const string TEMPLATE_LOWCOMPLT_NAME = "3DA_Template_LOWCOMPLT-V00.prt";
        public const string LOWCOMPLT_PRESENTATION_NAME = "LowCommonPlate";
        public const string LOWER_COMMON_PLATE = "LOWER_COMMON_PLATE";

        public CommonPlate(double length, double width, double thickness, string fileName = null) :
            base(length, width, thickness, fileName)
        {
        }

        public override void CreateNewCommonPlate(string folderPath, ProjectInfo projectInfo, string drawingCode, string itemName)
        {
            var config = ComponentCreationConfigs.CreateCommonPlateConfig(
                TEMPLATE_LOWCOMPLT_NAME,
                LOWCOMPLT_PRESENTATION_NAME,
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
