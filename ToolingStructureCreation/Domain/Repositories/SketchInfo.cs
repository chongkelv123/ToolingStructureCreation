using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.ValueObjects;

namespace ToolingStructureCreation.Domain.Repositories
{
    public class SketchInfo
    {
        public string SketchName { get; set; }
        public List<Position3D> Points { get; set; }
        public string PartName { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }

        public SketchInfo()
        {
            Points = new List<Position3D>();
        }
    }
}
