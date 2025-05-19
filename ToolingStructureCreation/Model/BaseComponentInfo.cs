using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Interfaces;

namespace ToolingStructureCreation.Model
{
    public class BaseComponentInfo
    {
        public TaggedObject Component { get; set; }
        public ComponentType Type { get; set; }
        public Point3d CenterPoint { get; set; }
        public BoundingBox BoundingBox { get; set; }
    }

    public class BoundingBox
    {
        public Point3d Min { get; set; }
        public Point3d Max { get; set; }      
    }
}
