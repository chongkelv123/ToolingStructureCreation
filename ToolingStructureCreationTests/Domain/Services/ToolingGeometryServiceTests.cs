using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToolingStructureCreation.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.Entities;
using ToolingStructureCreation.Domain.Enums;
using ToolingStructureCreation.Domain.ValueObjects;

namespace ToolingStructureCreation.Domain.Services.Tests
{
    [TestClass()]
    public class ToolingGeometryServiceTests
    {
        private ToolingGeometryService _geometryService;

        [TestInitialize()]
        public void Setup()
        {
            _geometryService = new ToolingGeometryService();
        }

        [TestMethod()]
        public void ValidateRectangularSketch_WithValidRectangle_ReturnsValidResult()
        {
            // Arrange - Rectangle from (0,0,10) to (100,50,10)
            var sketchPoints = new List<Position3D>
            {
                new Position3D(0, 0, 10),    // Bottom-left
                new Position3D(100, 0, 10),  // Bottom-right
                new Position3D(100, 50, 10), // Top-right
                new Position3D(0, 50, 10)    // Top-left
            };

            // Act
            var result = _geometryService.ValidateRectangularSketch(sketchPoints);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.IsNotNull(result.SketchInfo);
            Assert.AreEqual(100, result.SketchInfo.Dimensions.Length);
            Assert.AreEqual(50, result.SketchInfo.Dimensions.Width);

            // Verify startLocation logic: (minX, midY, firstPointZ)
            Assert.AreEqual(0, result.SketchInfo.StartLocation.X);     // minX
            Assert.AreEqual(25, result.SketchInfo.StartLocation.Y);    // midY = (0+50)/2
            Assert.AreEqual(10, result.SketchInfo.StartLocation.Z);    // firstPointZ

            // Verify midPoint logic: (midX, midY, firstPointZ)
            Assert.AreEqual(50, result.SketchInfo.MidPoint.X);         // midX = (0+100)/2
            Assert.AreEqual(25, result.SketchInfo.MidPoint.Y);         // midY = (0+50)/2
            Assert.AreEqual(10, result.SketchInfo.MidPoint.Z);         // firstPointZ
        }

        [TestMethod()]
        public void ValidateRectangularSketch_WithWrongNumberOfPoints_ReturnsInvalid()
        {
            // Arrange
            var sketchPoints = new List<Position3D>
            {
                new Position3D(0, 0, 0),
                new Position3D(100, 0, 0),
                new Position3D(100, 50, 0)  // Only 3 points
            };

            // Act
            var result = _geometryService.ValidateRectangularSketch(sketchPoints);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Message.Contains("exactly 4 points"));
        }

