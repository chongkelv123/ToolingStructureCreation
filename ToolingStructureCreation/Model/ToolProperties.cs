using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace ToolingStructureCreation.Model
{
    public class ToolProperties: PartProperties
    {        
        public bool IsStandardPart { get; set; }
        public string PartType { get; set; }
        public string StdPartItem { get; set; }

        public static Dictionary<string, string> GenerateKeyValue_Info(ToolProperties toolProp)
        {
            Dictionary<string, string> keyValue_Info = new Dictionary<string, string>()
            {
                [PART_TYPE] = toolProp.PartType                
            };

            return keyValue_Info;
        }
    }
}
