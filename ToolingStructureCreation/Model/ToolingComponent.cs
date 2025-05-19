using NXOpen;
using System.Collections.Generic;

namespace ToolingStructureCreation.Model
{
    public enum ToolingComponentType
    {
        Plate,
        Shoe,
        Fixture,
        Clamp
    }
    public class ToolingComponent
    {
        public TaggedObject NXObject { get; set; }
        public ToolingComponentType Type { get; set; }
        public string TypeName => Type.ToString();
        public int Index { get; set; }
        public ToolingMaterial Material { get; set; }
        public double Thickness { get; set; }
        public Point3d Position { get; set; }

        // Component-specific metrics and parameters
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
    }

    public class ComponentParameters
    {
        public BaseComponentInfo BaseInfo { get; set; }
        public ToolingMaterial Material { get; set; }
        public double Clearance { get; set; }
        public double Tolerance { get; set; }
        public int Index { get; set; }
        // Additional parameters as needed
    }

}
