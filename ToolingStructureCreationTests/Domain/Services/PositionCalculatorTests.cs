using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToolingStructureCreation.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.Enums;
using ToolingStructureCreation.Domain.ValueObjects;

namespace ToolingStructureCreation.Domain.Services.Tests
{
    [TestClass()]
    public class PositionCalculatorTests
    {
        private PlateThicknessCalculator _thicknessCalculator;
        private PositionCalculator _positionCalculator;

        [TestInitialize()]
        public void Setup()
        {
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
            _positionCalculator = new PositionCalculator(_thicknessCalculator, 155.0, 60.0);
        }

        [TestMethod()]
        public void CalculateUpperShoePosition_ReturnsCorrectPosition()
        {
            // Arrange
            var basePosition = new Position3D(100, 0, 0);

            // Act
            var upperShoePosition = _positionCalculator.CalculateUpperShoePosition(basePosition);

            // Assert
            Assert.AreEqual(100, upperShoePosition.X);
            Assert.AreEqual(0, upperShoePosition.Y);
            Assert.IsTrue(upperShoePosition.Z > 100); // Should be well above base
        }

        [TestMethod()]
        public void CalculateLowerShoePosition_ReturnsNegativeZ()
        {
            // Arrange
            var basePosition = new Position3D(100, 0, 0);

            // Act
            var lowerShoePosition = _positionCalculator.CalculateLowerShoePosition(basePosition);

            // Assert
            Assert.AreEqual(100, lowerShoePosition.X);
            Assert.AreEqual(0, lowerShoePosition.Y);
            Assert.IsTrue(lowerShoePosition.Z < 0); // Should be below base
        }

        [TestMethod()]
        public void CalculateParallelBarPlacements_WithLongTool_CreatesMultipleBars()
        {
            // Arrange
            var startPosition = new Position3D(0, 0, 0);
            var toolLength = 1000.0; // Long tool requiring multiple bars

            // Act
            var placements = _positionCalculator.CalculateParallelBarPlacements(startPosition, toolLength);

            // Assert
            Assert.IsTrue(placements.Count >= 2); // At minimum, start and end bars
            Assert.AreEqual(1, placements[0].SequenceNumber); // First bar
            Assert.IsTrue(placements[0].Position.X < placements[placements.Count - 1].Position.X); // Ordered by X
        }

        [TestMethod()]
        public void CalculateFeedHeight_WithLiftHeight_ReturnsCorrectHeight()
        {
            // Arrange
            var liftHeight = 5.0;

            // Act
            var feedHeight = _positionCalculator.CalculateFeedHeight(liftHeight);

            // Assert
            Assert.IsTrue(feedHeight > liftHeight); // Must be higher than lift height
            Assert.IsTrue(feedHeight > 200); // Should include all lower thicknesses
        }
    }
}