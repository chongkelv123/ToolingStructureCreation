using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Constants
{
    public class Const
    {
        public static class Attributes
        {
            public const string CATEGORY_PLT_THK = "PlateThickness";
            public const string CATEGORY_TITLEBLOCK = "TITLEBLOCK";
            public const string CATEGORY_TOOL = "Tool";
            public const string CATEGORY_TOOLINGINFO = "ToolingInfo";

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
            
        }

        public static class PartAttributeTitles
        {
            public const string BOTTOM_PLATE = "BottomPlate";
            public const string COMMON_PLATE = "CommonPlate";
            public const string DIE_PLATE = "DiePlate";
            public const string LOWER_PAD = "LowerPad";
            public const string LOWER_SHOE = "LowerShoe";
            public const string MAT_THK = "MatThk";
            public const string PARALLEL_BAR = "ParallelBar";
            public const string PUNCH_HOLDER = "PunchHolder";
            public const string STRIPPER_PLATE = "StripperPlate";
            public const string UPPER_PAD = "UpperPad";
            public const string UPPER_SHOE = "UpperShoe";
            public const string UPPER_PAD_SPACER = "UpperPadSpacer";
            public const string LOWER_PAD_SPACER = "LowerPadSpacer";
            public const string COIL_WIDTH = "CoilWidth";
            public const string PUNCH_LENGTH = "PunchLength";
            public const string LIFTING_STROKE = "LiftingStroke";
            public const string STRIPPER_STROKE = "StripperStroke";
        }

        public static class PartType
        {
            // PartType
            public const string SHOE = "SHOE";
            public const string PLATE = "PLATE";
            public const string INSERT = "INSERT";
            public const string WCBLK = "W/C BLK";
            public const string OTHERS = "OTHERS";
            public const string ASM = "ASM";
        }

        public static class ProjectInfo
        {
            // ProjectInfo
            public const string MODEL = "MODEL";
            public const string PART = "PART";
            public const string CODE_PREFIX = "CODE_PREFIX";
            public const string DESIGNER = "DESIGNER";
        }
        public static class Material
        {
            // Material
            public const string S50C = "S50C";
            public const string DC53 = "DC53";
            public const string GOA = "GOA";
            public const string MILD_STELL = "MILD STELL";
            public const string NAK80 = "NAK80";
            public const string SKD11 = "SKD11";
            public const string YXR3 = "YXR3";
            public const string YXM1 = "YXM1";
            public const string DEX20 = "DEX20";
            public const string EG2 = "E.G. 2.0t";
        }

        public static class HRC
        {
            // Hardness
            public const string HYPHEN = "-";
            public const string THIRTYFIVE_FOURTY = "35~40";
            public const string FIFTYTWO_FIFTYFOUR = "52~54";
            public const string FIFTYSEVEN_FIFTYNINE = "57~59";
            public const string FIFTYEIGHT_SIXTY = "58~60";
            public const string SIXTY_SIXTYTHREE = "60~63";
            public const string SIXTYTWO_SIXTYFIVE = "62~65";
        }

        public static class PlateType
        {
            // PlateType
            public const string UPPER_PAD_SPACER = "UPPER PAD SPACER";
            public const string UPPER_PAD = "UPPER PAD";
            public const string PUNCH_HOLDER = "PUNCH HOLDER";
            public const string BOTTOMING_PLATE = "BOTTOMING PLATE";
            public const string STRIPPER_PLATE = "STRIPPER PLATE";
            public const string DIE_PLATE = "DIE PLATE";
            public const string DIE_PLATE_R = "DIE PLATE-R";
            public const string DIE_PLATE_F = "DIE PLATE-F";
            public const string LOWER_PAD = "LOWER PAD";
            public const string LOWER_PAD_SPACER = "LOWER PAD SPACER";
        }

        public static class ShoeType
        {
            // ShoeType
            public const string UPPER_SHOE = "UPPER SHOE";
            public const string LOWER_SHOE = "LOWER SHOE";
            public const string PARALLEL_BAR = "PARALLEL BAR";
            public const string LOWER_COMMON_PLATE = "LOWER COMMON PLATE";
        }

        public static class AsmType
        {
            // AsmType
            public const string MAIN_ASSEMBLY = "MAIN ASSEMBLY";
            public const string TOP_ASSEMBLY = "TOP ASSEMBLY";
            public const string BOTTOM_ASSEMBLY = "BOTTOM ASSEMBLY";
            public const string LEFT_ASSEMBLY = "LEFT ASSEMBLY";
            public const string RIGHT_ASSEMBLY = "RIGHT ASSEMBLY";
            public const string ASSEMBLY_FOR = "ASSEMBLY FOR ";
        }

        public static class NxTemplate
        {
            public const string UG_APP_MODELING = "UG_APP_MODELING";
            public const string MODEL = "MODEL";
            public const string EXTENSION = ".prt";
            public const string MODEL_TEMPLATE = "ModelTemplate";
            public const string TEMPLATE_3DASTP_NAME = "3DA_Template_STP-V00.prt";
            public const string TMP_PRESENTATION_NAME_MODEL = "Model";
        }
    }
}
