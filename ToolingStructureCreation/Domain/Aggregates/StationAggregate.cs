using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.Entities;
using ToolingStructureCreation.Domain.Services;
using ToolingStructureCreation.Domain.ValueObjects;
using ToolingStructureCreation.Domain.Enums;

namespace ToolingStructureCreation.Domain.Aggregates
{
    public class StationAggregate
    {
        private readonly List<Plate> _plates;
        private readonly ToolingGeometryService _geometryService;

        public int StationNumber { get; }
        public SketchGeometry StationGeometry { get; }
        public IReadOnlyList<Plate> Plates => _plates.AsReadOnly();
        public string StationName => $"Station_{StationNumber:D2}";

        public StationAggregate(int stationNumber, SketchGeometry stationGeometry, ToolingGeometryService geometryService)
        {
            if (stationNumber <= 0)
                throw new ArgumentException("Station number must be greater than zero.", nameof(stationNumber));

            StationNumber = stationNumber;
            StationGeometry = stationGeometry ?? throw new ArgumentNullException(nameof(stationGeometry));
            _geometryService = geometryService ?? throw new ArgumentNullException(nameof(geometryService));
            _plates = new List<Plate>();
        }

        public void AddPlate(Plate plate)
        {
            if (plate == null)
                throw new ArgumentNullException(nameof(plate));

            // Business rule: Validate plate fits within station geometry
            if (!CanAccommodatePlate(plate))
                throw new InvalidOperationException($"Plate {plate.Name} with dimensions {plate.Dimensions} " +
                    $"cannot fit in station geometry {StationGeometry.Dimensions}");

            // Business rule: No duplicate plate types in same station
            if (_plates.Any(p => p.Type == plate.Type))
                throw new InvalidOperationException($"Station {StationNumber} already contains a plate of type {plate.Type}");

            _plates.Add(plate);
        }

        public void RemovePlate(PlateType plateType)
        {
            var plate = _plates.FirstOrDefault(p => p.Type == plateType);
            if(plate != null)
            {
                _plates.Remove(plate);
            }
        }

        public Plate GetPlate(PlateType plateType)
        {
            return _plates.FirstOrDefault(p => p.Type == plateType);
        }

        public bool HasPlate(PlateType plateType)
        {
            return _plates.Any(p => p.Type == plateType);
        }

        public bool CanAccommodatePlate(Plate plate)
        {
            if (plate == null)
                return false;

            // Business rule: Plate must fit within station footprint
            return plate.Dimensions.Length <= StationGeometry.Dimensions.Length &&
                plate.Dimensions.Width <= StationGeometry.Dimensions.Width;

        }

        public double GetTotalPlateThickness()
        {
            return _plates.Sum(p => p.Dimensions.Thickness);
        }

        public List<PlatePosition> CalculatePlatePosition(PlateThicknessCalculator thicknessCalculator)
        {
            if(thicknessCalculator == null)
                throw new ArgumentNullException(nameof(thicknessCalculator));

            var positions = new List<PlatePosition>();
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
            foreach( var plateType in plateOrder )
            {
                var plate = GetPlate(plateType);
                if (plate != null)
                {
                    var thickness = thicknessCalculator.GetCumulativeThicknessToPlate(plateType);
                    cumulativeZ += thickness;

                    positions.Add(new PlatePosition(
                        plateType,
                        new Position3D(
                            StationGeometry.StartLocation.X,
                            StationGeometry.StartLocation.Y,
                            cumulativeZ)));
                }
            }

            return positions;
        }

        public ClearanceAnalysis ValidateStationIntegrity()
        {
            return _geometryService.AnalyzeClearances(_plates);
        }

        public StationSummary GetStationSummary()
        {
            var totalThickness = GetTotalPlateThickness();
            var plateCount = _plates.Count;
            var hasAllRequirePlates = HasRequiredPlates();

            return new StationSummary(
                StationNumber,
                plateCount,
                totalThickness,
                hasAllRequirePlates,
                StationGeometry.Dimensions);
        }

        public bool HasRequiredPlates()
        {
            // Business rule: Minimum required plates for functional station
            var requiredPlates = new[] { PlateType.Die_Plate, PlateType.Stripper_Plate };
            return requiredPlates.All(HasPlate);
        }

        public override string ToString()
        {
            return $"Station {StationNumber}: {_plates.Count} plates, Geometry: {StationGeometry.Dimensions}";
        }
    }
}
