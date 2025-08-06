using NXOpen.Mechatronics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToolingStructureCreation.Domain.Entities;
using ToolingStructureCreation.Domain.Enums;
using ToolingStructureCreation.Domain.Services;
using ToolingStructureCreation.Domain.ValueObjects;
using ToolingStructureCreation.Model;

namespace ToolingStructureCreation.Domain.Aggregates
{
    public class ToolingStructureAggregate
    {
        private readonly List<StationAggregate> _stations;
        private readonly List<Shoe> _shoes;
        private readonly List<ParallelBar> _parallelBars;
        private readonly List<CommonPlate> _commonPlates;

        public DrawingCode BaseDrawingCode { get; }
        public MachineSpecification MachineSpec { get; }
        public double MaterialThickness { get; }
        public string ProjectName { get; }
        public string Designer { get; }

        public IReadOnlyList<StationAggregate> Stations => _stations.AsReadOnly();
        public IReadOnlyList<Shoe> Shoes => _shoes.AsReadOnly();
        public IReadOnlyList<ParallelBar> ParallelBars => _parallelBars.AsReadOnly();
        public IReadOnlyList<CommonPlate> CommonPlates => _commonPlates.AsReadOnly();

        public ToolingStructureAggregate(DrawingCode baseDrawingCode, MachineSpecification machineSpec,
            double materialThickness, string projectName, string designer)
        {
            BaseDrawingCode = baseDrawingCode ?? throw new ArgumentNullException(nameof(baseDrawingCode));
            MachineSpec = machineSpec ?? throw new ArgumentNullException(nameof(machineSpec));
            MaterialThickness = materialThickness > 0 ? materialThickness : throw new ArgumentException("Material thickness must be greater than zero.");
            ProjectName = string.IsNullOrWhiteSpace(projectName) ? throw new ArgumentException("Project name cannot be null or empty.") : projectName.Trim();
            Designer = string.IsNullOrWhiteSpace(designer) ? throw new ArgumentException("Designer cannot be null or empty.") : designer.Trim();

            _stations = new List<StationAggregate>();
            _shoes = new List<Shoe>();
            _parallelBars = new List<ParallelBar>();
            _commonPlates = new List<CommonPlate>();
        }

        public void AddStation(StationAggregate station)
        {
            if (station == null)
                throw new ArgumentNullException(nameof(station));

            // Business rule: No duplicate station numbers
            if (_stations.Any(s => s.StationNumber == station.StationNumber))
                throw new InvalidOperationException($"Station {station.StationNumber} already exists in tooling structure");

            // Business rule: Station geometry must fit within machine capabilities
            if (!MachineSpec.CanAccommodateTool(station.StationGeometry.Dimensions))
                throw new InvalidOperationException($"Station {station.StationNumber} dimensions {station.StationGeometry.Dimensions} exceed machine {MachineSpec.MachineName} capacity");

            _stations.Add(station);
        }

        public void AddShoe(Shoe shoe)
        {
            if (shoe == null)
                throw new ArgumentNullException(nameof(shoe));

            // Business rule: Validate shoe can accommodate all station plates
            var allPlates = _stations.SelectMany(s => s.Plates).ToList();
            if (allPlates.Any() && !allPlates.All(plate => shoe.CanAccommodatePlate(plate)))
                throw new InvalidOperationException($"Shoe {shoe.Name} cannot accommodate all station plates");

            _shoes.Add(shoe);
        }

        public void AddParallelBar(ParallelBar parallelBar)
        {
            if (parallelBar == null)
                throw new ArgumentNullException(nameof(parallelBar));

            _parallelBars.Add(parallelBar);
        }

        public void AddCommonPlate(CommonPlate commonPlate)
        {
            if (commonPlate == null)
                throw new ArgumentNullException(nameof(commonPlate));

            // Business rule: Common plate must fit within machine capabilities
            if (!commonPlate.CanSupportMachine(MachineSpec))
                throw new InvalidOperationException($"Common plate {commonPlate.Name} is not compatible with machine {MachineSpec.MachineName}");

            _commonPlates.Add(commonPlate);
        }

        public StationAggregate GetStation(int stationNumber)
        {
            return _stations.FirstOrDefault(s => s.StationNumber == stationNumber);
        }

        public Shoe GetShoe(ShoeType shoeType)
        {
            return _shoes.FirstOrDefault(s => s.Type == shoeType);
        }

