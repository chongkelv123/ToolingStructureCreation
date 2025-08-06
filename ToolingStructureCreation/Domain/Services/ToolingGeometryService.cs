using NXOpen.Assemblies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.Aggregates;
using ToolingStructureCreation.Domain.Entities;
using ToolingStructureCreation.Domain.Enums;
using ToolingStructureCreation.Domain.ValueObjects;

namespace ToolingStructureCreation.Domain.Services
{
    public class ToolingGeometryService
    {
        public SketchValidationResult ValidateRectangularSketch(List<Position3D> sketchPoints)
        {
            if (sketchPoints == null || sketchPoints.Count != 4)
                return SketchValidationResult.Invalid("Sketch must contain exactly 4 points for a rectangle.");

            // Sort points to find bounding rectangle
            var sortedByX = sketchPoints.OrderBy(p => p.X).ToList();
            var sortedByY = sketchPoints.OrderBy(p => p.Y).ToList();
            var minX = sortedByX.First().X;
            var maxX = sortedByX.Last().X;
            var minY = sortedByY.First().Y;
            var maxY = sortedByY.Last().Y;

            // Validate we have a proper rectangle (not degenerate)
            var length = Math.Abs(maxX - minX);
            var width = Math.Abs(maxY - minY);
            if (length < 0.1 || width < 0.1)
                return SketchValidationResult.Invalid("Rectangle dimensions too small (minimum 0.1mm).");

            try
            {
                var dimensions = new Dimensions(length, width, 1.0); // Thickness will be set later
                var midY = (minY + maxY) / 2.0;
                var startLocation = new Position3D(minX, midY, sketchPoints.First().Z);
                var midX = (minX + maxX) / 2.0;
                var midPoint = new Position3D(midX, midY, sketchPoints.First().Z);
                var sketchInfo = new SketchGeometry(dimensions, startLocation, midPoint);
                return SketchValidationResult.Valid(sketchInfo);
            }
            catch (ArgumentException ex)
            {
                return SketchValidationResult.Invalid($"Invalid rectangle: {ex.Message}");
            }
        }

        public List<SketchGeometry> SortSketchesByStartLocation(List<SketchGeometry> sketches)
        {
            if (sketches == null || !sketches.Any())
                return new List<SketchGeometry>();

            return sketches.OrderBy(s => s.StartLocation.X).ToList();
        }

        public bool CanPlatesFitInShoe(Shoe shoe, List<Plate> plates)
        {
            if (shoe == null)
                throw new ArgumentNullException(nameof(shoe));

            if (plates == null || !plates.Any())
                return true;

            // Business rule: All plates must fit within shoe dimensions
            return plates.All(plate => shoe.CanAccommodatePlate(plate));
        }

        public ClearanceAnalysis AnalyzeClearances(List<Plate> plates)
        {
            if (plates == null || !plates.Any())
                return new ClearanceAnalysis(true, "No plates to analyze");

            var issues = new List<string>();

            // Business rule: Check for thickness progression (should generally decrease going down)
            var platesByZ = plates.OrderBy(p => p.Dimensions.Thickness).ToList();

            // Check for reasonable thickness ratios
            for (int i = 1; i < platesByZ.Count; i++)
            {
                var current = platesByZ[i];
                var previous = platesByZ[i - 1];

                var ratio = current.Dimensions.Thickness / previous.Dimensions.Thickness;
                if (ratio > 3.0) // Thickness jumped more than 3x
                {
                    issues.Add($"Large thickness jump from {previous.Name} ({previous.Dimensions.Thickness:F1}mm) to {current.Name} ({current.Dimensions.Thickness:F1}mm)");
                }
            }

            // Business rule: Check for minimum clearances between operational plates
            var punchHolder = plates.FirstOrDefault(p => p.Type == PlateType.PunchHolder);
            var stripperPlate = plates.FirstOrDefault(p => p.Type == PlateType.StripperPlate);

            if (punchHolder != null && stripperPlate != null)
            {
                // Stripper should be thinner than punch holder for proper operation
                if (stripperPlate.Dimensions.Thickness >= punchHolder.Dimensions.Thickness)
                {
                    issues.Add("Stripper plate should typically be thinner than punch holder for proper operation");
                }
            }

            var hasIssues = issues.Any();
            var summary = hasIssues ? string.Join("; ", issues) : "All clearances within acceptable ranges";

            return new ClearanceAnalysis(!hasIssues, summary);
        }

