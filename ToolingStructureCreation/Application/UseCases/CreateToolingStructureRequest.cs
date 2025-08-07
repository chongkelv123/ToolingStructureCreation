using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.Services;

namespace ToolingStructureCreation.Application.UseCases
{
    /// <summary>
    /// Request for tooling structure creation
    /// </summary>
    public class CreateToolingStructureRequest
    {
        public ToolingParameters ToolingParameters { get; set; }
        public List<SketchGeometry> StationSketches { get; set; }
        public List<SketchGeometry> ShoeSketches { get; set; }
        public List<SketchGeometry> CommonPlateSketches { get; set; }
        public string OutputDirectory { get; set; }

        public CreateToolingStructureRequest()
        {
            StationSketches = new List<SketchGeometry>();
            ShoeSketches = new List<SketchGeometry>();
            CommonPlateSketches = new List<SketchGeometry>();
        }
    }
}
