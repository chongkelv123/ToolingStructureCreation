using NXOpen.Layout2d;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Domain.ValueObjects
{
    public sealed class MachineSpecification : IEquatable<MachineSpecification>
    {
        public string MachineName { get; }
        public Dimensions CommonPlateDimensions { get; }
        public bool SupportsDoubleJoint { get; }
        public double MaxToolLength { get; }
        public double MaxToolWidth { get; }
        private static readonly Dictionary<string, MachineSpecification> Specifications =
            new Dictionary<string, MachineSpecification>
            {
                ["MC304"] = new MachineSpecification("MC304", new Dimensions(2100, 700, 40), false, 2100, 700),
                ["MC303"] = new MachineSpecification("MC303", new Dimensions(2100, 700, 40), false, 2100, 700),
                ["MC254"] = new MachineSpecification("MC254", new Dimensions(2100, 700, 40), false, 2100, 700),
                ["MC302"] = new MachineSpecification("MC302", new Dimensions(2100, 700, 40), false, 2100, 700),
                ["MC602"] = new MachineSpecification("MC602", new Dimensions(2300, 960, 60), false, 2300, 960),
                ["MC403"] = new MachineSpecification("MC403", new Dimensions(2300, 960, 60), false, 2300, 960),
                ["MC803"] = new MachineSpecification("MC803", new Dimensions(2300, 960, 60), false, 2300, 960),
                ["MC1801"] = new MachineSpecification("MC1801", new Dimensions(2600, 960, 60), true, 2600, 960),
                ["MC1202"] = new MachineSpecification("MC1202", new Dimensions(2600, 960, 60), true, 2600, 960)
            };
        private MachineSpecification(string machineName, Dimensions commonPlateDimensions, bool supportsDoubleJoint, double maxToolLength, double maxToolWidth)
        {
            MachineName = machineName ?? throw new ArgumentNullException(nameof(machineName));
            CommonPlateDimensions = commonPlateDimensions ?? throw new ArgumentNullException(nameof(commonPlateDimensions));
            SupportsDoubleJoint = supportsDoubleJoint;
            MaxToolLength = maxToolLength;
            MaxToolWidth = maxToolWidth;
        }
        public static MachineSpecification GetByName(string machineName)
        {
            if (string.IsNullOrWhiteSpace(machineName))
                throw new ArgumentException("Machine name cannot be null or empty.", nameof(machineName));
            if (!Specifications.TryGetValue(machineName.Trim(), out var specification))
                throw new ArgumentException($"Unknown machine: {machineName}");

            return specification;
        }
        public static IEnumerable<string> GetAvailableMachine()
        {
            return Specifications.Keys.ToList();
        }
        public bool CanAccommodateTool(Dimensions toolDimensions)
        {
            if (toolDimensions == null)
                throw new ArgumentNullException(nameof(toolDimensions));

            return toolDimensions.Length <= MaxToolLength &&
                toolDimensions.Width <= MaxToolWidth;
        }
        public bool IsLargeMachine => SupportsDoubleJoint;
        public bool Equals(MachineSpecification other)
        {
            if (other is null) return false;
            if (ReferenceEquals (this, other)) return true;

            return string.Equals(MachineName, other.MachineName, StringComparison.OrdinalIgnoreCase);
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as MachineSpecification);
        }
        public override int GetHashCode()
        {
            return MachineName?.GetHashCode() ?? 0;            
        }
        public static bool operator ==(MachineSpecification left, MachineSpecification right)
        {
            return Equals(left, right);
        }
        public static bool operator !=(MachineSpecification left, MachineSpecification right)
        {
            return !Equals(left, right);
        }
        public override string ToString()
        {
            return $"{MachineName} - Common Plate: {CommonPlateDimensions}";
        }
    }
}