        [TestMethod()]
        public void ValidateRectangularSketch_WithTooSmallDimensions_ReturnsInvalid()
        {
            // Arrange - Very small rectangle
            var sketchPoints = new List<Position3D>
            {
                new Position3D(0, 0, 0),
                new Position3D(0.05, 0, 0),     // Length = 0.05mm (too small)
                new Position3D(0.05, 10, 0),
                new Position3D(0, 10, 0)
            };

            // Act
            var result = _geometryService.ValidateRectangularSketch(sketchPoints);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Message.Contains("too small"));
        }

        [TestMethod()]
        public void ValidateRectangularSketch_WithUnorderedPoints_StillWorks()
        {
            // Arrange - Points in random order
            var sketchPoints = new List<Position3D>
            {
                new Position3D(100, 50, 5),  // Top-right
                new Position3D(0, 0, 5),     // Bottom-left  
                new Position3D(0, 50, 5),    // Top-left
                new Position3D(100, 0, 5)    // Bottom-right
            };

            // Act
            var result = _geometryService.ValidateRectangularSketch(sketchPoints);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(100, result.SketchInfo.Dimensions.Length);
            Assert.AreEqual(50, result.SketchInfo.Dimensions.Width);
            Assert.AreEqual(0, result.SketchInfo.StartLocation.X);     // minX
            Assert.AreEqual(25, result.SketchInfo.StartLocation.Y);    // midY
            Assert.AreEqual(5, result.SketchInfo.StartLocation.Z);     // firstPointZ (from first in list)
        }

        [TestMethod()]
        public void SortSketchesByStartLocation_WithMultipleSketches_SortsCorrectly()
        {
            // Arrange
            var sketches = new List<SketchGeometry>
            {
                new SketchGeometry(new Dimensions(50, 30, 10), new Position3D(200, 0, 0), new Position3D(225, 15, 0)),
                new SketchGeometry(new Dimensions(60, 40, 10), new Position3D(50, 0, 0), new Position3D(80, 20, 0)),
                new SketchGeometry(new Dimensions(40, 20, 10), new Position3D(500, 0, 0), new Position3D(520, 10, 0))
            };

            // Act
            var sorted = _geometryService.SortSketchesByStartLocation(sketches);

            // Assert
            Assert.AreEqual(3, sorted.Count);
            Assert.AreEqual(50, sorted[0].StartLocation.X);   // First sketch
            Assert.AreEqual(200, sorted[1].StartLocation.X);  // Second sketch
            Assert.AreEqual(500, sorted[2].StartLocation.X);  // Third sketch
        }

        [TestMethod()]
        public void CanPlatesFitInShoe_WithSmallerPlates_ReturnsTrue()
        {
            // Arrange
            var shoe = new Shoe("TEST_SHOE", new Dimensions(500, 300, 80), ShoeType.Upper);
            var plates = new List<Plate>
            {
                new Plate("PLATE1", new Dimensions(400, 250, 30), PlateType.Die_Plate),
                new Plate("PLATE2", new Dimensions(350, 200, 25), PlateType.Upper_Pad)
            };

            // Act
            var canFit = _geometryService.CanPlatesFitInShoe(shoe, plates);

            // Assert
            Assert.IsTrue(canFit);
        }

        [TestMethod()]
        public void CanPlatesFitInShoe_WithOversizedPlate_ReturnsFalse()
        {
            // Arrange
            var shoe = new Shoe("TEST_SHOE", new Dimensions(500, 300, 80), ShoeType.Upper);
            var plates = new List<Plate>
            {
                new Plate("GOOD_PLATE", new Dimensions(400, 250, 30), PlateType.Die_Plate),
                new Plate("OVERSIZED_PLATE", new Dimensions(600, 250, 25), PlateType.Upper_Pad) // Too long
            };

            // Act
            var canFit = _geometryService.CanPlatesFitInShoe(shoe, plates);

            // Assert
            Assert.IsFalse(canFit);
        }

        [TestMethod()]
        public void AnalyzeClearances_WithReasonableThicknesses_ReturnsAcceptable()
        {
            // Arrange
            var plates = new List<Plate>
            {
                new Plate("UPPER_PAD", new Dimensions(400, 300, 27), PlateType.Upper_Pad),
                new Plate("PUNCH_HOLDER", new Dimensions(400, 300, 30), PlateType.Punch_Holder),
                new Plate("STRIPPER", new Dimensions(400, 300, 25), PlateType.Stripper_Plate),
                new Plate("DIE_PLATE", new Dimensions(400, 300, 35), PlateType.Die_Plate)
            };

            // Act
            var analysis = _geometryService.AnalyzeClearances(plates);

            // Assert
            Assert.IsTrue(analysis.IsAcceptable);
            Assert.IsTrue(analysis.Summary.Contains("acceptable"));
        }

        [TestMethod()]
        public void AnalyzeClearances_WithPoorThicknessProgression_ReturnsIssues()
        {
            // Arrange - Stripper thicker than punch holder (unusual)
            var plates = new List<Plate>
            {
                new Plate("PUNCH_HOLDER", new Dimensions(400, 300, 25), PlateType.Punch_Holder),
                new Plate("STRIPPER", new Dimensions(400, 300, 35), PlateType.Stripper_Plate) // Thicker than punch holder
            };

            // Act
            var analysis = _geometryService.AnalyzeClearances(plates);

            // Assert
            Assert.IsFalse(analysis.IsAcceptable);
            Assert.IsTrue(analysis.Summary.Contains("thinner"));
        }

        [TestMethod()]
        public void CalculateStripLength_WithMultipleStations_ReturnsCorrectLength()
        {
            // Arrange
            var stationSketches = new List<SketchGeometry>
            {
                new SketchGeometry(new Dimensions(80, 60, 10), new Position3D(0, 0, 0), new Position3D(40, 30, 0)),
                new SketchGeometry(new Dimensions(100, 70, 10), new Position3D(200, 0, 0), new Position3D(250, 35, 0)),
                new SketchGeometry(new Dimensions(90, 65, 10), new Position3D(450, 0, 0), new Position3D(495, 32.5, 0))
            };

            // Act
            var stripLength = _geometryService.CalculateStripLength(stationSketches);

            // Assert
            // From start of first station (X=0) to end of last station (450 + 90 = 540)
            Assert.AreEqual(540, stripLength);
        }

        [TestMethod()]
        public void SortSketchesByStartLocation_WithEmptyList_ReturnsEmptyList()
        {
            // Arrange
            var sketches = new List<SketchGeometry>();

            // Act
            var sorted = _geometryService.SortSketchesByStartLocation(sketches);

            // Assert
            Assert.IsNotNull(sorted);
            Assert.AreEqual(0, sorted.Count);
        }

        [TestMethod()]
        public void AnalyzeClearances_WithEmptyPlateList_ReturnsAcceptable()
        {
            // Arrange
            var plates = new List<Plate>();

            // Act
            var analysis = _geometryService.AnalyzeClearances(plates);

            // Assert
            Assert.IsTrue(analysis.IsAcceptable);
            Assert.IsTrue(analysis.Summary.Contains("No plates"));
        }
    }
}