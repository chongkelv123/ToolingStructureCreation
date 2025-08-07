using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToolingStructureCreation.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.Enums;
using ToolingStructureCreation.Domain.Services;
using ToolingStructureCreation.Domain.ValueObjects;
using ToolingStructureCreation.Domain.Entities;

namespace ToolingStructureCreation.Domain.Aggregates.Tests
{
    [TestClass()]
    public class StationAggregateTests
    {
        private SketchGeometry _stationGeometry;
        private ToolingGeometryService _geometryService;
        private StationAggregate _station;
        private PlateThicknessCalculator _thicknessCalculator;

        [TestInitialize()]
        public void Setup()
        {
            _stationGeometry = new SketchGeometry(
                new Dimensions(100, 60, 2.0),
                new Position3D(0, 0, 0),
                new Position3D(50, 30, 0)
            );

            _geometryService = new ToolingGeometryService();
            _station = new StationAggregate(1, _stationGeometry, _geometryService);

            // Setup thickness calculator for plate positioning tests
            var thicknesses = new Dictionary<PlateType, double>
            {
                { PlateType.Upper_Pad, 27.0 },
                { PlateType.Punch_Holder, 30.0 },
                { PlateType.Bottoming_Plate, 16.0 },
                { PlateType.Stripper_Plate, 30.0 },
                { PlateType.Die_Plate, 35.0 },
                { PlateType.Lower_Pad, 25.0 }
            };
            _thicknessCalculator = new PlateThicknessCalculator(thicknesses, 1.55);
        }

        [TestMethod()]
        public void Constructor_WithValidParameters_CreatesStation()
        {
            // Act
            var station = new StationAggregate(2, _stationGeometry, _geometryService);

            // Assert
            Assert.AreEqual(2, station.StationNumber);
            Assert.AreEqual(_stationGeometry, station.StationGeometry);
            Assert.AreEqual("Station_02", station.StationName);
            Assert.AreEqual(0, station.Plates.Count);
        }

