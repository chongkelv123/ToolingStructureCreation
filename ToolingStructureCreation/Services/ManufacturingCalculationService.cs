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

        /// <summary>
        /// Calculates the thickness of material guide full (front / rear)
        /// </summary>
        public static double CalculateMatGuideFullThk(double strPlt_Thk, double mat_Thk)
        {
            double totalHeight = strPlt_Thk + mat_Thk;
            return Math.Ceiling((totalHeight - 0.2) * 10) / 10.0; // Round up to nearest 0.1mm
        }

        /// <summary>
        /// Calculates the width of material guide full (front / rear)
        /// </summary>
        public static double CalculateMatGuideFullWidth(double plate_Width, double mat_Width)
        {
            return ((plate_Width - (mat_Width + 1)) / 2) + 4.5; // Adjusted width calculation
        }

        #endregion
        #region Utility Methods

        /// <summary>
        /// Snaps a value to the nearest standard band according to manufacturing rules
        /// 1. Values below smallest band snap UP to that band
        /// 2. Values between bands: if value > band && value <= band + 10, return band + 10
        /// 3. Values exactly matching a band return that band
        /// 4. Values above all bands return original value
        /// </summary>
        public double SnapToNearestBand(double value, List<double> bands)
        {
            if (bands == null || !bands.Any())
                return value;

            // Sort bands in ascending order for proper logic
            var sortedBands = bands.OrderBy(b => b).ToList();

            // Case 1: Value is below the smallest band - snap UP to smallest band
            if (value <= sortedBands.First())
            {
                return sortedBands.First();
            }

            // Case 2 & 3: Check each band for exact match or +10mm rule
            foreach (var band in sortedBands)
            {
                // Exact match - return the band
                if (value == band)
                {
                    return band;
                }

                // Between band and band+10 - snap to band+10
                if (value > band && value <= band + 10.0)
                {
                    return band + 10.0;
                }
            }

            // Case 4: Value is above all bands and their +10 ranges - return original
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

        /// <summary>
        /// Validates that critical thickness values are greater than zero for manufacturing
        /// </summary>
        public bool ValidateCriticalThicknesses(ThicknessData thicknesses)
        {
            if (thicknesses == null) return false;

            // These values must be > 0 for valid manufacturing
            return thicknesses.UpperShoeThk > 0 && thicknesses.DiePltThk > 0 &&
                   thicknesses.LowerShoeThk > 0 && thicknesses.MatThk > 0;
        }

        #endregion
    }
}