        public double CalculateStripLength()
        {
            if (!_stations.Any())
                return 0;

            var geometries = _stations.Select(s => s.StationGeometry).ToList();
            var sortedStations = geometries.OrderBy(g => g.StartLocation.X).ToList();

            var firstStation = sortedStations.First();
            var lastStation = sortedStations.Last();

            var startX = firstStation.StartLocation.X;
            var endX = lastStation.StartLocation.X + lastStation.Dimensions.Length;

            return Math.Abs(endX - startX);
        }

        public Position3D CalculateSingleCommonPlatePosition(SketchGeometry shoeSketch, PositionCalculator positionCalculator)
        {
            if (shoeSketch == null)
                throw new ArgumentNullException(nameof(shoeSketch));
            if (positionCalculator == null)
                throw new ArgumentNullException(nameof(positionCalculator));

            var xyPosition = new Position3D(shoeSketch.MidPoint.X, 0, 0);
            return positionCalculator.CalculateCommonPlatePosition(xyPosition);
        }

        public Position3D CalculateDoubleJointCommonPlatePosition(SketchGeometry commonPlateSketch, PositionCalculator positionCalculator)
        {
            if (commonPlateSketch == null)
                throw new ArgumentNullException(nameof(commonPlateSketch));
            if (positionCalculator == null)
                throw new ArgumentNullException(nameof(positionCalculator));

            // Calculate the position based on the common plate sketch
            var xyPosition = new Position3D(commonPlateSketch.MidPoint.X, 0, 0);
            return positionCalculator.CalculateCommonPlatePosition(xyPosition);
        }

        public List<CommonPlatePositioning> CalculateCommonPlatePositions(SketchGeometry shoeSketch,
    PositionCalculator positionCalculator, List<SketchGeometry> commonPlateSketchesOrNull = null)
        {
            if (shoeSketch == null)
                throw new ArgumentNullException(nameof(shoeSketch));
            if ((positionCalculator == null)
                throw new ArgumentNullException(nameof(positionCalculator));

            var positions = new List<CommonPlatePositioning>();

            if (commonPlateSketchesOrNull == null || !commonPlateSketchesOrNull.Any())
            {
                // If no common plate sketches provided, calculate single common plate position
                var singlePosition = CalculateSingleCommonPlatePosition(shoeSketch, positionCalculator);
                positions.Add(new CommonPlatePositioning(
                    CommonPlateType.Single,
                    singlePosition,
                    1,
                    "Single common plate at shoe center"
                    ));
            }
            else
            {
                // Calculate positions for each common plate sketch
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

        public ToolingStructureValidationResult ValidateStructure()
        {
            var issues = new List<string>();

            // Validate minimum requirements
            if (!_stations.Any())
                issues.Add("Tooling structure must contain at least one station");

            if (!_shoes.Any())
                issues.Add("Tooling structure must contain at least one shoe");

            if (!_commonPlates.Any())
                issues.Add("Tooling structure must contain at least one common plate");

            // Validate shoe coverage
            var allPlates = _stations.SelectMany(s => s.Plates).ToList();
            foreach (var shoe in _shoes)
            {
                var incompatiblePlates = allPlates.Where(p => !shoe.CanAccommodatePlate(p)).ToList();
                if (incompatiblePlates.Any())
                {
                    issues.Add($"Shoe {shoe.Name} cannot accommodate plates: {string.Join(", ", incompatiblePlates.Select(p => p.Name))}");
                }
            }

            // Validate station integrity
            foreach (var station in _stations)
            {
                var stationAnalysis = station.ValidateStationIntegrity();
                if (!stationAnalysis.IsAcceptable)
                {
                    issues.Add($"Station {station.StationNumber}: {stationAnalysis.Summary}");
                }
            }

            var isValid = !issues.Any();
            var summary = isValid ? "Tooling structure validation passed" : $"Found {issues.Count} validation issues";

            return new ToolingStructureValidationResult(isValid, summary, issues);
        }

        public ToolingStructureSummary GetStructureSummary()
        {
            var stationCount = _stations.Count;
            var totalPlateCount = _stations.Sum(s => s.Plates.Count);
            var stripLength = CalculateStripLength();
            var isValid = ValidateStructure().IsValid;

            return new ToolingStructureSummary(
                ProjectName,
                Designer,
                BaseDrawingCode,
                MachineSpec.MachineName,
                stationCount,
                totalPlateCount,
                stripLength,
                MaterialThickness,
                isValid);
        }

        public override string ToString()
        {
            return $"Tooling Structure '{ProjectName}' - {_stations.Count} stations, Machine: {MachineSpec.MachineName}";
        }
    }
}
