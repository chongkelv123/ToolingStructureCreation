using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToolingStructureCreation.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ToolingStructureCreation.Domain.Enums;
using ToolingStructureCreation.Domain.ValueObjects;

namespace ToolingStructureCreation.Domain.Services.Tests
{
    [TestClass()]
    public class NamingConventionServiceTests
    {
        private DrawingCode _baseDrawingCode;
        private string _testDirectory;
        private NamingConventionService _namingService;

        [TestInitialize()]
        public void Setup()
        {
            _baseDrawingCode = new DrawingCode("40XC00-2401-0000");
            _testDirectory = Path.Combine(Path.GetTempPath(), "ToolingTest");
            Directory.CreateDirectory(_testDirectory);
            _namingService = new NamingConventionService(_baseDrawingCode, _testDirectory);
        }

        [TestCleanup()]
        public void Cleanup()
        {
            if (Directory.Exists(_testDirectory))
                Directory.Delete(_testDirectory, true);
        }

        [TestMethod()]
        public void GeneratePlateNaming_WithValidParameters_ReturnsCorrectNaming()
        {
            // Act
            var naming = _namingService.GeneratePlateNaming(PlateType.Die_Plate, 2);

            // Assert
            Assert.AreEqual("40XC00-2401-0206", naming.DrawingCode.FullCode);
            Assert.AreEqual("DIE PLATE", naming.ItemName);
            Assert.IsTrue(naming.FolderCode.Contains("Die_Plate"));
            Assert.IsTrue(naming.FileName.EndsWith("-V00"));
        }

        [TestMethod()]
        public void GenerateShoeNaming_WithUpperShoe_ReturnsCorrectNaming()
        {
            // Act
            var naming = _namingService.GenerateShoeNaming(ShoeType.Upper);

            // Assert
            Assert.AreEqual("40XC00-2401-0001", naming.DrawingCode.FullCode);
            Assert.AreEqual("UPPER SHOE", naming.ItemName);
            Assert.IsTrue(naming.FolderCode.Contains("Upper_SHOE"));
        }

        [TestMethod()]
        public void GenerateAssemblyNaming_WithStationNumber_ReturnsCorrectNaming()
        {
            // Act
            var naming = _namingService.GenerateAssemblyNaming("Station Assembly", 3);

            // Assert
            Assert.AreEqual("40XC00-2401-0300", naming.DrawingCode.FullCode);
            Assert.AreEqual("Stn3-Assembly", naming.ItemName);
        }

        [TestMethod()]
        public void GenerateAssemblyNaming_WithoutStationNumber_UsesZeroCode()
        {
            // Act
            var naming = _namingService.GenerateAssemblyNaming("Main Assembly");

            // Assert
            Assert.AreEqual("40XC00-2401-0000", naming.DrawingCode.FullCode);
            Assert.AreEqual("Main Assembly", naming.ItemName);
        }
    }
}