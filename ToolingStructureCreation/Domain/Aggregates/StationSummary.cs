using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.ValueObjects;

namespace ToolingStructureCreation.Domain.Aggregates
{
    public class StationSummary
    {
        public int StationNumber { get; }
        public int PlateCount { get; }
        public double TotalThickness { get; }
        public bool HasAllRequiredPlates { get; }
        public Dimensions StationDimensions { get; }

        public StationSummary(int stationNumber, int plateCount, double totalThickness,
            bool hasAllRequiredPlates, Dimensions stationDimensions)
        {
            StationNumber = stationNumber;
            PlateCount = plateCount;
            TotalThickness = totalThickness;
            HasAllRequiredPlates = hasAllRequiredPlates;
            StationDimensions = stationDimensions;
        }

        public override string ToString()
        {
            return $"Station {StationNumber}: {PlateCount} plates, {TotalThickness:F1}mm thick, Complete: {HasAllRequiredPlates}";
        }
    }
}
