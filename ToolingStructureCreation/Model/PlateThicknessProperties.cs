using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ToolingStructureCreation.Constants.Const;

namespace ToolingStructureCreation.Model
{
    public class PlateThicknessProperties: PartProperties
    {
        public double UpperShoeThk { get; set; }
        public double UpperPadSpacerThk { get; set; } = 0.0;
        public double UpperPadThk { get; set; }
        public double PunchHolderThk { get; set; }
        public double BottomPlateThk { get; set; }
        public double StripperPlateThk { get; set; }
        public double MatThk { get; set; }        
        public double DiePlateThk { get; set; }
        public double LowerPadThk { get; set; }
        public double LowerPadSpacerThk { get; set; } = 0.0;
        public double LowerShoeThk { get; set; }        
        public double ParallelBarThk { get; set; }                                                
        public double CommonPlateThk { get; set; }

        public static Dictionary<string, string> GenerateKeyValue_Info(PlateThicknessProperties prop)
        {
            Dictionary<string, string> keyValue_Info = new Dictionary<string, string>()
            {
                [PartAttributeTitles.UPPER_SHOE] = prop.UpperShoeThk.ToString("0.00"),
                [PartAttributeTitles.UPPER_PAD_SPACER] = prop.UpperPadSpacerThk.ToString("0.00"),
                [PartAttributeTitles.UPPER_PAD] = prop.UpperPadThk.ToString("0.00"),
                [PartAttributeTitles.PUNCH_HOLDER] = prop.PunchHolderThk.ToString("0.00"),
                [PartAttributeTitles.BOTTOM_PLATE] = prop.BottomPlateThk.ToString("0.00"),
                [PartAttributeTitles.STRIPPER_PLATE] = prop.StripperPlateThk.ToString("0.00"),
                [PartAttributeTitles.MAT_THK] = prop.MatThk.ToString("0.00"),
                [PartAttributeTitles.DIE_PLATE] = prop.DiePlateThk.ToString("0.00"),
                [PartAttributeTitles.LOWER_PAD] = prop.LowerPadThk.ToString("0.00"),
                [PartAttributeTitles.LOWER_PAD_SPACER] = prop.LowerPadSpacerThk.ToString("0.00"),
                [PartAttributeTitles.LOWER_SHOE] = prop.LowerShoeThk.ToString("0.00"),
                [PartAttributeTitles.PARALLEL_BAR] = prop.ParallelBarThk.ToString("0.00"),
                [PartAttributeTitles.COMMON_PLATE] = prop.CommonPlateThk.ToString("0.00")
            };

            return keyValue_Info;
        }
    }

    
}
