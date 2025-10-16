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
        public string DesignBy { get; set; }
        public string DrawingCode { get; set; }
        public string HRC { get; set; }
        public string ItemName {get; set; }
        public string Length { get; set; }
        public string Thickness { get; set; }
        public string Width { get; set; }
        public string Material { get; set; }
        public string ModelName { get; set; }
        public string PartName { get; set; }
        public string Quantity { get; set; }               

        public static Dictionary<string, string> GenerateKeyValue_Info(TitleBlockProperties titleProp)
        {
            Dictionary<string, string> keyValue_Info = new Dictionary<string, string>()
            {
                [MODEL_NAME] = titleProp.ModelName,
                [PART] = titleProp.PartName,
                [ITEM_NAME] = titleProp.ItemName,
                [DRAWING_CODE] = titleProp.DrawingCode,
                [MATERIAL] = titleProp.Material,
                [PartProperties.HRC] = titleProp.HRC,
                [QUANTITY] = titleProp.Quantity,
                [DESIGNBY] = titleProp.DesignBy,
                [THICKNESS] = titleProp.Thickness,
                [WIDTH] = titleProp.Width,
                [LENGTH] = titleProp.Length,
                [DESIGN_DATE] = DateTime.Now.ToString("dd MMM yyyy")
            };
            return keyValue_Info;
        }
    }
}
