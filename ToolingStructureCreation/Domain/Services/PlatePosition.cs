using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.Enums;
using ToolingStructureCreation.Domain.ValueObjects;

namespace ToolingStructureCreation.Domain.Services
{
    public class PlatePosition
    {
        public PlateType PlateType { get; }
        public Position3D Position { get; }

        public PlatePosition(PlateType plateType, Position3D position)
        {
            PlateType = plateType;
            Position = position ?? throw new ArgumentNullException(nameof(position));
        }

        public override string ToString()
        {
            return $"{PlateType} at {Position}";
        }
    }
}
