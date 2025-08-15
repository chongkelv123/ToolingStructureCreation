using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Services
{
    /// <summary>
    /// Contains all manufacturing calculation logic extracted from formToolStructure
    /// for better separation of concerns and unit testing capability
    /// </summary>
    public class ManufacturingCalculationService
    {
        #region Core Calculation Methods

        /// <summary>
        /// Calculates total die height by summing all thickness values
        /// </summary>
        public double CalculateDieHeight(ThicknessData thicknesses)
        {
            return thicknesses.UpperShoeThk + thicknesses.UpperPadThk + thicknesses.PunHolderThk +
                   thicknesses.BottomPltThk + thicknesses.StripperPltThk + thicknesses.MatThk +
                   thicknesses.DiePltThk + thicknesses.LowerPadThk + thicknesses.LowerShoeThk +
                   thicknesses.ParallelBarThk + thicknesses.CommonPltThk;
        }

        /// <summary>
        /// Calculates punch length using standard band snapping logic
        /// </summary>
        public double CalculatePunchLength(ThicknessData thicknesses)
        {
            var punchLengthBands = new List<double> { 50.0, 60.0, 70.0, 80.0 };
            double requiredLength = CalculatePHld_BPlt_SPlt_MatThk(thicknesses);
            return SnapToNearestBand(requiredLength, punchLengthBands);
        }

        /// <summary>
        /// Calculates punch penetration (punch length minus required thickness)
        /// </summary>
        public double CalculatePenetration(ThicknessData thicknesses)
        {
            double punchLength = CalculatePunchLength(thicknesses);
            double requiredThickness = CalculatePHld_BPlt_SPlt_MatThk(thicknesses);
            return punchLength - requiredThickness;
        }

        /// <summary>
        /// Calculates feed height based on lift height and lower die set thickness
        /// </summary>
        public double CalculateFeedHeight(ThicknessData thicknesses, double liftHeight)
        {
            double lowerDieSetThickness = CalculateLowerDieSetThickness(thicknesses);
            return liftHeight + lowerDieSetThickness;
        }

        #endregion

        #region Position Calculations

        /// <summary>
        /// Calculates Z position for upper shoe component
        /// </summary>
        public double CalculateUpperShoeZPosition(ThicknessData thicknesses)
        {
            return thicknesses.UpperShoeThk + thicknesses.UpperPadThk + thicknesses.PunHolderThk +
                   thicknesses.BottomPltThk + thicknesses.StripperPltThk + thicknesses.MatThk;
        }

        /// <summary>
        /// Calculates Z position for parallel bar component (negative direction)
        /// </summary>
        public double CalculateParallelBarZPosition(ThicknessData thicknesses)
        {
            return (thicknesses.DiePltThk + thicknesses.LowerPadThk + thicknesses.LowerShoeThk) * -1;
        }

        /// <summary>
        /// Calculates Z position for common plate component (negative direction)
        /// </summary>
        public double CalculateCommonPlateZPosition(ThicknessData thicknesses)
        {
            return (thicknesses.DiePltThk + thicknesses.LowerPadThk +
                    thicknesses.LowerShoeThk + thicknesses.ParallelBarThk) * -1;
        }

        #endregion

        #region Sub-Calculations

        /// <summary>
        /// Calculates combined thickness of die plate and lower pad
        /// </summary>
        public double CalculateDiePlt_LowPadThk(ThicknessData thicknesses)
        {
            return thicknesses.DiePltThk + thicknesses.LowerPadThk;
        }

        /// <summary>
        /// Calculates total lower die set thickness
        /// </summary>
        public double CalculateLowerDieSetThickness(ThicknessData thicknesses)
        {
            return thicknesses.LowerShoeThk + thicknesses.LowerPadThk +
                   thicknesses.DiePltThk + thicknesses.ParallelBarThk + thicknesses.CommonPltThk;
        }

        /// <summary>
        /// Calculates combined thickness of punch holder, bottom plate, stripper plate, and material
        /// </summary>
        public double CalculatePHld_BPlt_SPlt_MatThk(ThicknessData thicknesses)
        {
            return thicknesses.PunHolderThk + thicknesses.BottomPltThk +
                   thicknesses.StripperPltThk + thicknesses.MatThk;
        }

        #endregion
        #region Utility Methods

        /// <summary>
        /// Snaps a value to the nearest standard band according to manufacturing rules
        /// If value is within 10mm above a band, snap to band + 10mm
        /// </summary>
        public double SnapToNearestBand(double value, List<double> bands)
        {
            if (bands == null || !bands.Any())
                return value;

            foreach (var band in bands.OrderByDescending(b => b))
            {
                if (value > band && value <= band + 10.0)
                {
                    return band + 10.0;
                }
                if (value == band)
                {
                    return band;
                }
            }
            return value;
        }

        /// <summary>
        /// Validates that all thickness values are positive
        /// </summary>
        public bool ValidateThicknesses(ThicknessData thicknesses)
        {
            if (thicknesses == null) return false;

            return thicknesses.UpperShoeThk >= 0 && thicknesses.UpperPadThk >= 0 &&
                   thicknesses.PunHolderThk >= 0 && thicknesses.BottomPltThk >= 0 &&
                   thicknesses.StripperPltThk >= 0 && thicknesses.MatThk >= 0 &&
                   thicknesses.DiePltThk >= 0 && thicknesses.LowerPadThk >= 0 &&
                   thicknesses.LowerShoeThk >= 0 && thicknesses.ParallelBarThk >= 0 &&
                   thicknesses.CommonPltThk >= 0;
        }

        #endregion
    }
}
