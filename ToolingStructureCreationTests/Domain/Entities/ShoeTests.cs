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
    public class ShoeTests
    {
        [TestMethod()]
        public void Constructor_WithValidParameters_CreatesShoe()
        {
            // Arrange
            var dimensions = new Dimensions(500, 200, 70);

            // Act
            var shoe = new Shoe("UPPER_SHOE_1", dimensions, ShoeType.Upper);

            // Assert
            Assert.AreEqual("UPPER_SHOE_1", shoe.Name);
            Assert.AreEqual(dimensions, shoe.Dimensions);
            Assert.AreEqual(ShoeType.Upper, shoe.Type);
            Assert.IsTrue(shoe.IsUpperShoe);
        }

        [TestMethod()]
        public void Constructor_WithTooThinShoe_ThrowsException()
        {
            // Arrange
            var dimensions = new Dimensions(500, 200, 20); // Too thin

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                new Shoe("TEST", dimensions, ShoeType.Upper));
        }

        [TestMethod()]
        public void CanAccommodatePlate_WithSmallerPlate_ReturnsTrue()
        {
            // Arrange
            var shoe = new Shoe("SHOE", new Dimensions(500, 200, 70), ShoeType.Upper);
            var plate = new Plate("PLATE", new Dimensions(400, 150, 25), PlateType.DiePlate);

            // Act & Assert
            Assert.IsTrue(shoe.CanAccommodatePlate(plate));
        }

        [TestMethod()]
        public void CanAccommodatePlate_WithLargerPlate_ReturnsFalse()
        {
            // Arrange
            var shoe = new Shoe("SHOE", new Dimensions(500, 200, 70), ShoeType.Upper);
            var plate = new Plate("PLATE", new Dimensions(600, 250, 25), PlateType.DiePlate);

            // Act & Assert
            Assert.IsFalse(shoe.CanAccommodatePlate(plate));
        }
    }
}