        [TestMethod()]
        public void Constructor_WithZeroStationNumber_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                new StationAggregate(0, _stationGeometry, _geometryService));
        }

        [TestMethod()]
        public void Constructor_WithNullStationGeometry_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                new StationAggregate(1, null, _geometryService));
        }

        [TestMethod()]
        public void Constructor_WithNullGeometryService_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                new StationAggregate(1, _stationGeometry, null));
        }

        [TestMethod()]
        public void AddPlate_WithValidPlate_AddsPlate()
        {
            // Arrange
            var plate = new Plate("DIE_PLATE", new Dimensions(90, 50, 35), PlateType.Die_Plate);

            // Act
            _station.AddPlate(plate);

            // Assert
            Assert.AreEqual(1, _station.Plates.Count);
            Assert.AreEqual(plate, _station.Plates.First());
        }

        [TestMethod()]
        public void AddPlate_WithNullPlate_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                _station.AddPlate(null));
        }

        [TestMethod()]
        public void AddPlate_WithDuplicateType_ThrowsException()
        {
            // Arrange
            var plate1 = new Plate("DIE_PLATE_1", new Dimensions(90, 50, 35), PlateType.Die_Plate);
            var plate2 = new Plate("DIE_PLATE_2", new Dimensions(90, 50, 35), PlateType.Die_Plate);

            // Act
            _station.AddPlate(plate1);

            // Assert
            Assert.ThrowsException<InvalidOperationException>(() =>
                _station.AddPlate(plate2));
        }

        [TestMethod()]
        public void AddPlate_WithOversizedPlate_ThrowsException()
        {
            // Arrange - Plate larger than station geometry
            var oversizedPlate = new Plate("OVERSIZED", new Dimensions(200, 100, 35), PlateType.Die_Plate);

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() =>
                _station.AddPlate(oversizedPlate));
        }

        [TestMethod()]
        public void RemovePlate_WithExistingPlate_RemovesPlate()
        {
            // Arrange
            var plate = new Plate("DIE_PLATE", new Dimensions(90, 50, 35), PlateType.Die_Plate);
            _station.AddPlate(plate);

            // Act
            _station.RemovePlate(PlateType.Die_Plate);

            // Assert
            Assert.AreEqual(0, _station.Plates.Count);
        }

        [TestMethod()]
        public void RemovePlate_WithNonExistingPlate_DoesNotThrow()
        {
            // Act & Assert - Should not throw
            _station.RemovePlate(PlateType.Die_Plate);

            Assert.AreEqual(0, _station.Plates.Count);
        }

        [TestMethod()]
        public void GetPlate_WithExistingType_ReturnsPlate()
        {
            // Arrange
            var plate = new Plate("DIE_PLATE", new Dimensions(90, 50, 35), PlateType.Die_Plate);
            _station.AddPlate(plate);

            // Act
            var result = _station.GetPlate(PlateType.Die_Plate);

            // Assert
            Assert.AreEqual(plate, result);
        }

        [TestMethod()]
        public void GetPlate_WithNonExistingType_ReturnsNull()
        {
            // Act
            var result = _station.GetPlate(PlateType.Die_Plate);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void HasPlate_WithExistingPlate_ReturnsTrue()
        {
            // Arrange
            var plate = new Plate("DIE_PLATE", new Dimensions(90, 50, 35), PlateType.Die_Plate);
            _station.AddPlate(plate);

            // Act & Assert
            Assert.IsTrue(_station.HasPlate(PlateType.Die_Plate));
        }

        [TestMethod()]
        public void HasPlate_WithNonExistingPlate_ReturnsFalse()
        {
            // Act & Assert
            Assert.IsFalse(_station.HasPlate(PlateType.Die_Plate));
        }

        [TestMethod()]
        public void CanAccommodatePlate_WithSmallerPlate_ReturnsTrue()
        {
            // Arrange - Plate that fits within station geometry
            var plate = new Plate("SMALL_PLATE", new Dimensions(90, 50, 35), PlateType.Die_Plate);

            // Act & Assert
            Assert.IsTrue(_station.CanAccommodatePlate(plate));
        }

        [TestMethod()]
        public void CanAccommodatePlate_WithOversizedPlate_ReturnsFalse()
        {
            // Arrange - Plate larger than station geometry
            var plate = new Plate("OVERSIZED", new Dimensions(200, 100, 35), PlateType.Die_Plate);

            // Act & Assert
            Assert.IsFalse(_station.CanAccommodatePlate(plate));
        }

        [TestMethod()]
        public void CanAccommodatePlate_WithNullPlate_ReturnsFalse()
        {
            // Act & Assert
            Assert.IsFalse(_station.CanAccommodatePlate(null));
        }

        [TestMethod()]
        public void GetTotalPlateThickness_WithMultiplePlates_ReturnsSum()
        {
            // Arrange
            var plate1 = new Plate("DIE_PLATE", new Dimensions(90, 50, 35), PlateType.Die_Plate);
            var plate2 = new Plate("STRIPPER_PLATE", new Dimensions(90, 50, 30), PlateType.Stripper_Plate);

            _station.AddPlate(plate1);
            _station.AddPlate(plate2);

            // Act
            var totalThickness = _station.GetTotalPlateThickness();

            // Assert
            Assert.AreEqual(65, totalThickness); // 35 + 30
        }

        [TestMethod()]
        public void GetTotalPlateThickness_WithNoPlates_ReturnsZero()
        {
            // Act
            var totalThickness = _station.GetTotalPlateThickness();

            // Assert
            Assert.AreEqual(0, totalThickness);
        }

        [TestMethod()]
        public void CalculatePlatePosition_WithValidCalculator_ReturnsPositions()
        {
            // Arrange
            var diePlate = new Plate("DIE_PLATE", new Dimensions(90, 50, 35), PlateType.Die_Plate);
            var stripperPlate = new Plate("STRIPPER_PLATE", new Dimensions(90, 50, 30), PlateType.Stripper_Plate);

            _station.AddPlate(diePlate);
            _station.AddPlate(stripperPlate);

            // Act
            var positions = _station.CalculatePlatePosition(_thicknessCalculator);

            // Assert
            Assert.IsTrue(positions.Any());
            var diePosition = positions.FirstOrDefault(p => p.PlateType == PlateType.Die_Plate);
            Assert.IsNotNull(diePosition);
            Assert.AreEqual(PlateType.Die_Plate, diePosition.PlateType);
        }

        [TestMethod()]
        public void CalculatePlatePosition_WithNullCalculator_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                _station.CalculatePlatePosition(null));
        }

        [TestMethod()]
        public void ValidateStationIntegrity_WithValidPlates_ReturnsAcceptableAnalysis()
        {
            // Arrange
            var plate = new Plate("DIE_PLATE", new Dimensions(90, 50, 35), PlateType.Die_Plate);
            _station.AddPlate(plate);

            // Act
            var analysis = _station.ValidateStationIntegrity();

            // Assert
            Assert.IsNotNull(analysis);
            Assert.IsInstanceOfType(analysis, typeof(ClearanceAnalysis));
        }

        [TestMethod()]
        public void GetStationSummary_WithPlates_ReturnsSummary()
        {
            // Arrange
            var uprPad = new Plate("UPPER_PAD", new Dimensions(90, 50, 27), PlateType.Upper_Pad);
            var punHolder = new Plate("PUNCH_HOLDER", new Dimensions(90, 50, 30), PlateType.Punch_Holder);
            var botPlate = new Plate("BOTTOMING_PLATE", new Dimensions(90, 50, 16), PlateType.Bottoming_Plate);
            var stripperPlate = new Plate("STRIPPER_PLATE", new Dimensions(90, 50, 30), PlateType.Stripper_Plate);
            var diePlate = new Plate("DIE_PLATE", new Dimensions(90, 50, 35), PlateType.Die_Plate);
            var lowPad = new Plate("LOWER_PAD", new Dimensions(90, 50, 25), PlateType.Lower_Pad);
            

            _station.AddPlate(uprPad);
            _station.AddPlate(punHolder);
            _station.AddPlate(botPlate);
            _station.AddPlate(stripperPlate);
            _station.AddPlate(diePlate);
            _station.AddPlate(lowPad);

            // Act
            var summary = _station.GetStationSummary();

            // Assert
            Assert.IsNotNull(summary);
            Assert.AreEqual(1, summary.StationNumber);
            Assert.AreEqual(6, summary.PlateCount);
            Assert.AreEqual(163, summary.TotalThickness); // 35 + 30
            Assert.IsTrue(summary.HasAllRequiredPlates); 
            Assert.AreEqual(_stationGeometry.Dimensions, summary.StationDimensions);
        }

        [TestMethod()]
        public void HasRequiredPlates_WithDieAndStripperPlates_ReturnsTrue()
        {
            // Arrange
            var uprPad = new Plate("UPPER_PAD", new Dimensions(90, 50, 27), PlateType.Upper_Pad);
            var punHolder = new Plate("PUNCH_HOLDER", new Dimensions(90, 50, 30), PlateType.Punch_Holder);
            var botPlate = new Plate("BOTTOMING_PLATE", new Dimensions(90, 50, 16), PlateType.Bottoming_Plate);
            var stripperPlate = new Plate("STRIPPER_PLATE", new Dimensions(90, 50, 30), PlateType.Stripper_Plate);
            var diePlate = new Plate("DIE_PLATE", new Dimensions(90, 50, 35), PlateType.Die_Plate);
            var lowPad = new Plate("LOWER_PAD", new Dimensions(90, 50, 25), PlateType.Lower_Pad);


            _station.AddPlate(uprPad);
            _station.AddPlate(punHolder);
            _station.AddPlate(botPlate);
            _station.AddPlate(stripperPlate);
            _station.AddPlate(diePlate);
            _station.AddPlate(lowPad);

            // Act & Assert
            Assert.IsTrue(_station.HasRequiredPlates());
        }

        [TestMethod()]
        public void HasRequiredPlates_WithMissingRequiredPlates_ReturnsFalse()
        {
            // Arrange - Add only Die Plate, missing Stripper Plate
            var diePlate = new Plate("DIE_PLATE", new Dimensions(90, 50, 35), PlateType.Die_Plate);
            _station.AddPlate(diePlate);

            // Act & Assert
            Assert.IsFalse(_station.HasRequiredPlates());
        }

        [TestMethod()]
        public void HasRequiredPlates_WithNoPlates_ReturnsFalse()
        {
            // Act & Assert
            Assert.IsFalse(_station.HasRequiredPlates());
        }

        [TestMethod()]
        public void StationName_WithStationNumber_ReturnsFormattedName()
        {
            // Arrange
            var station = new StationAggregate(5, _stationGeometry, _geometryService);

            // Act & Assert
            Assert.AreEqual("Station_05", station.StationName);
        }

        [TestMethod()]
        public void ToString_ReturnsFormattedString()
        {
            // Arrange
            var plate = new Plate("DIE_PLATE", new Dimensions(90, 50, 35), PlateType.Die_Plate);
            _station.AddPlate(plate);

            // Act
            var result = _station.ToString();

            // Assert
            Assert.IsTrue(result.Contains("Station 1"));
            Assert.IsTrue(result.Contains("1 plates"));
            Assert.IsTrue(result.Contains("Geometry:"));
        }
    }
}