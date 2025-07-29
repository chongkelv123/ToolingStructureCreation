using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace ToolingStructureCreation.Model
{
    public abstract class PartProperties
    {
        protected Part workPart;

        public const string CATEGORY_TITLE = "TITLEBLOCK";
        public const string CATEGORY_TOOL = "Tool";

        public const string PART = "PART";
        public const string DRAWING_CODE = "DRAWING CODE";
        public const string MODEL_NAME = "MODEL NAME";
        public const string ITEM_NAME = "ITEM NAME";
        public const string QUANTITY = "QUANTITY";
        public const string MATERIAL = "MATL";
        public const string HRC = "HRC";
        public const string THICKNESS = "Thk";
        public const string LENGTH = "Length/Diameter";
        public const string WIDTH = "Width";
        public const string DESIGNBY = "DesignBy";
        public const string PART_TYPE = "PartType";
        public const string DESIGN_DATE = "Design Date";
        public const string IS_STANDARD_PART = "isStandardPart";
        public const string STDPART_ITEM = "StdPartItem";
        public const string STD_PART = "STD PART";
        public const string PLATE = "PLATE";
        public const string ASM = "ASM";
        public const string SHOE = "SHOE";

        protected PartProperties(Part workPart)
        {
            this.workPart = workPart;
        }

        public List<NXObject.AttributeInformation> AttributeInfoToList(string category, Dictionary<string, string> titleInfos)
        {
            List<NXObject.AttributeInformation> result = new List<NXObject.AttributeInformation>();
            foreach (var titleInfo in titleInfos)
            {
                NXObject.AttributeInformation info = new NXObject.AttributeInformation();

                if (titleInfo.Value.Equals("TRUE"))
                {
                    info.Type = NXObject.AttributeType.Boolean;
                    info.BooleanValue = true;
                }
                else
                {
                    info.Type = NXObject.AttributeType.String;
                    info.StringValue = titleInfo.Value;
                }
                
                info.Category = category;
                info.Title = titleInfo.Key;

                result.Add(info);
            }

            return result;
        }

        public void SetAttributesByList(List<NXObject.AttributeInformation> attributes)
        {
            try
            {
                attributes.ForEach(a => { workPart.SetUserAttribute(a, Update.Option.Now); });
            }
            catch (Exception e)
            {
                string message = $"Error occur at TitleBlockProperties: {e.Message}";
                NXDrawing.ShowMessageBox("ERROR", NXMessageBox.DialogType.Error, message);
            }
        }

        public void SetAttribute(NXObject.AttributeInformation info)
        {
            workPart.SetUserAttribute(info, Update.Option.Now);
        }
    }
}
