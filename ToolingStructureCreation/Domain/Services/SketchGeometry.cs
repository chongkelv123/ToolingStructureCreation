using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.ValueObjects;

namespace ToolingStructureCreation.Domain.Services
{
    public class SketchGeometry
    {
        public Dimensions Dimensions { get; }
        public Position3D StartLocation { get; }
        public Position3D MidPoint { get; }

        public SketchGeometry(Dimensions dimensions, Position3D startLocation, Position3D midPoint)
        {
            Dimensions = dimensions ?? throw new ArgumentNullException(nameof(dimensions));
            StartLocation = startLocation ?? throw new ArgumentNullException(nameof(startLocation));
            MidPoint = midPoint ?? throw new ArgumentNullException(nameof(midPoint));
        }

        public override string ToString()
        {
            return $"Sketch {Dimensions} at {StartLocation}";
        }
    }
}
