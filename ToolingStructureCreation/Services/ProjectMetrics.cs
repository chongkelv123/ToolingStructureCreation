using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Services
{
    public class ProjectMetrics
    {
        public int TotalComponents { get; set; }
        public double TotalProcessingTimeMs { get; set; }
        public long TotalFileSizeBytes { get; set; }
        public double SessionDurationMs { get; set; }
    }
}
