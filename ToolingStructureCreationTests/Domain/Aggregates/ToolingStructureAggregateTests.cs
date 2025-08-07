using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToolingStructureCreation.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.Entities;
using ToolingStructureCreation.Domain.Enums;
using ToolingStructureCreation.Domain.Services;
using ToolingStructureCreation.Domain.ValueObjects;
using static NXOpen.CAM.FBM.ThreadFeatureGeometry;

namespace ToolingStructureCreation.Domain.Aggregates.Tests
{
    [TestClass()]
    public class ToolingStructureAggregateTests
    {
        private DrawingCode _baseDrawingCode;
        private MachineSpecification _machineSpec;
        private ToolingStructureAggregate _toolingStructure;
        private StationAggregate _station1;
        private StationAggregate _station2;
        private Shoe _upperShoe;
        private Shoe _lowerShoe;
        private ParallelBar _parallelBar;
        private CommonPlate _commonPlate;
        private ToolingParameters toolingParameters;

        [TestInitialize()]
        public void Setup()
        {
            _baseDrawingCode = new DrawingCode("40XC00-2401-0000");
            _machineSpec = MachineSpecification.GetByName("MC304");
            var plateThicknesses = new Dictionary<PlateType, double>
            {
                { PlateType.Upper_Pad, 27.0 },
                { PlateType.Punch_Holder, 30.0 },
                { PlateType.Bottoming_Plate, 16.0 },
                { PlateType.Stripper_Plate, 30.0 },
                { PlateType.Die_Plate, 35.0 },
                { PlateType.Lower_Pad, 25.0 }
            };

            toolingParameters = new ToolingParameters(
                plateThicknesses,
                materialThickness: 1.55,
                upperShoeThickness: 70.0,
                lowerShoeThickness: 70.0,
                parallelBarThickness: 155.0,
                commonPlateThickness: 60.0,
                _machineSpec,
                _baseDrawingCode,
                projectName: "Project1",
                designer: "Designer1"
                );

            _toolingStructure = new ToolingStructureAggregate(toolingParameters);


            var geometryService = new ToolingGeometryService();
            var stationGeometry1 = new SketchGeometry(
                new Dimensions(100, 60, 2.0),
                new Position3D(0, 0, 0),
                new Position3D(50, 30, 0)
            );
            var stationGeometry2 = new SketchGeometry(
                new Dimensions(120, 70, 2.0),
                new Position3D(200, 0, 0),
                new Position3D(260, 35, 0)
            );

            _station1 = new StationAggregate(1, stationGeometry1, geometryService);
            _station2 = new StationAggregate(2, stationGeometry2, geometryService);

            _upperShoe = new Shoe("UPPER_SHOE", new Dimensions(300, 150, 70), ShoeType.Upper);
            _lowerShoe = new Shoe("LOWER_SHOE", new Dimensions(300, 150, 70), ShoeType.Lower);

            _parallelBar = new ParallelBar("PBAR_001", new Dimensions(60, 200, 155));
            _commonPlate = CommonPlate.CreateSingle("COMMON_PLATE", _machineSpec);
        }

        [TestMethod()]
        public void Constructor_WithValidParameters_CreatesToolingStructure()
        {
            // Act
            var tooling = new ToolingStructureAggregate(toolingParameters);

            // Assert
            Assert.AreEqual(_baseDrawingCode, tooling.BaseDrawingCode);
            Assert.AreEqual(_machineSpec, tooling.MachineSpec);
            Assert.AreEqual(1.55, tooling.MaterialThickness);
            Assert.AreEqual("Project1", tooling.ProjectName);
            Assert.AreEqual("Designer1", tooling.Designer);
            Assert.AreEqual(0, tooling.Stations.Count);
            Assert.AreEqual(0, tooling.Shoes.Count);
            Assert.AreEqual(0, tooling.ParallelBars.Count);
            Assert.AreEqual(0, tooling.CommonPlates.Count);
        }

        [TestMethod()]
        public void Constructor_WithNullDrawingCode_ThrowsException()
        {
            // Arrange
            var plateThicknesses = new Dictionary<PlateType, double>
            {
                { PlateType.Upper_Pad, 27.0 },
                { PlateType.Punch_Holder, 30.0 },
                { PlateType.Bottoming_Plate, 16.0 },
                { PlateType.Stripper_Plate, 30.0 },
                { PlateType.Die_Plate, 35.0 },
                { PlateType.Lower_Pad, 25.0 }
            };

            ToolingParameters toolingParameters = new ToolingParameters(
                plateThicknesses,
                materialThickness: 1.55,
                upperShoeThickness: 70.0,
                lowerShoeThickness: 70.0,
                parallelBarThickness: 155.0,
                commonPlateThickness: 60.0,
                _machineSpec,
                null,
                projectName: "CSRS400",
                designer: "Kelvin"
                );


            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
            new ToolingStructureAggregate(toolingParameters));

        }

