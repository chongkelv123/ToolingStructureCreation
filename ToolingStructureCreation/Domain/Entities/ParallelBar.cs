using NXOpen.CAM;
using NXOpen.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.Services;
using ToolingStructureCreation.Domain.ValueObjects;

namespace ToolingStructureCreation.Domain.Entities
{
    public class ParallelBar
    {
        public string Name { get; }
        public Dimensions Dimensions { get; }
        public string Material { get; }
        public int Quantity { get; }

        public ParallelBar(string name, Dimensions dimensions, string material = "S50C", int quantity = 1)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("ParallelBar name cannot be null or empty.", nameof(name));

            Name = name.Trim();
            Dimensions = dimensions ?? throw new ArgumentNullException(nameof(dimensions));
            Material = string.IsNullOrWhiteSpace(material) ? "S50C" : material.Trim();
            Quantity = quantity;

            ValidateBusinessRules();
        }

        private void ValidateBusinessRules()
        {
            // Business rule: Parallel bar thickness constraints
            if (Dimensions.Thickness < 50 || Dimensions.Thickness > 200)
                throw new ArgumentException($"Parallel bar thickness must be between 50-200mm, got {Dimensions.Thickness}mm");

            // Business rule: Standard parallel bar width
            if (Dimensions.Length != 60.0)
                throw new ArgumentException($"Standard parallel bar width must be 60mm, got {Dimensions.Length}mm");

            // Business rule: Minimum width for structural stability
            if (Dimensions.Width < 100)
                throw new ArgumentException($"Parallel bar width must be at least 100mm, got {Dimensions.Width}mm");

            // Business rule: Maximum reasonable width
            if (Dimensions.Width > 2000)
                throw new ArgumentException($"Parallel bar width cannot exceed 2000mm, got {Dimensions.Width}mm");

            // Business rule: Quantity must be positive
            if (Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(Quantity));
        }
        public double Volume => Dimensions.Volume;
        public double TotalVolume => Volume * Quantity;
        public double Weight => CalculateWeight();
        public double TotalWeight => Weight * Quantity;
        private double CalculateWeight()
        {
            // Business rule: Steel density for parallel bar weight calculation
            const double steelDensity = 7.85; // g/cm³
            var volumeInCm3 = Volume / 1000; // Convert mm³ to cm³
            return volumeInCm3 * steelDensity; // Weight in grams
        }

        public static ParallelBar CreateFromShoeSketch(string name, SketchGeometry shoeSketch, double thickness, int quantity = 1)
        {
            if (shoeSketch == null)
                throw new ArgumentNullException(nameof(shoeSketch));

            // Business rule: Length derived from shoe sketch width minus clearance
            var parallelBarLength = shoeSketch.Dimensions.Width - 85.0;

            // Create dimensions based on the shoe sketch
            var dimensions = new Dimensions(
                60.0,                    // Fixed width
                parallelBarLength,       // Calculated length
                thickness               // User specified thickness
                );

            return new ParallelBar(name, dimensions, "S50C", quantity);
        }

        // ADD: Parallel bar spacing business rule
        public static double GetStandardSpacing() => 330.0;

        public bool CanSupportLoad(double loadInKg)
        {
            // Business rule: Basic load capacity estimation
            // Simplified calculation based on cross-sectional area
            var crossSectionalArea = Dimensions.Length * Dimensions.Thickness; // mm²
            var safeLoadCapacity = crossSectionalArea * 0.1; // Conservative estimate: 0.1 kg/mm²

            return loadInKg <= safeLoadCapacity;
        }
        public ParallelBar WithQuantity(int newQuantity)
        {
            return new ParallelBar(Name, Dimensions, Material, newQuantity);
        }
        public ParallelBar WithDimensions(Dimensions newDimensions)
        {
            return new ParallelBar(Name, newDimensions, Material, Quantity);
        }
        public override string ToString()
        {
            return $"ParallelBar '{Name} - {Dimensions} x{Quantity}";
        }
    }
}
