using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Services
{
    /// <summary>
    /// Data transfer object containing all thickness values for calculations
    /// </summary>
    public class ThicknessData
    {
        public double UpperShoeThk { get; set; }
        public double UpperPadThk { get; set; }
        public double PunHolderThk { get; set; }
        public double BottomPltThk { get; set; }
        public double StripperPltThk { get; set; }
        public double MatThk { get; set; }
        public double DiePltThk { get; set; }
        public double LowerPadThk { get; set; }
        public double LowerShoeThk { get; set; }
        public double ParallelBarThk { get; set; }
        public double CommonPltThk { get; set; }

        /// <summary>
        /// Creates ThicknessData from form property getters
        /// </summary>
        public static ThicknessData FromForm(
            double upperShoeThk, double upperPadThk, double punHolderThk,
            double bottomPltThk, double stripperPltThk, double matThk,
            double diePltThk, double lowerPadThk, double lowerShoeThk,
            double parallelBarThk, double commonPltThk)
        {
            return new ThicknessData
            {
                UpperShoeThk = upperShoeThk,
                UpperPadThk = upperPadThk,
                PunHolderThk = punHolderThk,
                BottomPltThk = bottomPltThk,
                StripperPltThk = stripperPltThk,
                MatThk = matThk,
                DiePltThk = diePltThk,
                LowerPadThk = lowerPadThk,
                LowerShoeThk = lowerShoeThk,
                ParallelBarThk = parallelBarThk,
                CommonPltThk = commonPltThk
            };
        }

        /// <summary>
        /// Creates ThicknessData with standard test values
        /// </summary>
        public static ThicknessData CreateTestData()
        {
            return new ThicknessData
            {
                UpperShoeThk = 70.0,
                UpperPadThk = 27.0,
                PunHolderThk = 30.0,
                BottomPltThk = 25.0,
                StripperPltThk = 20.0,
                MatThk = 2.0,
                DiePltThk = 40.0,
                LowerPadThk = 20.0,
                LowerShoeThk = 70.0,
                ParallelBarThk = 15.0,
                CommonPltThk = 40.0
            };
        }
    }
}
