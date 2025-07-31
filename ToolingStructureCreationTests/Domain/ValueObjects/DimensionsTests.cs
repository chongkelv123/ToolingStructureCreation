using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToolingStructureCreation.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Domain.ValueObjects.Tests
{
    [TestClass()]
    public class DimensionsTests
    {
        [TestMethod()]
        public void Constructor_WithValidValues_CreatesInstance()
        {
            // Arrange & Act
            var dimensions = new Dimensions(100.0, 50.0, 25.0);

            // Assert
            Assert.AreEqual(100.0, dimensions.Length);
            Assert.AreEqual(50.0, dimensions.Width);
            Assert.AreEqual(25.0, dimensions.Thickness);
        }

        [TestMethod()]
        public void Constructor_WithNegativeWidth_ThrowsArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() => new Dimensions(100, -50, 25));
        }

        [TestMethod()]
        public void Volumme_CalculatesCorrectly()
        {
            // Arrange
            var dimensions = new Dimensions(100, 50, 25);

            // Act
        }

        [TestMethod()]
        public void Equals_WithSameDimensions_ReturnsTrue()
        {
            // Arrange
            var dim1 = new Dimensions(100, 50, 25);
            var dim2 = new Dimensions(100, 50, 25);

            // Act & Assert
            Assert.IsTrue(dim1.Equals(dim2));
            Assert.IsTrue(dim1 == dim2);
        }

        [TestMethod()]
        public void ScaleBy_WithValidFactor_ReturnScaleDimensions()
        {
            // Arrange
            var original = new Dimensions(100, 50, 25);

            // Act 
            var scaled = original.ScaleBy(2.0);

            // Assert
            Assert.AreEqual(200, scaled.Length);
            Assert.AreEqual(100, scaled.Width);
            Assert.AreEqual(50, scaled.Thickness);
        }

        [TestMethod()]
        public void WithThickness_ReturnsNewInstanceWithUpdatedThickness()
        {
            // Arrange
            var original = new Dimensions(100, 50, 25);

            // Act
            var updated = original.WithThickness(30);

            // Assert
            Assert.AreEqual(100, updated.Length);
            Assert.AreEqual(50, updated.Width);
            Assert.AreEqual(30, updated.Thickness);
        }
    }
}