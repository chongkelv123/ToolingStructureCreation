using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Services
{
    public class ComponentCreated
    {
        public string ComponentType { get; set; }
        public DateTime CreatedAt { get; set; }
        public long FileSizeBytes { get; set; }
        public double ProcessingTimeMs { get; set; }
    }
}
