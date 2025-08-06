using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Domain.Repositories
{
    public class ProjectInfo
    {
        public string ProjectName { get; set; }
        public string Designer { get; set; }
        public string Customer { get; set; }
        public string PartNumber { get; set; }
        public string ModelNumber { get; set; }
        public string DrawingPrefix { get; set; }
        public string OutputDirectory { get; set; }
        public string MachineName { get; set; }
        public double MaterialThickness { get; set; }
        public Dictionary<string, double> PlateThicknesses { get; set; }

        public ProjectInfo()
        {
            PlateThicknesses = new Dictionary<string, double>();
        }
    }
}
