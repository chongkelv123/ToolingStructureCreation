using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Domain.Services
{
    public class ToolingThicknessSummary
    {
        public double TotalDieHeight { get; }
        public double UpperDieSetThickness { get; }
        public double LowerDieSetThickness { get; }
        public double PunchActiveLength { get; }
        public double PenetrationDepth { get; }
        public double MaterialThickness { get; }

        public ToolingThicknessSummary(double totalDieHeight, double upperDieSetThickness, double lowerDieSetThickness, double punchActiveLength, double penetrationDepth, double materialThickness)
        {
            TotalDieHeight = totalDieHeight;
            UpperDieSetThickness = upperDieSetThickness;
            LowerDieSetThickness = lowerDieSetThickness;
            PunchActiveLength = punchActiveLength;
            PenetrationDepth = penetrationDepth;
            MaterialThickness = materialThickness;
        }
        public override string ToString()
        {
            return $"Die Height: {TotalDieHeight:F1}mm, Punch Length: {PunchActiveLength:F0}mm, Penetration: {PenetrationDepth:F1}mm";
        }
    }
}
