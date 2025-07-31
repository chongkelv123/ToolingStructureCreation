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
    public class DrawingCodeTests
    {
        [TestMethod()]
        public void Constructor_WithValidParts_CreatesDrawingCode()
        {
            // Arrange & Act
            var drawingCode = new DrawingCode("40XC00", "2401", "0102");

            // Assert
            Assert.AreEqual("40XC00", drawingCode.Prefix);
            Assert.AreEqual("2401", drawingCode.Suffix);
            Assert.AreEqual("0102", drawingCode.Code);
            Assert.AreEqual("40XC00-2401-0102", drawingCode.FullCode);
        }

        [TestMethod()]
        public void Constructor_WithFullCode_ParsesCorrectly()
        {
            // Arrange & Act
            var drawingCode = new DrawingCode("40XC00-2401-0102");

            // Assert
            Assert.AreEqual("40XC00", drawingCode.Prefix);
            Assert.AreEqual("2401", drawingCode.Suffix);
            Assert.AreEqual("0102", drawingCode.Code);
        }

        [TestMethod()]
        public void Constructor_WithShortPrefix_NormalizesToSixCharacters()
        {
            // Arrange & Act
            var drawingCode = new DrawingCode("43J", "2401", "0102");

            // Assert
            Assert.AreEqual("43JXXX", drawingCode.Prefix);
        }

        [TestMethod()]
        public void GetStationNumber_ReturnsCorrectValue()
        {
            // Arrange
            var drawingCode = new DrawingCode("40XC00", "2401", "0502");

            // Act
            var stationNumber = drawingCode.GetStationNumber();

            // Assert
            Assert.AreEqual(5, stationNumber);
        }

        [TestMethod()]
        public void GetTypeCode_ReturnsCorrectValue()
        {
            // Arrange
            var drawingCode = new DrawingCode("40XC00", "2401", "0502");

            // Act
            var typeCode = drawingCode.GetTypeCode();

            // Assert
            Assert.AreEqual(2, typeCode);
        }

        [TestMethod()]
        public void NextInSequence_ReturnsIncrementedCode()
        {
            // Arrange
            var drawingCode = new DrawingCode("40XC00", "2401", "0102");

            // Act
            var next = drawingCode.NextInSequence();

            // Assert
            Assert.AreEqual("0103", next.Code);
            Assert.AreEqual("40XC00-2401-0103", next.FullCode);
        }
    }
}