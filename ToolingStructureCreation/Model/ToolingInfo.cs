using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Model
{
    public enum MaterialGuideType
    {
        FullCoverage,
        PartialCoverage
    }
    public class ToolingInfo
    {
        public double CoilWidth { get; set; }
        public MaterialGuideType MaterialGuideType { get; set; }
        public Dictionary<string, double> KeyValuesPlateThk { get; set; }
        public Dictionary<string, Point3d> KeyValuesMaterialGuideThk { get; set; }

        public static ToolingInfo FromForm(double coilWidth, MaterialGuideType materialGuideType)
        {
            return new ToolingInfo 
            { 
                CoilWidth = coilWidth, 
                MaterialGuideType = materialGuideType 
            };                      
        }
    }

    
}
