using NXOpen;
using ToolingStructureCreation.Interfaces;

namespace ToolingStructureCreation.Model
{
    public class ToolingParameters
    {
        public double UpperShoeThickness { get; set; }
        public double UpperPadThickness { get; set; }
        public double PunchHolderThickness { get; set; }
        public double BottomPadThickness { get; set; }
        public double StripperThickness { get; set; }
        public double MaterialThickness { get; set; }
        public double DiePlateThickness { get; set; }
        public double LowerPadThickness { get; set; }
        public double LowerShoeThickness { get; set; }


        public TaggedObject BaseComponent { get; set; }
        public ComponentType BaseComponentType { get; set; }
        public double Clearance { get; set; } = 1.0;
        public double Tolerance { get; set; } = 0.1;
        public bool CreateFixture { get; set; } = true;
        public bool CreateClamps { get; set; } = false;
        public int NumberOfClamps { get; set; } = 0;
        public ToolingMaterial Material { get; set; } = ToolingMaterial.Steel;
        public ToolingTemplate SelectedTemplate { get; set; }

        public bool IsValid =>
            BaseComponent != null &&
            Clearance > 0 &&
            Tolerance > 0 &&
            (!CreateClamps || NumberOfClamps > 0);
    }
}
