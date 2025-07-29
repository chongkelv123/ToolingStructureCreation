using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.Annotations;

namespace ToolingStructureCreation.Model
{
    public class TitleBlockProperties: PartProperties
    {        
        private string designBy;
        private string drawingCode;
        private string hrc;
        private string itemName;
        private string length;
        private string thickness;
        private string width;
        private string material;
        private string modelName;
        private string partName;
        private string quantity;

        private Dictionary<string, string> keyValue_Info;

        public Dictionary<string, string> KeyValueInfo => keyValue_Info;

        public TitleBlockProperties(Part workPart, string designBy, string drawingCode, string hrc, string itemName, string length, string thickness, string width, string material, string modelName, string partName, string quantity = "1") : base(workPart)
        {            
            this.designBy = designBy;
            this.drawingCode = drawingCode;
            this.hrc = hrc;
            this.itemName = itemName;
            this.length = length;
            this.thickness = thickness;
            this.width = width;
            this.material = material;
            this.modelName = modelName;
            this.partName = partName;
            this.quantity = quantity;

            GenerateKeyValue_Info();
        }

        private void GenerateKeyValue_Info()
        {
            keyValue_Info = new Dictionary<string, string>();
            keyValue_Info.Add(MODEL_NAME, modelName);
            keyValue_Info.Add(PART, partName);
            keyValue_Info.Add(ITEM_NAME, itemName);
            keyValue_Info.Add(DRAWING_CODE, drawingCode);
            keyValue_Info.Add(MATERIAL, material);
            keyValue_Info.Add(HRC, hrc);
            keyValue_Info.Add(QUANTITY, quantity);
            keyValue_Info.Add(DESIGNBY, designBy);
            keyValue_Info.Add(THICKNESS, thickness);
            keyValue_Info.Add(WIDTH, width);
            keyValue_Info.Add(LENGTH, length);
            keyValue_Info.Add(DESIGN_DATE, DateTime.Now.ToString("dd MMM yyyy"));
        }

        
    }
}
