using NXOpen.Layout2d;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Domain.ValueObjects
{
    public sealed class Dimensions : IEquatable<Dimensions>
    {
        public double Length { get; }
        public double Width { get; }
        public double Thickness { get; }

        public Dimensions(double length, double width, double thickness)
        {
            if (length <= 0)
                throw new ArgumentException("Length must be greater than zero.", nameof(length));
            if (width <= 0)
                throw new ArgumentException("Width must be greater than zero.", nameof(width));
            if (thickness <= 0)
                throw new ArgumentException("Thickness must be greater than zero", nameof(thickness));

            // Business rule: Maximum reasonable dimension for stamping tools
            if (length > 5000)
                throw new ArgumentException("Length cannot exceed 5000mm for stamping tools.", nameof(length));
            if (width > 2000)
                throw new ArgumentException("Width cannot excced 2000mm for stamping tools.", nameof(width));
            if (thickness > 300)
                throw new ArgumentException("Thickness cannot exceed 300mm for stamping tools.", nameof(thickness));

            Length = Math.Round(length, 2);
            Width = Math.Round(width, 2);
            Thickness = Math.Round(thickness, 2);
        }

        public double Volume => Length * Width * Thickness;
        public double Area => Length * Width;
        public bool IsSquare => Math.Abs(Length - Width) < 0.01;

        public Dimensions ScaleBy(double factor)
        {
            if (factor <= 0)
                throw new ArgumentException("Scale factor must be greater than zero", nameof(factor));

            return new Dimensions(Length * factor, Width * factor, Thickness * factor);
        }

        public Dimensions WithThickness (double newThickness)
        {
            return new Dimensions(Length, Width, newThickness);
        }

        public bool Equals(Dimensions other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Math.Abs(Length - other.Length) < 0.01 &&
                Math.Abs(Width - other.Width) < 0.01 &&
                Math.Abs(Thickness - other.Thickness) < 0.01;
        }

        public override bool Equals (object obj)
        {
            return Equals(obj as Dimensions);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Length.GetHashCode();
                hashCode = (hashCode * 397) ^ Width.GetHashCode();
                hashCode = (hashCode * 397) ^ Thickness.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Dimensions left, Dimensions right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Dimensions left, Dimensions right) 
        { 
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"L:{Length:F1} * W:{Width:F1} * T:{Thickness:F2}";
        }
    }
}
