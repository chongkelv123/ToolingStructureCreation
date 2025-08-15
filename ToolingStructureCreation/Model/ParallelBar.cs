using NXOpen;
using NXOpen.Features.ShipDesign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Services;
using static NXOpen.Motion.HydrodynamicBearingBuilder;

namespace ToolingStructureCreation.Model
{
    public class ParallelBar
    {
        private string fileName;
        private double length;
        private double width;
        private double thickness;
        public int Quantity { get; set; }

        public const string TEMPLATE_PARALLELBAR_NAME = "3DA_Template_PARALLELBAR-V00.prt";
        public const string PBAR_PRESENTATION_NAME = "ParallelBar";

        public const string PARALLEL_BAR = "PARALLEL_BAR";

        public ParallelBar(string fileName, double length, double width, double thickness)
        {
            this.fileName = fileName;
            this.length = length;
            this.width = width;
            this.thickness = thickness;
        }

        public double GetParallelBarLength() => length;
        public double GetParallelBarWidth() => width;
        public double GetParallelBarThickness() => thickness;
        
        public void CreateNewParallelBar(string folderPath, ProjectInfo projectInfo, string drawingCode, string itemName)
        {
            var config = ComponentCreationConfigs.CreateParallelBarConfig(
                folderPath, fileName, length, width, thickness, projectInfo, drawingCode, itemName
                );

            var creationService = new ComponentCreationService();
            creationService.CreateComponent(config);
        }
    }
}
