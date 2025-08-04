using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToolingStructureCreation.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.Enums;

namespace ToolingStructureCreation.Domain.Services.Tests
{
    [TestClass()]
    public class PlateThicknessCalculatorTests
    {
        private Dictionary<PlateType, double> _standardThicknesses;

        [TestInitialize()]
        public void setup()
        {
            _standardThicknesses = new Dictionary<PlateType, double>
            {
                { PlateType.Upper_Pad, 27.0 },
                { PlateType.Punch_Holder, 30.0 },
                { PlateType.Bottoming_Plate, 16.0 },
                { PlateType.Stripper_Plate, 30.0 },
                { PlateType.Die_Plate, 35.0 },
                { PlateType.Lower_Pad, 25.0 }
            };
        }

        [TestMethod()]
        public void Constructor_WithValidThicknesses_CreatesCalculator()
        {
            // Act
            var calculator = new PlateThicknessCalculator(_standardThicknesses, 1.55);

            // Assert
            Assert.IsNotNull(calculator);
        }

        [TestMethod()]
        public void Constructor_WithZeroMaterialThickness_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                new PlateThicknessCalculator(_standardThicknesses, 0));
        }

        [TestMethod()]
        public void GetTotalDieHeight_CalculatesCorrectly()
        {
            // Arrange
            var calculator = new PlateThicknessCalculator(_standardThicknesses, 1.55);
            var expected = 27.0 + 30.0 + 16.0 + 30.0 + 35.0 + 25.0 + 1.55; // 164.55

            // Act
            var totalHeight = calculator.GetTotalDieHeight();

            // Assert
            Assert.AreEqual(expected, totalHeight, 0.01);
        }

        [TestMethod()]
        public void GetPunchActiveLength_WithStandardThickness_ReturnsStandardLength()
        {
            // Arrange
            var calculator = new PlateThicknessCalculator(_standardThicknesses, 1.55);
            // Required: 30 + 16 + 30 + 1.55 = 77.55, should snap to 80

            // Act
            var punchLength = calculator.GetPunchActiveLength();

            // Assert
            Assert.AreEqual(80.0, punchLength);
        }

        [TestMethod()]
        public void GetPenetrationDepth_CalculatesCorrectly()
        {
            // Arrange
            var calculator = new PlateThicknessCalculator(_standardThicknesses, 1.55);
            var expectedPunchLength = 80.0;
            var expectedConsumed = 30.0 + 16.0 + 30.0 + 1.55; // 77.55
            var expectedPenetration = expectedPunchLength - expectedConsumed; // 2.45

            // Act
            var penetration = calculator.GetPenetrationDepth();

            // Assert
            Assert.AreEqual(expectedPenetration, penetration, 0.01);
        }

        [TestMethod()]
        public void GetThicknessSummary_ReturnsCompleteInformation()
        {
            // Arrange
            var calculator = new PlateThicknessCalculator(_standardThicknesses, 1.55);

            // Act
            var summary = calculator.GetThicknessSummary();

            // Assert
            Assert.AreEqual(164.55, summary.TotalDieHeight, 0.01);
            Assert.AreEqual(80.0, summary.PunchActiveLength);
            Assert.AreEqual(1.55, summary.MaterialThickness);
            Assert.IsTrue(summary.PenetrationDepth > 0);
        }
    }
}