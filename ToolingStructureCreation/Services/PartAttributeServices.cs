using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Model;
using static ToolingStructureCreation.Constants.Const;

namespace ToolingStructureCreation.Services
{
    public class PartAttributeServices
    {
        public static void UpdatePlateThicknessProperties(PlateThicknessProperties prop)
        {
            Part workPart = Session.GetSession().Parts.Work;
            PlateThicknessProperties plateThickness = new PlateThicknessProperties();

            var plateThkKeyValueInfo = PlateThicknessProperties.GenerateKeyValue_Info(prop);
            var attributeInfoList = plateThickness.AttributeInfoToList(
                Attributes.CATEGORY_PLATE_THK,
                plateThkKeyValueInfo);
            plateThickness.SetAttributesByList(attributeInfoList);
        }

        public static void UpdateTitleBlockProperties(TitleBlockProperties titleProp)
        {
            Part workPart = Session.GetSession().Parts.Work;
            TitleBlockProperties titleBlockProperties = new TitleBlockProperties();
            var titleKeyValueInfo = TitleBlockProperties.GenerateKeyValue_Info(titleProp);
            var attributeInfoList = titleBlockProperties.AttributeInfoToList(
                Attributes.CATEGORY_TITLEBLOCK,
                titleKeyValueInfo);
            titleBlockProperties.SetAttributesByList(attributeInfoList);
        }

        public static void UpdateToolProperties(ToolProperties toolProp)
        {
            Part workPart = Session.GetSession().Parts.Work;
            ToolProperties toolProperties = new ToolProperties();
            var toolKeyValueInfo = ToolProperties.GenerateKeyValue_Info(toolProp);
            var attributeInfoList = toolProperties.AttributeInfoToList(
                Attributes.CATEGORY_TOOL,
                toolKeyValueInfo);
            toolProperties.SetAttributesByList(attributeInfoList);
        }

        public static void UpdateToolingInfoProperties(ToolingInfoProperties toolingInfoProp)
        {
            Part workPart = Session.GetSession().Parts.Work;
            ToolingInfoProperties toolingInfoProperties = new ToolingInfoProperties();
            var toolingInfoKeyValueInfo = ToolingInfoProperties.GenerateKeyValue_Info(toolingInfoProp);
            var attributeInfoList = toolingInfoProperties.AttributeInfoToList(
                Attributes.CATEGORY_TOOLINGINFO,
                toolingInfoKeyValueInfo);
            toolingInfoProperties.SetAttributesByList(attributeInfoList);
        }
    }
}
