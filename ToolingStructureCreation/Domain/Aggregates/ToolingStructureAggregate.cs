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
        private readonly ToolingParameters _toolingParameters;

        public DrawingCode BaseDrawingCode { get; }
        public MachineSpecification MachineSpec { get; }
        public double MaterialThickness { get; }
        public string ProjectName { get; }
        public string Designer { get; }

        public IReadOnlyList<StationAggregate> Stations => _stations.AsReadOnly();
        public IReadOnlyList<Shoe> Shoes => _shoes.AsReadOnly();
        public IReadOnlyList<ParallelBar> ParallelBars => _parallelBars.AsReadOnly();
        public IReadOnlyList<CommonPlate> CommonPlates => _commonPlates.AsReadOnly();

        public ToolingStructureAggregate(ToolingParameters parameters)
        {
            BaseDrawingCode = parameters.BaseDrawingCode ?? throw new ArgumentNullException(nameof(parameters.BaseDrawingCode));
            MachineSpec = parameters.MachineSpec ?? throw new ArgumentNullException(nameof(parameters.MachineSpec));
            MaterialThickness = parameters.MaterialThickness <= 0 
                ? throw new ArgumentException("Material thickness must be more than zero.", nameof(parameters.MaterialThickness)) 
                : parameters.MaterialThickness;
            ProjectName = string.IsNullOrWhiteSpace(parameters.ProjectName) 
                ? throw new ArgumentException("Project name can not be empty.",nameof(parameters.ProjectName)) 
                : parameters.ProjectName.Trim();
            Designer = string.IsNullOrWhiteSpace(parameters.Designer) 
                ? throw new ArgumentException("Designer can not empty.", nameof(parameters.Designer)) 
                : parameters.Designer.Trim();

            _stations = new List<StationAggregate>();
            _shoes = new List<Shoe>();
            _parallelBars = new List<ParallelBar>();
            _commonPlates = new List<CommonPlate>();

            // Store parameters for later use
            _toolingParameters = parameters;
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

        public void GenerateCompleteToolingStructure(List<SketchGeometry> stationSketches,
    List<SketchGeometry> shoeSketches, List<SketchGeometry> commonPlateSketches = null)
        {
            if (stationSketches == null || !stationSketches.Any())
                throw new ArgumentException("At least one station sketch is required.");

            if (shoeSketches == null || !shoeSketches.Any())
                throw new ArgumentException("At least one shoe sketch is required.");

            // Generate stations with user-specified thicknesses
            GenerateStations(stationSketches);

            // Generate shoes with user-specified thicknesses
            GenerateShoes(shoeSketches);

            // Generate parallel bars with user-specified thickness
            GenerateParallelBars(shoeSketches);

            // Generate common plates with user-specified thickness
            GenerateCommonPlates(shoeSketches, commonPlateSketches);
        }

        private void GenerateCommonPlates(List<SketchGeometry> shoeSketches, List<SketchGeometry> commonPlateSketches)
        {
            if (commonPlateSketches == null || !commonPlateSketches.Any())
            {
                // Single common plate using form-specified thickness
                var commonPlate = new CommonPlate(
                    "LOWER_COMMON_PLATE",
                    new Dimensions(
                        MachineSpec.CommonPlateDimensions.Length,
                        MachineSpec.CommonPlateDimensions.Width,
                        _toolingParameters.CommonPlateThickness
                    ),
                    CommonPlateType.Single,
                    MachineSpec
                );
                AddCommonPlate(commonPlate);
            }
            else
            {
                // Double joint common plates using form-specified thickness
                for (int i = 0; i < commonPlateSketches.Count; i++)
                {
                    var plateType = i == 0 ? CommonPlateType.DoubleLeft : CommonPlateType.DoubleRight;
                    var sketch = commonPlateSketches[i];

                    var commonPlate = new CommonPlate(
                        $"LOWER_COMMON_PLATE-{i + 1}",
                        new Dimensions(
                            sketch.Dimensions.Length,
                            sketch.Dimensions.Width,
                            _toolingParameters.CommonPlateThickness
                        ),
                        plateType,
                        MachineSpec
                    );
                    AddCommonPlate(commonPlate);
                }
            }
        }

        private void GenerateStations(List<SketchGeometry> stationSketches)
        {
            var geometryService = new ToolingGeometryService();
            var thicknessCalculator = new PlateThicknessCalculator(_toolingParameters.PlateThicknesses, _toolingParameters.MaterialThickness);

            for (int i = 0; i < stationSketches.Count; i++)
            {
                var station = new StationAggregate(i + 1, stationSketches[i], geometryService);

                // Create plates using form-specified thicknesses
                var plates = CreateCompleteePlateSet(stationSketches[i], i + 1, _toolingParameters.PlateThicknesses);
                foreach (var plate in plates)
                {
                    station.AddPlate(plate);
                }

                AddStation(station);
            }
        }

        private void GenerateShoes(List<SketchGeometry> shoeSketches)
        {
            for (int i = 0; i < shoeSketches.Count; i++)
            {
                var shoeSketch = shoeSketches[i];

                // Use form-specified shoe thicknesses
                var upperShoe = new Shoe(
                    $"UPPER_SHOE-{i + 1}",
                    new Dimensions(shoeSketch.Dimensions.Length, shoeSketch.Dimensions.Width, _toolingParameters.UpperShoeThickness),
                    ShoeType.Upper
                );

                var lowerShoe = new Shoe(
                    $"LOWER_SHOE-{i + 1}",
                    new Dimensions(shoeSketch.Dimensions.Length, shoeSketch.Dimensions.Width, _toolingParameters.LowerShoeThickness),
                    ShoeType.Lower
                );

                AddShoe(upperShoe);
                AddShoe(lowerShoe);
            }
        }

        private void GenerateParallelBars(List<SketchGeometry> shoeSketches)
        {
            for (int i = 0; i < shoeSketches.Count; i++)
            {
                var shoeSketch = shoeSketches[i];

                // Use form-specified parallel bar thickness
                var parallelBar = ParallelBar.CreateFromShoeSketch(
                    $"PARALLEL_BAR-{i + 1}",
                    shoeSketch,
                    _toolingParameters.ParallelBarThickness
                );

                AddParallelBar(parallelBar);
            }
        }

        private List<Plate> CreateCompleteePlateSet(SketchGeometry stationSketch, int stationNumber, Dictionary<PlateType, double> plateThicknesses)
        {
            var plates = new List<Plate>();

            var plateTypes = new[]
            {
                PlateType.Upper_Pad,
                PlateType.Punch_Holder,
                PlateType.Bottoming_Plate,
                PlateType.Stripper_Plate,
                PlateType.Die_Plate,
                PlateType.Lower_Pad
            };

            foreach (var plateType in plateTypes)
            {
                // Use form-specified thickness, not hardcoded
                var thickness = plateThicknesses[plateType];
                var dimensions = new Dimensions(
                    stationSketch.Dimensions.Length,
                    stationSketch.Dimensions.Width,
                    thickness
                );

                var plateName = $"STN{stationNumber:D2}_{plateType}";
                plates.Add(new Plate(plateName, dimensions, plateType));
            }

            return plates;
        }

        public override string ToString()
        {
            return $"Tooling Structure '{ProjectName}' - {_stations.Count} stations, Machine: {MachineSpec.MachineName}";
        }
    }
}
