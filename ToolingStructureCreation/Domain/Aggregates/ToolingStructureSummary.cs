using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.ValueObjects;

namespace ToolingStructureCreation.Domain.Aggregates
{
    public class ToolingStructureSummary
    {
        public string ProjectName { get; }
        public string Designer { get; }
        public DrawingCode BaseDrawingCode { get; }
        public string MachineName { get; }
        public int StationCount { get; }
        public int TotalPlateCount { get; }
        public double StripLength { get; }
        public double MaterialThickness { get; }
        public bool IsValid { get; }

        public ToolingStructureSummary(string projectName, string designer, DrawingCode baseDrawingCode,
            string machineName, int stationCount, int totalPlateCount, double stripLength,
            double materialThickness, bool isValid)
        {
            ProjectName = projectName;
            Designer = designer;
            BaseDrawingCode = baseDrawingCode;
            MachineName = machineName;
            StationCount = stationCount;
            TotalPlateCount = totalPlateCount;
            StripLength = stripLength;
            MaterialThickness = materialThickness;
            IsValid = isValid;
        }

        public override string ToString()
        {
            return $"{ProjectName} ({BaseDrawingCode}) - {StationCount} stations, {TotalPlateCount} plates, {StripLength:F1}mm strip";
        }
    }
}
