using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.Enums;
using ToolingStructureCreation.Domain.ValueObjects;

namespace ToolingStructureCreation.Domain.Entities
{
    public class Shoe
    {
        public string Name { get; }
        public Dimensions Dimensions { get; }
        public ShoeType Type { get; }
        public PlateColor Color { get; }
        public string Material { get; }
        public Shoe(string name, Dimensions dimensions, ShoeType type, string material = "S50C")
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Shoe name cannot be null or empty.", nameof(name));

            Name = name.Trim();
            Dimensions = dimensions ?? throw new ArgumentNullException(nameof(dimensions));
            Type = type;
            Color = DetermineColor(type);
            Material = string.IsNullOrWhiteSpace(material) ? "S50C" : material.Trim();

            ValidateBusinessRules();
        }
        private void ValidateBusinessRules()
        {
            // Business rule: Shoe thickness constraints
            if (Dimensions.Thickness < 30 || Dimensions.Thickness > 150)
                throw new ArgumentException($"Shoe thickness must be between 30-150mm, got {Dimensions.Thickness}mm");

            // Business rule: Minimum shoe dimensions for structural integrity
            if (Dimensions.Length < 100)
                throw new ArgumentException($"Shoe length must be at least 100mm, got {Dimensions.Length}mm");

            if (Dimensions.Width < 50)
                throw new ArgumentException($"Shoe width must be at least 50mm, got {Dimensions.Width}mm");

            // Business rule: Maximum reasonable shoe dimensions
            if (Dimensions.Length > 3000)
                throw new ArgumentException($"Shoe length cannot exceed 3000mm, got {Dimensions.Length}mm");

            if (Dimensions.Width > 1500)
                throw new ArgumentException($"Shoe width cannot exceed 1500mm, got {Dimensions.Width}mm");
        }
        private static PlateColor DetermineColor(ShoeType type)
        {
            switch (type)
            {
                case ShoeType.Upper:
                    return (PlateColor)127; // UPPERSHOE from your original enum
                case ShoeType.Lower:
                    return (PlateColor)60;  // LOWERSHOE from your original enum
                default:
                    return PlateColor.CommonPlate;
            }
        }
        public double Volume => Dimensions.Volume;

        public double Weight => CalculateWeight();

        private double CalculateWeight()
        {
            // Business rule: Steel density for shoe weight calculation
            const double steelDensity = 7.85; // g/cm³
            var volumeInCm3 = Volume / 1000; // Convert mm³ to cm³
            return volumeInCm3 * steelDensity; // Weight in grams
        }

        public bool IsUpperShoe => Type == ShoeType.Upper;

        public bool IsLowerShoe => Type == ShoeType.Lower;
        public bool CanAccommodatePlate(Plate plate)
        {
            if (plate == null)
                throw new ArgumentNullException(nameof(plate));

            // Business rule: Shoe must be larger than plates it contains
            return Dimensions.Length >= plate.Dimensions.Length &&
                   Dimensions.Width >= plate.Dimensions.Width;
        }

        public Shoe WithDimensions(Dimensions newDimensions)
        {
            return new Shoe(Name, newDimensions, Type, Material);
        }

        public override string ToString()
        {
            return $"{Type} Shoe '{Name}' - {Dimensions}";
        }

    }
}
