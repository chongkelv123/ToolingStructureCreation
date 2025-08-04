using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.ValueObjects;

namespace ToolingStructureCreation.Domain.Services
{
    public class ParallelBarPlacement
    {
        public Position3D Position { get; }
        public int SequenceNumber { get; }

        public ParallelBarPlacement(Position3D position, int sequenceNumber)
        {
            Position = position ?? throw new ArgumentNullException(nameof(position));
            SequenceNumber = sequenceNumber;
        }
        public override string ToString()
        {
            return $"ParallelBar #{SequenceNumber} at {Position}";
        }
    }
}
