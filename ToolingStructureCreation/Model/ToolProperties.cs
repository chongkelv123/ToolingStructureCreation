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
        private string partType;
        private string stdPartItem;
        private bool isStdPart;

        private Dictionary<string, string> keyValue_Info;
        public Dictionary<string, string> KeyValueInfo => KeyValueInfo;

        public ToolProperties(Part workPart, string partType, string stdPartItem, bool isStdPart): base(workPart)
        {
            this.partType = partType;
            this.stdPartItem = stdPartItem;
            this.isStdPart = isStdPart;

            GenerateKeyValue_Info();
        }

        private void GenerateKeyValue_Info()
        {
            keyValue_Info = new Dictionary<string, string>();
            keyValue_Info.Add(PART_TYPE, partType);
            keyValue_Info.Add(STDPART_ITEM, stdPartItem);
            keyValue_Info.Add(IS_STANDARD_PART, isStdPart ? "TRUE":"FALSE");
        }
    }
}