        public double CalculateStripLength(List<SketchGeometry> stationSketches)
        {
            if (stationSketches == null || !stationSketches.Any())
                return 0;

            var sortedSketches = SortSketchesByStartLocation(stationSketches);
            var firstSketch = sortedSketches.First();
            var lastSketch = sortedSketches.Last();

            // Business rule: Strip length from start of first station to end of last station
            var startX = firstSketch.StartLocation.X;
            var endX = lastSketch.StartLocation.X + lastSketch.Dimensions.Length;

            return Math.Abs(endX - startX);
        }

        public Position3D CalculateSingleCommonPlatePosition(SketchGeometry shoeSketch, PositionCalculator positionCalculator)
        {
            if (shoeSketch == null)
                throw new ArgumentNullException(nameof(shoeSketch));
            if (positionCalculator == null)
                throw new ArgumentNullException(nameof(positionCalculator));

            // Original logic: X,Y from shoe sketch midpoint, Z from position calculator
            var xyPosition = new Position3D(shoeSketch.MidPoint.X, 0, 0);
            return positionCalculator.CalculateCommonPlatePosition(xyPosition);
        }

        public Position3D CalculateDoubleJointCommonPlatePosition(SketchGeometry commonPlateSketch, PositionCalculator positionCalculator)
        {
            if (commonPlateSketch == null)
                throw new ArgumentNullException(nameof(commonPlateSketch));
            if (positionCalculator == null)
                throw new ArgumentNullException(nameof(positionCalculator));

            // Original logic: X,Y from common plate sketch midpoint, Z from position calculator  
            var xyPosition = new Position3D(commonPlateSketch.MidPoint.X, 0, 0);
            return positionCalculator.CalculateCommonPlatePosition(xyPosition);
        }

        public List<CommonPlatePositioning> CalculateCommonPlatePositions(SketchGeometry shoeSketch,
            PositionCalculator positionCalculator, List<SketchGeometry> commonPlateSketchesOrNull = null)
        {
            if (shoeSketch == null)
                throw new ArgumentNullException(nameof(shoeSketch));
            if (positionCalculator == null)
                throw new ArgumentNullException(nameof(positionCalculator));

            var positions = new List<CommonPlatePositioning>();

            // Original logic: If no common plate sketches selected, use single common plate
            if (commonPlateSketchesOrNull == null || !commonPlateSketchesOrNull.Any())
            {
                var singlePosition = CalculateSingleCommonPlatePosition(shoeSketch, positionCalculator);
                positions.Add(new CommonPlatePositioning(
                    CommonPlateType.Single,
                    singlePosition,
                    1,
                    "Single common plate at shoe center"));
            }
            else
            {
                // Original logic: Double joint - each common plate at its own sketch position
                for (int i = 0; i < commonPlateSketchesOrNull.Count; i++)
                {
                    var commonPlateSketch = commonPlateSketchesOrNull[i];
                    var plateType = i == 0 ? CommonPlateType.DoubleLeft : CommonPlateType.DoubleRight;
                    var position = CalculateDoubleJointCommonPlatePosition(commonPlateSketch, positionCalculator);

                    positions.Add(new CommonPlatePositioning(
                        plateType,
                        position,
                        i + 1,
                        $"Double joint plate {i + 1} at own sketch center"));
                }
            }

            return positions;
        }

    }
}
