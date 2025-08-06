using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.Enums;
using ToolingStructureCreation.Domain.ValueObjects;

namespace ToolingStructureCreation.Domain.Aggregates
{
    public class CommonPlatePositioning
    {
        public CommonPlateType PlateType { get; }
        public Position3D Position { get; }
        public int SequenceNumber { get; }
        public string Description { get; }

        public CommonPlatePositioning(CommonPlateType plateType, Position3D position, int sequenceNumber, string description)
        {
            PlateType = plateType;
            Position = position ?? throw new ArgumentNullException(nameof(position));
            SequenceNumber = sequenceNumber;
            Description = description ?? string.Empty;
        }

        public override string ToString()
        {
            return $"{PlateType} #{SequenceNumber} at {Position} - {Description}";
        }
    }
}