        [TestMethod()]
        public void Constructor_WithNullMachineSpec_ThrowsException()
        {
            // Arrange
            var plateThicknesses = new Dictionary<PlateType, double>
            {
                { PlateType.Upper_Pad, 27.0 },
                { PlateType.Punch_Holder, 30.0 },
                { PlateType.Bottoming_Plate, 16.0 },
                { PlateType.Stripper_Plate, 30.0 },
                { PlateType.Die_Plate, 35.0 },
                { PlateType.Lower_Pad, 25.0 }
            };

            ToolingParameters toolingParameteres = new ToolingParameters(
                plateThicknesses,
                materialThickness: 1.55,
                upperShoeThickness: 70.0,
                lowerShoeThickness: 70.0,
                parallelBarThickness: 155.0,
                commonPlateThickness: 60.0,
                null,
                _baseDrawingCode,
                projectName: "CSRS400",
                designer: "Kelvin"
                );


            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ToolingStructureAggregate(toolingParameteres));
        }

        [TestMethod()]
        public void Constructor_WithZeroMaterialThickness_ThrowsException()
        {
            // Arrange
            var plateThicknesses = new Dictionary<PlateType, double>
            {
                { PlateType.Upper_Pad, 27.0 },
                { PlateType.Punch_Holder, 30.0 },
                { PlateType.Bottoming_Plate, 16.0 },
                { PlateType.Stripper_Plate, 30.0 },
                { PlateType.Die_Plate, 35.0 },
                { PlateType.Lower_Pad, 25.0 }
            };

            ToolingParameters toolingParameters = new ToolingParameters(
                plateThicknesses,
                materialThickness: 0.0,
                upperShoeThickness: 70.0,
                lowerShoeThickness: 70.0,
                parallelBarThickness: 155.0,
                commonPlateThickness: 60.0,
                _machineSpec,
                _baseDrawingCode,
                projectName: "Project1",
                designer: "Designer1"
                );

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                new ToolingStructureAggregate(toolingParameters));
        }

        [TestMethod()]
        public void Constructor_WithEmptyProjectName_ThrowsException()
        {
            // Arrange
            var plateThicknesses = new Dictionary<PlateType, double>
            {
                { PlateType.Upper_Pad, 27.0 },
                { PlateType.Punch_Holder, 30.0 },
                { PlateType.Bottoming_Plate, 16.0 },
                { PlateType.Stripper_Plate, 30.0 },
                { PlateType.Die_Plate, 35.0 },
                { PlateType.Lower_Pad, 25.0 }
            };

            ToolingParameters toolingParameters = new ToolingParameters(
                plateThicknesses,
                materialThickness: 1.55,
                upperShoeThickness: 70.0,
                lowerShoeThickness: 70.0,
                parallelBarThickness: 155.0,
                commonPlateThickness: 60.0,
                _machineSpec,
                _baseDrawingCode,
                projectName: "",
                designer: "Kelvin"
                );

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                new ToolingStructureAggregate(toolingParameters));
        }

        [TestMethod()]
        public void Constructor_WithEmptyDesigner_ThrowsException()
        {
            // Arrange
            // Arrange
            var plateThicknesses = new Dictionary<PlateType, double>
            {
                { PlateType.Upper_Pad, 27.0 },
                { PlateType.Punch_Holder, 30.0 },
                { PlateType.Bottoming_Plate, 16.0 },
                { PlateType.Stripper_Plate, 30.0 },
                { PlateType.Die_Plate, 35.0 },
                { PlateType.Lower_Pad, 25.0 }
            };

            ToolingParameters toolingParameters = new ToolingParameters(
                plateThicknesses,
                materialThickness: 1.55,
                upperShoeThickness: 70.0,
                lowerShoeThickness: 70.0,
                parallelBarThickness: 155.0,
                commonPlateThickness: 60.0,
                _machineSpec,
                _baseDrawingCode,
                projectName: "CSRS400",
                designer: ""
                );

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                new ToolingStructureAggregate(toolingParameters));
        }

        [TestMethod()]
        public void AddStation_WithValidStation_AddsStation()
        {
            // Act
            _toolingStructure.AddStation(_station1);

            // Assert
            Assert.AreEqual(1, _toolingStructure.Stations.Count);
            Assert.AreEqual(_station1, _toolingStructure.Stations.First());
        }

        [TestMethod()]
        public void AddStation_WithNullStation_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                _toolingStructure.AddStation(null));
        }

        [TestMethod()]
        public void AddStation_WithDuplicateStationNumber_ThrowsException()
        {
            // Arrange
            _toolingStructure.AddStation(_station1);
            var geometryService = new ToolingGeometryService();
            var duplicateStation = new StationAggregate(1, new SketchGeometry(
                new Dimensions(80, 50, 2.0), new Position3D(100, 0, 0), new Position3D(140, 25, 0)
            ), geometryService);

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() =>
                _toolingStructure.AddStation(duplicateStation));
        }

        [TestMethod()]
        public void AddStation_WithOversizedGeometry_ThrowsException()
        {
            // Arrange - Create station that exceeds machine capacity
            var geometryService = new ToolingGeometryService();
            var oversizedGeometry = new SketchGeometry(
                new Dimensions(3000, 1000, 2.0), // Exceeds MC304 capacity
                new Position3D(0, 0, 0),
                new Position3D(1500, 500, 0)
            );
            var oversizedStation = new StationAggregate(3, oversizedGeometry, geometryService);

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() =>
                _toolingStructure.AddStation(oversizedStation));
        }

        [TestMethod()]
        public void AddShoe_WithValidShoe_AddsShoe()
        {
            // Act
            _toolingStructure.AddShoe(_upperShoe);

            // Assert
            Assert.AreEqual(1, _toolingStructure.Shoes.Count);
            Assert.AreEqual(_upperShoe, _toolingStructure.Shoes.First());
        }

        [TestMethod()]
        public void AddShoe_WithNullShoe_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                _toolingStructure.AddShoe(null));
        }

        [TestMethod()]
        public void AddShoe_WithIncompatibleShoe_ThrowsException()
        {
            // Arrange - Add station with plate, then try to add incompatible shoe
            _toolingStructure.AddStation(_station1);
            var largePlate = new Plate("LARGE_PLATE", new Dimensions(90, 55, 35), PlateType.Die_Plate);
            _station1.AddPlate(largePlate);

            var incompatibleShoe = new Shoe("SMALL_SHOE", new Dimensions(100, 50, 70), ShoeType.Upper); // Too small

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() =>
                _toolingStructure.AddShoe(incompatibleShoe));
        }

        [TestMethod()]
        public void AddParallelBar_WithValidBar_AddsBar()
        {
            // Act
            _toolingStructure.AddParallelBar(_parallelBar);

            // Assert
            Assert.AreEqual(1, _toolingStructure.ParallelBars.Count);
            Assert.AreEqual(_parallelBar, _toolingStructure.ParallelBars.First());
        }

        [TestMethod()]
        public void AddParallelBar_WithNullBar_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                _toolingStructure.AddParallelBar(null));
        }

        [TestMethod()]
        public void AddCommonPlate_WithValidPlate_AddsPlate()
        {
            // Act
            _toolingStructure.AddCommonPlate(_commonPlate);

            // Assert
            Assert.AreEqual(1, _toolingStructure.CommonPlates.Count);
            Assert.AreEqual(_commonPlate, _toolingStructure.CommonPlates.First());
        }

        [TestMethod()]
        public void AddCommonPlate_WithNullPlate_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                _toolingStructure.AddCommonPlate(null));
        }

        [TestMethod()]
        public void AddCommonPlate_WithIncompatiblePlate_ThrowsException()
        {
            // Arrange - Create common plate for different machine
            var largeMachineSpec = MachineSpecification.GetByName("MC1801");
            var incompatiblePlate = CommonPlate.CreateSingle("INCOMPATIBLE", largeMachineSpec);

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() =>
                _toolingStructure.AddCommonPlate(incompatiblePlate));
        }

        [TestMethod()]
        public void GetStation_WithExistingStation_ReturnsStation()
        {
            // Arrange
            _toolingStructure.AddStation(_station1);
            _toolingStructure.AddStation(_station2);

            // Act
            var result = _toolingStructure.GetStation(2);

            // Assert
            Assert.AreEqual(_station2, result);
        }

        [TestMethod()]
        public void GetStation_WithNonExistingStation_ReturnsNull()
        {
            // Act
            var result = _toolingStructure.GetStation(99);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void GetShoe_WithExistingShoe_ReturnsShoe()
        {
            // Arrange
            _toolingStructure.AddShoe(_upperShoe);
            _toolingStructure.AddShoe(_lowerShoe);

            // Act
            var result = _toolingStructure.GetShoe(ShoeType.Lower);

            // Assert
            Assert.AreEqual(_lowerShoe, result);
        }

        [TestMethod()]
        public void GetShoe_WithNonExistingShoe_ReturnsNull()
        {
            // Act
            var result = _toolingStructure.GetShoe(ShoeType.Upper);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void CalculateStripLength_WithNoStations_ReturnsZero()
        {
            // Act
            var stripLength = _toolingStructure.CalculateStripLength();

            // Assert
            Assert.AreEqual(0, stripLength);
        }

        [TestMethod()]
        public void CalculateStripLength_WithMultipleStations_ReturnsCorrectLength()
        {
            // Arrange
            _toolingStructure.AddStation(_station1); // At X=0, Length=100
            _toolingStructure.AddStation(_station2); // At X=200, Length=120

            // Act
            var stripLength = _toolingStructure.CalculateStripLength();

            // Assert
            // From station 1 start (X=0) to station 2 end (X=200+120=320)
            Assert.AreEqual(320, stripLength);
        }

        [TestMethod()]
        public void ValidateStructure_WithCompleteStructure_ReturnsValid()
        {
            // Arrange - Add all required components
            _toolingStructure.AddStation(_station1);
            _toolingStructure.AddShoe(_upperShoe);
            _toolingStructure.AddCommonPlate(_commonPlate);

            // Act
            var validation = _toolingStructure.ValidateStructure();

            // Assert
            Assert.IsTrue(validation.IsValid);
            Assert.AreEqual("Tooling structure validation passed", validation.Summary);
            Assert.AreEqual(0, validation.Issues.Count);
        }

        [TestMethod()]
        public void ValidateStructure_WithMissingStations_ReturnsInvalid()
        {
            // Arrange - No stations added
            _toolingStructure.AddShoe(_upperShoe);
            _toolingStructure.AddCommonPlate(_commonPlate);

            // Act
            var validation = _toolingStructure.ValidateStructure();

            // Assert
            Assert.IsFalse(validation.IsValid);
            Assert.IsTrue(validation.Issues.Any(i => i.Contains("at least one station")));
        }

        [TestMethod()]
        public void ValidateStructure_WithMissingShoes_ReturnsInvalid()
        {
            // Arrange - No shoes added
            _toolingStructure.AddStation(_station1);
            _toolingStructure.AddCommonPlate(_commonPlate);

            // Act
            var validation = _toolingStructure.ValidateStructure();

            // Assert
            Assert.IsFalse(validation.IsValid);
            Assert.IsTrue(validation.Issues.Any(i => i.Contains("at least one shoe")));
        }

        [TestMethod()]
        public void ValidateStructure_WithMissingCommonPlates_ReturnsInvalid()
        {
            // Arrange - No common plates added
            _toolingStructure.AddStation(_station1);
            _toolingStructure.AddShoe(_upperShoe);

            // Act
            var validation = _toolingStructure.ValidateStructure();

            // Assert
            Assert.IsFalse(validation.IsValid);
            Assert.IsTrue(validation.Issues.Any(i => i.Contains("common plate")));
        }

        [TestMethod()]
        public void GetStructureSummary_ReturnsCorrectSummary()
        {
            // Arrange
            _toolingStructure.AddStation(_station1);
            _toolingStructure.AddStation(_station2);
            _toolingStructure.AddShoe(_upperShoe);
            _toolingStructure.AddCommonPlate(_commonPlate);

            // Add plates to stations for plate count
            var plate1 = new Plate("PLATE_1", new Dimensions(90, 50, 35), PlateType.Die_Plate);
            var plate2 = new Plate("PLATE_2", new Dimensions(90, 50, 30), PlateType.Stripper_Plate);
            _station1.AddPlate(plate1);
            _station2.AddPlate(plate2);

            // Act
            var summary = _toolingStructure.GetStructureSummary();

            // Assert
            Assert.AreEqual("Project1", summary.ProjectName);
            Assert.AreEqual("Designer1", summary.Designer);
            Assert.AreEqual(_baseDrawingCode, summary.BaseDrawingCode);
            Assert.AreEqual("MC304", summary.MachineName);
            Assert.AreEqual(2, summary.StationCount);
            Assert.AreEqual(2, summary.TotalPlateCount);
            Assert.AreEqual(320, summary.StripLength); // Calculated strip length
            Assert.AreEqual(1.55, summary.MaterialThickness);
            Assert.IsTrue(summary.IsValid); // Should be valid with all components
        }

        [TestMethod()]
        public void ToString_ReturnsFormattedString()
        {
            // Arrange
            _toolingStructure.AddStation(_station1);

            // Act
            var result = _toolingStructure.ToString();

            // Assert
            Assert.IsTrue(result.Contains("Project1"));
            Assert.IsTrue(result.Contains("1 stations"));
            Assert.IsTrue(result.Contains("MC304"));
        }
    }
}