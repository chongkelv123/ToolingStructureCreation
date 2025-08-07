using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Integration
{
    /// <summary>
    /// Result from integration controller
    /// </summary>
    public class ToolingCreationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ProjectPath { get; set; }
        public int ComponentCount { get; set; }
        public Exception Exception { get; set; }
    }
}
