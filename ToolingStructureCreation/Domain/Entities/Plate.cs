using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.Enums;
using ToolingStructureCreation.Domain.ValueObjects;

namespace ToolingStructureCreation.Domain.Entities
{
    public class Plate
    {
        public string Name { get; }
        public Dimensions Dimensions { get; }
        public PlateType Type { get; }
        public PlateColor Color { get; }
        public string Material { get; }
        public string Hardness { get; }

        public Plate(string name, Dimensions dimensions, PlateType type, string material = "GOA", string hardness = "52~54")
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Plate name cannot be null or empty.", nameof(name));
                
            Name = name.Trim();
            Dimensions = dimensions ?? throw new ArgumentNullException(nameof(dimensions));
            Type = type;
            Color = DetermineColor(type);
            Material = string.IsNullOrWhiteSpace(material) ? "GOA" : material.Trim();
            Hardness = string.IsNullOrWhiteSpace(hardness) ? "52~54" : hardness.Trim();

            ValidateBusinessRules();
        }

        private void ValidateBusinessRules()
        {
            // Business rule: Thickness constraints by plate type
            switch (Type)
            {
                case PlateType.UpperPad:
                    if (Dimensions.Thickness < 15 || Dimensions.Thickness > 50)
                        throw new ArgumentException($"Upper pad thickness must be between 15-50mm, got {Dimensions.Thickness}mm");
                    break;
                case PlateType.PunchHolder:
                    if (Dimensions.Thickness < 20 || Dimensions.Thickness > 60)
                        throw new ArgumentException($"Punch holder thickness must be between 20-60mm, got {Dimensions.Thickness}mm");
                    break;
                case PlateType.DiePlate:
                    if (Dimensions.Thickness < 25 || Dimensions.Thickness > 80)
                        throw new ArgumentException($"Die plate thickness must be between 25-80mm, got {Dimensions.Thickness}mm");
                    break;
                case PlateType.LowerPad:
                    if (Dimensions.Thickness < 15 || Dimensions.Thickness > 40)
                        throw new ArgumentException($"Lower pad thickness must be between 15-40mm, got {Dimensions.Thickness}mm");
                    break;
            }

            // Business rule: Minimum area requirement
            if (Dimensions.Area < 100) // 10mm x 10mm minimum
                throw new ArgumentException("Plate area cannot be less than 100 square mm");
        }

        private static PlateColor DetermineColor(PlateType type)
        {
            switch (type)
            {
                case PlateType.UpperPad:
                    return PlateColor.UpperPad;
                case PlateType.PunchHolder:
                    return PlateColor.PunchHolder;
                case PlateType.BottomingPlate:
                    return PlateColor.BottomingPlate;
                case PlateType.StripperPlate:
                    return PlateColor.StripperPlate;
                case PlateType.DiePlate:
                    return PlateColor.DiePlate;
                case PlateType.LowerPad:
                    return PlateColor.LowerPad;
                default:
                    return PlateColor.CommonPlate;
            }
        }
        public double Volume => Dimensions.Volume;
        public double Weight => CalculateWeight();
        private double CalculateWeight()
        {
            // Business rule: Steel approximation for weight calculation
            const double steelDensity = 7.85; // g/cm^3
            var volumeInCm3 = Volume / 1000; // Convert mm^3 to cm^3
            return volumeInCm3 * steelDensity;
        }
        public bool IsThickerThan(Plate other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return Dimensions.Thickness > other.Dimensions.Thickness;
        }
        public Plate WithDimension(Dimensions newDimensions)
        {
            return new Plate(Name, newDimensions, Type, Material, Hardness);
        }
        public override string ToString()
        {
            return $"{Type} '{Name}' - {Dimensions}";
        }
    }
}
