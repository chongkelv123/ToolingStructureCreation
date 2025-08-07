using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToolingStructureCreation.Domain.Enums;

namespace ToolingStructureCreation.Domain.Services
{
    public class PlateThicknessCalculator
    {
        private readonly Dictionary<PlateType, double> _plateThicknesses;
        private readonly double _materialThickness;

        public PlateThicknessCalculator(Dictionary<PlateType, double> plateThicknesses, double materialThickness)
        {
            _plateThicknesses = plateThicknesses ?? throw new ArgumentNullException(nameof(plateThicknesses));
            _materialThickness = materialThickness;

            ValidateThicknesses();
        }
        private void ValidateThicknesses()
        {
            if (_materialThickness <= 0)
                throw new ArgumentException("Material thickness must be greater than zero.");

            if (!_plateThicknesses.Any())
                throw new ArgumentException("At least one plate thickness must be specified.");

            foreach (var kvp in _plateThicknesses)
            {
                if (kvp.Value <= 0)
                    throw new ArgumentException($"Thickness for {kvp.Key} must be greater than zero.");
            }
        }

        public double GetTotalDieHeight()
        {
            var plateTotal = _plateThicknesses.Sum(kvp => kvp.Value);
            return plateTotal + _materialThickness;
        }

        public double GetUpperDieSetThickness()
        {
            var upperPlates = new[]
            {
                PlateType.Upper_Pad,
                PlateType.Punch_Holder,
                PlateType.Bottoming_Plate,
                PlateType.Stripper_Plate
            };

            return upperPlates
                .Where(plateType => _plateThicknesses.ContainsKey(plateType))
                .Sum(plateType => _plateThicknesses[plateType]);
        }

        public double GetLowerDieSetThickness()
        {
            var lowerPlates = new[]
            {
                PlateType.Die_Plate,
                PlateType.Lower_Pad                
            };

            return lowerPlates
                .Where(plateType => _plateThicknesses.ContainsKey(plateType))
                .Sum(plateType => _plateThicknesses[plateType]);
        }

        public double GetPunchActiveLength()
        {
            // Business rule: Punch length calculation
            var activeThickness =
                GetThickness(PlateType.Punch_Holder) +
                GetThickness(PlateType.Bottoming_Plate) +
                GetThickness(PlateType.Stripper_Plate) +
                _materialThickness;

            return SnapToStandardPunchLength(activeThickness);

        }

        public double GetPenetrationDepth()
        {
            var punchLength = GetPunchActiveLength();
            var consumedLength =
                GetThickness(PlateType.Punch_Holder) +
                GetThickness(PlateType.Bottoming_Plate) +
                GetThickness(PlateType.Stripper_Plate) +
                _materialThickness;

            return Math.Max(0, punchLength - consumedLength);
        }

        public double GetCumulativeThicknessToPlate(PlateType plateType)
        {
            // Business rule: Calculate cumulative thickness from top to specified plate
            var plateOrder = new[]
            {
                PlateType.Upper_Pad,
                PlateType.Punch_Holder,
                PlateType.Bottoming_Plate,
                PlateType.Stripper_Plate,
                PlateType.Die_Plate,
                PlateType.Lower_Pad
            };

            double cumulative = 0;
            foreach(var plate in plateOrder)
            {
                if (plate == plateType)
                    break;
                if (_plateThicknesses.ContainsKey(plate))
                    cumulative += _plateThicknesses[plate];
                //Add material thickness after stripper plate
                if (plate == PlateType.Stripper_Plate)
                    cumulative += _materialThickness;
            }

            return cumulative;
        }

        private double SnapToStandardPunchLength(double requireLength)
        {
            // Business rule: Standard punch lengths
            var standardLengths = new[] { 50.0, 60.0, 70.0, 80.0, 90.0, 100.0 };

            var suitableLength = standardLengths.FirstOrDefault(length => length >= requireLength);
            return suitableLength > 0 ? suitableLength : standardLengths.Last();
        }

        public double GetThickness(PlateType plateType)
        {
            return _plateThicknesses.TryGetValue(plateType, out var thickness) ? thickness : 0;
        }

        public double GetMaterialThickness()
        {
            return _materialThickness;
        }

        public ToolingThicknessSummary GetThicknessSummary()
        {
            return new ToolingThicknessSummary(
                totalDieHeight: GetTotalDieHeight(),
                upperDieSetThickness: GetUpperDieSetThickness(),
                lowerDieSetThickness: GetLowerDieSetThickness(),
                punchActiveLength: GetPunchActiveLength(),
                penetrationDepth: GetPenetrationDepth(),
                materialThickness: _materialThickness
                );
        }
    }
}
