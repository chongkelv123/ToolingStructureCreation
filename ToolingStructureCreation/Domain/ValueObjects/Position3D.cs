using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Domain.ValueObjects
{
    public sealed class Position3D : IEquatable<Position3D>
    {
        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public Position3D(double x, double y, double z)
        {
            // Validate reasonable coordinate range for manufactoring
            if (Math.Abs(x) > 10000)
                throw new ArgumentException("X coordinate must be within ±10000mm range.", nameof(x));
            if (Math.Abs(y) > 10000)
                throw new ArgumentException("Y coordinate must be within ±10000mm range.", nameof(y));
            if (Math.Abs(z) > 10000)
                throw new ArgumentException("Z coordinate must be within ±10000mm range.", nameof(z));

            X = Math.Round(x, 3);
            Y = Math.Round(y, 3);
            Z = Math.Round(z, 3);
        }

        public static Position3D Origin => new Position3D(0, 0, 0);
        public Position3D Translate(double deltaX, double deltaY, double deltaZ)
        {
            return new Position3D(X + deltaX, Y + deltaY, Z + deltaZ);
        }
        public Position3D TranslateX(double deltaX)
        {
            return new Position3D(X + deltaX, Y, Z);
        }
        public Position3D TranslateZ(double deltaZ)
        {
            return new Position3D(X, Y, Z + deltaZ);
        }
        public Position3D WithZ(double newZ)
        {
            return new Position3D(X, Y, newZ);
        }
        public double DistanceTo(Position3D other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            var dx = X - other.X;
            var dy = Y - other.Y;
            var dz = Z - other.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
        public Position3D MidpointTo(Position3D other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return new Position3D(
                (X + other.X) / 2,
                (Y + other.Y) / 2,
                (Z + other.Z) / 2);
        }

        public bool Equals(Position3D other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return
                Math.Abs(X - other.X) < 0.001 &&
                Math.Abs(Y - other.Y) < 0.001 &&
                Math.Abs(Z - other.Z) < 0.001;
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as Position3D);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }
        public static bool operator ==(Position3D left, Position3D right)
        {
            return Equals(left, right);
        }
        public static bool operator !=(Position3D left, Position3D right)
        {
            return !Equals(left, right);
        }
        public override string ToString()
        {
            return $"{X:F3}, {Y:F3}, {Z:F3}";
        }
    }
}
