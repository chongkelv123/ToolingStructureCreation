using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToolingStructureCreation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.ValueObjects;

namespace ToolingStructureCreation.Domain.Entities.Tests
{
    [TestClass()]
    public class ParallelBarTests
    {
        [TestMethod()]
        public void Constructor_WithValidParameters_CreatesParallelBar()
        {
            // Arrange
            var dimensions = new Dimensions(60, 500, 155);

            // Act
            var parallelBar = new ParallelBar("PBAR_001", dimensions);

            // Assert
            Assert.AreEqual("PBAR_001", parallelBar.Name);
            Assert.AreEqual(dimensions, parallelBar.Dimensions);
            Assert.AreEqual(1, parallelBar.Quantity);
        }

        [TestMethod()]
        public void Constructor_WithInvalidWidth_ThrowsException()
        {
            // Arrange
            var dimensions = new Dimensions(50, 500, 155); // Wrong width, should be 60

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                new ParallelBar("TEST", dimensions));
        }

        [TestMethod()]
        public void TotalWeight_WithMultipleQuantity_CalculatesCorrectly()
        {
            // Arrange
            var dimensions = new Dimensions(60, 500, 155);
            var parallelBar = new ParallelBar("TEST", dimensions, "S50C", 3);

            // Act
            var totalWeight = parallelBar.TotalWeight;
            var singleWeight = parallelBar.Weight;

            // Assert
            Assert.AreEqual(singleWeight * 3, totalWeight);
        }

        [TestMethod()]
        public void CanSupportLoad_WithReasonableLoad_ReturnsTrue()
        {
            // Arrange
            var dimensions = new Dimensions(60, 500, 155);
            var parallelBar = new ParallelBar("TEST", dimensions);

            // Act & Assert
            Assert.IsTrue(parallelBar.CanSupportLoad(100)); // 100 kg should be fine
        }

    }
}