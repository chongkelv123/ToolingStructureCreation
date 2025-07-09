using NXOpen;
using NXOpen.UF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Model
{
    public class Sketch
    {
        double length;
        double width;
        Point3d startLocation;
        Point3d midPoint;
        List<Point3d> pointCollections;

        bool showDebugMessage = false;

        public Sketch() { }

        public Sketch(double length, double width, Point3d startLocation, Point3d midPoint)
        {
            this.length = length;
            this.width = width;
            this.startLocation = startLocation;
            this.midPoint = midPoint;
        }

        public double Length => length;
        public double Width => width;
        public Point3d StartLocation => startLocation;
        public Point3d MidPoint => midPoint;
        //public List<Point3d> PointCollections => pointCollections;

        public bool IsRectangle(TaggedObject[] taggedObjs)
        {
            //System.Diagnostics.Debugger.Launch();
            // Check for exactly 4 curves
            if (taggedObjs == null || taggedObjs.Length != 4)
                return false;

            // Try to cast all to Line
            var lines = taggedObjs.Select(obj => obj as NXOpen.Line).ToArray();
            if (lines.Any(l => l == null))
                return false;

            // Collect endpoints
            var points = lines.SelectMany(l => new[] { l.StartPoint, l.EndPoint }).ToList();

            // Find unique points (should be 4 for a rectangle)
            var uniquePoints = points
                .GroupBy(p => new { 
                    X = Math.Round(p.X, 5),
                    Y = Math.Round(p.Y, 5), 
                    Z = Math.Round(p.Z, 5)
                })
                .Select(g => g.First())
                .ToList();

            if (uniquePoints.Count != 4)
                return false;

            // Check right angles at each corner
            for (int i = 0; i < 4; i++)
            {
                // Get three consecutive points
                var p0 = uniquePoints[i];
                var p1 = uniquePoints[(i + 1) % 4];
                var p2 = uniquePoints[(i + 2) % 4];

                // Vectors
                var v1 = new double[] { p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z };
                var v2 = new double[] { p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z };

                // Dot product should be close to zero for right angle
                double dot = v1[0] * v2[0] + v1[1] * v2[1] + v1[2] * v2[2];
                if (Math.Abs(dot) > 1e-6)
                    return false;
            }

            pointCollections = uniquePoints;

            if (showDebugMessage)
            {
                foreach (var p in pointCollections)
                {
                    Guide.InfoWriteLine($"Point: {p.X}, {p.Y}, {p.Z}");
                }
                Guide.InfoWriteLine("\n**********");
            }
            

            return true;
        }
    }
}
