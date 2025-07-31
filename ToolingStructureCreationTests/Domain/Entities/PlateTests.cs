using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToolingStructureCreation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.Enums;
using ToolingStructureCreation.Domain.ValueObjects;

namespace ToolingStructureCreation.Domain.Entities.Tests
{
    [TestClass()]
    public class PlateTests
    {
        [TestMethod()]
        public void Constructor_WithValidParameters_CreatesPlate()
        {
            // Arrange
            var dimensions = new Dimensions(100, 50, 25);

            // Act
            var plate = new Plate("TEST_PLATE", dimensions, PlateType.DiePlate);

            // Assert
            Assert.AreEqual("TEST_PLATE", plate.Name);
            Assert.AreEqual(dimensions, plate.Dimensions);
            Assert.AreEqual(PlateType.DiePlate, plate.Type);
            Assert.AreEqual(PlateColor.DiePlate, plate.Color);
        }

        [TestMethod()]
        public void Constructor_WithInvalidUpperPadThickness_ThrowsException()
        {
            // Arrange
            var dimensions = new Dimensions(100, 50, 10); // Too thin

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                new Plate("TEST", dimensions, PlateType.UpperPad));
        }

        [TestMethod()]
        public void Constructor_WithTooSmallArea_ThrowsException()
        {
            // Arrange
            var dimensions = new Dimensions(5, 5, 25); // Area = 25mm² < 100mm²

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                new Plate("TEST", dimensions, PlateType.DiePlate));
        }

        [TestMethod()]
        public void Weight_CalculatesCorrectly()
        {
            // Arrange
            var dimensions = new Dimensions(100, 100, 25); // 100,000 mm³
            var plate = new Plate("TEST", dimensions, PlateType.DiePlate);

            // Act
            var weight = plate.Weight;

            // Assert
            // 250,000 mm³ = 250 cm³, steel density ≈ 7.85 g/cm³ = 1962.5g
            Assert.AreEqual(1962.5, weight, 0.1);
        }

        [TestMethod()]
        public void IsThickerThan_WithThickerPlate_ReturnsTrue()
        {
            // Arrange
            var thickPlate = new Plate("THICK", new Dimensions(100, 50, 30), PlateType.DiePlate);
            var thinPlate = new Plate("THIN", new Dimensions(100, 50, 20), PlateType.UpperPad);

            // Act & Assert
            Assert.IsTrue(thickPlate.IsThickerThan(thinPlate));
            Assert.IsFalse(thinPlate.IsThickerThan(thickPlate));
        }
    }
}