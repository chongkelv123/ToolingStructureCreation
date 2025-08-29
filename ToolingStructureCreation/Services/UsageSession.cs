using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Model;

namespace ToolingStructureCreation.Services
{
    public class UsageSession
    {
        public string SessionId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string EngineerName { get; set; }
        public string ModuleVersion { get; set; }
        public string MachineType { get; set; }
        public string Model { get; set; }
        public string Part { get; set; }
        public MaterialGuideType GuideType { get; set; }
        public int StationCount { get; set; }
        public bool CompletedSuccessfully { get; set; }
        public List<ComponentCreated> Components { get; set; }
        public List<UserAction> Actions { get; set; }
        public ProjectMetrics Metrics { get; set; }
    }
}
