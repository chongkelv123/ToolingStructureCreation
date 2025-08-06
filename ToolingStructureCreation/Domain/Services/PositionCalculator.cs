using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.Aggregates;
using ToolingStructureCreation.Domain.Enums;
using ToolingStructureCreation.Domain.ValueObjects;

namespace ToolingStructureCreation.Domain.Services
{
    public class PositionCalculator
    {
        private readonly PlateThicknessCalculator _thicknessCalculator;
        private readonly double _parallelBarThickness;
        private readonly double _commonPlateThickness;

        public PositionCalculator(PlateThicknessCalculator plateThicknessCalculator, double parallelBarThickness, double commonPlateThickness)
        {
            _thicknessCalculator = plateThicknessCalculator ?? throw new ArgumentNullException(nameof(plateThicknessCalculator));
            _parallelBarThickness = parallelBarThickness;
            _commonPlateThickness = commonPlateThickness;
        }
        public Position3D CalculateUpperShoePosition(Position3D basePosition)
        {
            if (basePosition == null)
                throw new ArgumentNullException(nameof(basePosition));
            var zOffset = _thicknessCalculator.GetTotalDieHeight() - _thicknessCalculator.GetLowerDieSetThickness();
            return basePosition.WithZ(zOffset);
        }
        public Position3D CalculateLowerShoePosition(Position3D basePosition)
        {
            if (basePosition == null)
                throw new ArgumentNullException(nameof(basePosition));
            var zOffset = -_thicknessCalculator.GetLowerDieSetThickness();
            return basePosition.WithZ(zOffset);
        }

        public Position3D CalculateParallelBarPosition(Position3D basePosition)
        {
            if (basePosition == null)
                throw new ArgumentNullException(nameof(basePosition));
            var zOffest = -(_thicknessCalculator.GetLowerDieSetThickness() + _parallelBarThickness);
            return basePosition.WithZ(zOffest);
        }

        public Position3D CalculateCommonPlatePosition(Position3D basePosition)
        {
            if (basePosition == null)
                throw new ArgumentNullException(nameof(basePosition));

            var zOffet = -(_thicknessCalculator.GetLowerDieSetThickness() +
                _parallelBarThickness + _commonPlateThickness);
            return basePosition.WithZ(zOffet);
        }

        public Position3D CalculateStationAssemblyPosition(Position3D basePosition)
        {
            if (basePosition == null)
                throw new ArgumentNullException(nameof(basePosition));
            var zOffset = _thicknessCalculator.GetLowerDieSetThickness();
            return basePosition.WithZ(zOffset);
        }

        public List<ParallelBarPlacement> CalculateParallelBarPlacements(Position3D startPosition, double toolLength, double parallelBarWidth = 60.0)
        {
            if (startPosition == null)
                throw new ArgumentNullException(nameof(startPosition));
            var placements = new List<ParallelBarPlacement>();
            var baseZ = CalculateParallelBarPosition(startPosition);

            // Business rule: Parallel bar positioning
            var firstBarX = startPosition.X + (parallelBarWidth / 2.0);
            var lastBarX = startPosition.X + toolLength - (parallelBarWidth / 2.0);
            var distanceBetweenEnds = lastBarX - firstBarX;

            const double standardSpacing = 330.0;   // Minimum two bars
            var numberOfBars = (int)Math.Ceiling(distanceBetweenEnds / standardSpacing) + 1;

            if (numberOfBars < 2)
                numberOfBars = 2;   // Minimum two bars

            // Always place bar at start and end
            placements.Add(new ParallelBarPlacement(new Position3D(firstBarX, startPosition.Y, baseZ.Z), 1));
            placements.Add(new ParallelBarPlacement(new Position3D(lastBarX, startPosition.Y, baseZ.Z), 1));

            // Place intermediate bars if needed
            if (numberOfBars > 2)
            {
                var spacing = distanceBetweenEnds / (numberOfBars - 1);
                for (int i = 1; i < numberOfBars - 1; i++)
                {
                    var xPosition = firstBarX + (i * spacing);
                    placements.Add(new ParallelBarPlacement(
                        new Position3D(xPosition, startPosition.Y, baseZ.Z), i + 1));
                }
            }
            return placements.OrderBy(p => p.Position.X).ToList();
        }

        public double CalculateFeedHeight(double liftHeight)
        {
            var lowerDieSetThickness = _thicknessCalculator.GetLowerDieSetThickness();
            return liftHeight + lowerDieSetThickness + _parallelBarThickness + _commonPlateThickness;
        }

        public List<PlatePosition> CalculatePlatePositions(Position3D basePosition)
        {
            if (basePosition == null)
                throw new ArgumentNullException(nameof(basePosition));

            var position = new List<PlatePosition>();
            var plateOrder = new[]
            {
                PlateType.Lower_Pad,
                PlateType.Die_Plate,
                PlateType.Stripper_Plate,
                PlateType.Bottoming_Plate,
                PlateType.Punch_Holder,
                PlateType.Upper_Pad
            };

            double cumulativeZ = 0;
            foreach (var plateType in plateOrder)
            {
                var thickness = _thicknessCalculator.GetCumulativeThicknessToPlate(plateType);
                cumulativeZ += thickness;

                position.Add(new PlatePosition(
                    plateType,
                    new Position3D(basePosition.X, basePosition.Y, cumulativeZ)));
            }

            return position;
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
