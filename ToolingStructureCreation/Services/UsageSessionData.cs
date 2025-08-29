using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Services
{
    public class UsageSessionData
    {
        public string SessionId { get; set; }
        public DateTime Timestamp { get; set; }
        public string EngineerName { get; set; } = "";
        public string Model { get; set; } = "";
        public string Part { get; set; } = "";
        public string MachineType { get; set; } = "";
        public string GuideType { get; set; } = "";
        public int StationCount { get; set; }
        public int PlateShoeCount { get; set; }
        public int AssemblyCount { get; set; }
        public int TotalComponents { get; set; }
        public double EstimatedManualTimeMin { get; set; }
        public double ActualTimeMin { get; set; }
        public double TimeSavedMin { get; set; }
        public double ReductionPercentage { get; set; }
        public bool CompletedSuccessfully { get; set; }
        public double TotalFileSizeMB { get; set; }
        public long TotalFileSizeBytes { get; set; }
        public string ModuleVersion { get; set; } = "";
    }
}
