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
    public class MachineSpecificationTests
    {
        [TestMethod()]
        public void GetByName_WithValidMachine_ReturnsSpecification()
        {
            // Act
            var spec = MachineSpecification.GetByName("MC304");

            // Assert
            Assert.AreEqual("MC304", spec.MachineName);
            Assert.AreEqual(2100, spec.CommonPlateDimensions.Length);
            Assert.AreEqual(700, spec.CommonPlateDimensions.Width);
            Assert.AreEqual(40, spec.CommonPlateDimensions.Thickness);
            Assert.IsFalse(spec.SupportsDoubleJoint);
        }

        [TestMethod()]
        public void GetByName_WithLargeMachine_ReturnsDoubleJointSupport()
        {
            // Act
            var spec = MachineSpecification.GetByName("MC1801");

            // Assert
            Assert.IsTrue(spec.SupportsDoubleJoint);
            Assert.IsTrue(spec.IsLargeMachine);
        }

        [TestMethod()]
        public void GetByName_WithInvalidMachine_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                MachineSpecification.GetByName("INVALID"));
        }

        [TestMethod()]
        public void CanAccommodateTool_WithValidDimensions_ReturnsTrue()
        {
            // Arrange
            var spec = MachineSpecification.GetByName("MC304");
            var toolDimensions = new Dimensions(2000, 600, 100);

            // Act & Assert
            Assert.IsTrue(spec.CanAccommodateTool(toolDimensions));
        }

        [TestMethod()]
        public void CanAccommodateTool_WithOversizedTool_ReturnsFalse()
        {
            // Arrange
            var spec = MachineSpecification.GetByName("MC304");
            var toolDimensions = new Dimensions(2500, 600, 100); // Too long

            // Act & Assert
            Assert.IsFalse(spec.CanAccommodateTool(toolDimensions));
        }
    }
}