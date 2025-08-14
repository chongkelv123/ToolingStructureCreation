using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Services
{
    /// <summary>
    /// Form validation data transfer object
    /// </summary>
    public class FormValidationData
    {
        public string Path { get; set; }
        public string UpperShoeThk { get; set; }
        public string UpperPadThk { get; set; }
        public string PunHolderThk { get; set; }
        public string BottomPltThk { get; set; }
        public string StripperPltThk { get; set; }
        public string MatThk { get; set; }
        public string DiePltThk { get; set; }
        public string LowerPadThk { get; set; }
        public string LowerShoeThk { get; set; }
        public string ParallelBarThk { get; set; }
        public string CommonPltThk { get; set; }
        public bool IsPlateSketchSelected { get; set; }
        public bool IsShoeSketchSelected { get; set; }

        // Project Info Fields
        public string Model { get; set; }
        public string Part { get; set; }
        public string CodePrefix { get; set; }
        public string Designer { get; set; }
    }
}
