using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ToolingStructureCreation.Constants.Const;

namespace ToolingStructureCreation.Model
{
    public class ToolingInfoProperties: PartProperties
    {
        public double CoilWidth { get; set; }
        public double LiftingHeight { get; set; }
        public double PunchLength { get; set; }
        public double StripperStroke { get; set; } = 6.5;

        public static Dictionary<string, string> GenerateKeyValue_Info(ToolingInfoProperties prop)
        {
            Dictionary<string, string> keyValue_Info = new Dictionary<string, string>()
            {
                [PartAttributeTitles.COIL_WIDTH] = prop.CoilWidth.ToString("0.00"),
                [PartAttributeTitles.LIFTING_HEIGHT] = prop.LiftingHeight.ToString("0.00"),
                [PartAttributeTitles.PUNCH_LENGTH] = prop.PunchLength.ToString("0.00"),
                [PartAttributeTitles.STRIPPER_STROKE] = prop.StripperStroke.ToString("0.00")
            };
            return keyValue_Info;
        }
    }
}
