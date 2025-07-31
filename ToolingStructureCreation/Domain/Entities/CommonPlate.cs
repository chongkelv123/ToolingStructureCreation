using NXOpen.Layout2d;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.Enums;
using ToolingStructureCreation.Domain.ValueObjects;

namespace ToolingStructureCreation.Domain.Entities
{
    public class CommonPlate
    {
        public string Name { get; }
        public Dimensions Dimensions { get; }
        public CommonPlateType Type { get; }
        public string Material { get; }
        public MachineSpecification MachineSpec { get; }

        public CommonPlate(string name, Dimensions dimensions, CommonPlateType type, MachineSpecification machineSpec, string material = "S50C")
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("CommonPlate name cannot be null or empty.", nameof(name));

            Name = name.Trim();
            Dimensions = dimensions ?? throw new ArgumentNullException(nameof(dimensions));
            Type = type;
            MachineSpec = machineSpec ?? throw new ArgumentNullException(nameof(machineSpec));
            Material = string.IsNullOrWhiteSpace(material) ? "S50C" : material.Trim();

            ValidateBusinessRules();
        }
        private void ValidateBusinessRules()
        {
            // Business rule: Common plate thickness constraints
            if (Dimensions.Thickness < 35 || Dimensions.Thickness > 100)
                throw new ArgumentException($"Common plate thickness must be between 35-100mm, got {Dimensions.Thickness}mm");

            // Business rule: Double joint validation
            if (Type != CommonPlateType.Single && !MachineSpec.SupportsDoubleJoint)
                throw new ArgumentException($"Machine {MachineSpec.MachineName} does not support double joint common plates");

            // Business rule: Plate must fit machine specifications
            if (!MachineSpec.CanAccommodateTool(Dimensions))
                throw new ArgumentException($"Common plate dimensions {Dimensions} exceed machine {MachineSpec.MachineName} capacity");

            // Business rule: Double plates should have reasonable dimensions
            if (Type != CommonPlateType.Single)
            {
                if (Dimensions.Length < 500)
                    throw new ArgumentException("Double joint common plates must be at least 500mm long");
            }
        }
        public double Volume => Dimensions.Volume;
        public double Weight => CalculateWeight();
        private double CalculateWeight()
        {
            // Business rule: Steel density for common plate weight calculation
            const double steelDensity = 7.85; // g/cm³
            var volumeInCm3 = Volume / 1000; // Convert mm³ to cm³
            return volumeInCm3 * steelDensity; // Weight in grams
        }
        public bool IsSinglePlate => Type == CommonPlateType.Single;
        public bool IsDoubleJointPlate => Type == CommonPlateType.DoubleLeft || Type == CommonPlateType.DoubleRight;
        public bool IsLeftPlate => Type == CommonPlateType.DoubleLeft;
        public bool IsRightPlate => Type == CommonPlateType.DoubleRight;
        public bool CanSupportMachine(MachineSpecification machineSpec)
        {
            if(machineSpec == null)
                throw new ArgumentNullException(nameof(machineSpec));

            return machineSpec.CanAccommodateTool(Dimensions);
        }
        public CommonPlate WithDimensions(Dimensions newDimensions)
        {
            return new CommonPlate(Name, newDimensions, Type, MachineSpec, Material);
        }
        public static CommonPlate CreateSingle(string name, MachineSpecification machineSpec, double? customThickness = null)
        {
            var thickness = customThickness ?? machineSpec.CommonPlateDimensions.Thickness;
            var dimensions = new Dimensions(
                machineSpec.CommonPlateDimensions.Length,
                machineSpec.CommonPlateDimensions.Width,
                thickness);

            return new CommonPlate(name, dimensions, CommonPlateType.Single, machineSpec);
        }
        public static CommonPlate CreateDoubleLeft(string name, Dimensions dimensions, MachineSpecification machineSpec)
        {
            return new CommonPlate(name, dimensions, CommonPlateType.DoubleLeft, machineSpec);
        }

        public static CommonPlate CreateDoubleRight(string name, Dimensions dimensions, MachineSpecification machineSpec)
        {
            return new CommonPlate(name, dimensions, CommonPlateType.DoubleRight, machineSpec);
        }

        public override string ToString()
        {
            return $"{Type} CommonPlate '{Name}' - {Dimensions} for {MachineSpec.MachineName}";
        }
    }

}
