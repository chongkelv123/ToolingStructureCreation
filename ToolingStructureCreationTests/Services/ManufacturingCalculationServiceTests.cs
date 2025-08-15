using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToolingStructureCreation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Services.Tests
{
    [TestClass]
    public class ManufacturingCalculationServiceTests
    {
        private ManufacturingCalculationService _service;
        private ThicknessData _standardTestData;

        [TestInitialize]
        public void TestInitialize()
        {
            _service = new ManufacturingCalculationService();
            _standardTestData = ThicknessData.CreateTestData();
        }

        #region Die Height Calculation Tests

        [TestMethod]
        public void CalculateDieHeight_StandardValues_ReturnsCorrectSum()
        {
            // Arrange
            var thicknesses = new ThicknessData
            {
                UpperShoeThk = 70.0,
                UpperPadThk = 27.0,
                PunHolderThk = 30.0,
                BottomPltThk = 25.0,
                StripperPltThk = 20.0,
                MatThk = 2.0,
                DiePltThk = 40.0,
                LowerPadThk = 20.0,
                LowerShoeThk = 70.0,
                ParallelBarThk = 15.0,
                CommonPltThk = 40.0
            };
            var expected = 359.0; // Sum of all values

            // Act
            var result = _service.CalculateDieHeight(thicknesses);

            // Assert
            Assert.AreEqual(expected, result, 0.01, "Die height calculation should sum all thickness values");
        }

        [TestMethod]
        public void CalculateDieHeight_ZeroValues_ReturnsZero()
        {
            // Arrange
            var thicknesses = new ThicknessData(); // All values default to 0

            // Act
            var result = _service.CalculateDieHeight(thicknesses);

            // Assert
            Assert.AreEqual(0.0, result, "Die height should be zero when all thicknesses are zero");
        }

        [TestMethod]
        public void CalculateDieHeight_DecimalValues_HandlesCorrectly()
        {
            // Arrange
            var thicknesses = new ThicknessData
            {
                UpperShoeThk = 70.5,
                UpperPadThk = 27.25,
                DiePltThk = 40.75
                // Other values remain 0
            };
            var expected = 138.5;

            // Act
            var result = _service.CalculateDieHeight(thicknesses);

            // Assert
            Assert.AreEqual(expected, result, 0.01, "Die height should handle decimal values correctly");
        }

        #endregion

        #region Punch Length Calculation Tests

        [TestMethod]
        public void CalculatePunchLength_RequiredLength45_Returns60()
        {
            // Arrange - set values that sum to 45mm (between 50-60 band)
            var thicknesses = new ThicknessData
            {
                PunHolderThk = 20.0,
                BottomPltThk = 15.0,
                StripperPltThk = 8.0,
                MatThk = 2.0
                // Sum = 45.0, should snap to 50 band
            };

            // Act
            var result = _service.CalculatePunchLength(thicknesses);

            // Assert
            Assert.AreEqual(50.0, result, "Punch length should snap to 50mm band for 45mm requirement");
        }

        [TestMethod]
        public void CalculatePunchLength_RequiredLength55_Returns60()
        {
            // Arrange - set values that sum to 55mm (between 50-60 band)
            var thicknesses = new ThicknessData
            {
                PunHolderThk = 25.0,
                BottomPltThk = 15.0,
                StripperPltThk = 13.0,
                MatThk = 2.0
                // Sum = 55.0, should snap to 60mm (50 + 10)
            };

            // Act
            var result = _service.CalculatePunchLength(thicknesses);

            // Assert
            Assert.AreEqual(60.0, result, "Punch length should snap to 60mm for 55mm requirement");
        }

        [TestMethod]
        public void CalculatePunchLength_ExactBandValue_ReturnsSameBand()
        {
            // Arrange - set values that sum exactly to 60mm
            var thicknesses = new ThicknessData
            {
                PunHolderThk = 30.0,
                BottomPltThk = 15.0,
                StripperPltThk = 13.0,
                MatThk = 2.0
                // Sum = 60.0, should return exactly 60
            };

            // Act
            var result = _service.CalculatePunchLength(thicknesses);

            // Assert
            Assert.AreEqual(60.0, result, "Punch length should return exact band value when requirement matches");
        }

        [TestMethod]
        public void CalculatePunchLength_RequiredLength85_Returns90()
        {
            // Arrange - set values that sum to 85mm (above 80 band)
            var thicknesses = new ThicknessData
            {
                PunHolderThk = 40.0,
                BottomPltThk = 25.0,
                StripperPltThk = 18.0,
                MatThk = 2.0
                // Sum = 85.0, should snap to 90mm (80 + 10)
            };

            // Act
            var result = _service.CalculatePunchLength(thicknesses);

            // Assert
            Assert.AreEqual(90.0, result, "Punch length should snap to 90mm for 85mm requirement");
        }

        #endregion

        #region Penetration Calculation Tests

        [TestMethod]
        public void CalculatePenetration_PunchLength60Required55_Returns5()
        {
            // Arrange
            var thicknesses = new ThicknessData
            {
                PunHolderThk = 25.0,
                BottomPltThk = 15.0,
                StripperPltThk = 13.0,
                MatThk = 2.0
                // Sum = 55.0, punch length = 60.0, penetration = 5.0
            };

            // Act
            var result = _service.CalculatePenetration(thicknesses);

            // Assert
            Assert.AreEqual(5.0, result, 0.01, "Penetration should be punch length minus required thickness");
        }

        [TestMethod]
        public void CalculatePenetration_ExactMatch_ReturnsZero()
        {
            // Arrange - values that exactly match a punch length band
            var thicknesses = new ThicknessData
            {
                PunHolderThk = 30.0,
                BottomPltThk = 15.0,
                StripperPltThk = 13.0,
                MatThk = 2.0
                // Sum = 60.0, punch length = 60.0, penetration = 0.0
            };

            // Act
            var result = _service.CalculatePenetration(thicknesses);

            // Assert
            Assert.AreEqual(0.0, result, 0.01, "Penetration should be zero when requirement exactly matches band");
        }

        #endregion

        #region Feed Height Calculation Tests

        [TestMethod]
        public void CalculateFeedHeight_LiftHeight100_ReturnsCorrectSum()
        {
            // Arrange
            var thicknesses = new ThicknessData
            {
                LowerShoeThk = 70.0,
                LowerPadThk = 20.0,
                DiePltThk = 40.0,
                ParallelBarThk = 15.0,
                CommonPltThk = 40.0
                // Lower die set = 185.0
            };
            var liftHeight = 100.0;
            var expected = 285.0; // 100 + 185

            // Act
            var result = _service.CalculateFeedHeight(thicknesses, liftHeight);

            // Assert
            Assert.AreEqual(expected, result, 0.01, "Feed height should be lift height plus lower die set thickness");
        }

        [TestMethod]
        public void CalculateFeedHeight_ZeroLiftHeight_ReturnsLowerDieSetOnly()
        {
            // Arrange
            var thicknesses = _standardTestData;
            var liftHeight = 0.0;

            // Act
            var result = _service.CalculateFeedHeight(thicknesses, liftHeight);

            // Assert
            var expectedLowerDieSet = _service.CalculateLowerDieSetThickness(thicknesses);
            Assert.AreEqual(expectedLowerDieSet, result, 0.01, "Feed height should equal lower die set when lift height is zero");
        }

        #endregion

        #region Position Calculation Tests

        [TestMethod]
        public void CalculateUpperShoeZPosition_StandardValues_ReturnsCorrectSum()
        {
            // Arrange
            var thicknesses = new ThicknessData
            {
                UpperShoeThk = 70.0,
                UpperPadThk = 27.0,
                PunHolderThk = 30.0,
                BottomPltThk = 25.0,
                StripperPltThk = 20.0,
                MatThk = 2.0
            };
            var expected = 174.0; // Sum of upper components

            // Act
            var result = _service.CalculateUpperShoeZPosition(thicknesses);

            // Assert
            Assert.AreEqual(expected, result, 0.01, "Upper shoe Z position should sum upper components");
        }

        [TestMethod]
        public void CalculateParallelBarZPosition_ReturnsNegativeValue()
        {
            // Arrange
            var thicknesses = new ThicknessData
            {
                DiePltThk = 40.0,
                LowerPadThk = 20.0,
                LowerShoeThk = 70.0
            };
            var expected = -130.0; // -(40 + 20 + 70)

            // Act
            var result = _service.CalculateParallelBarZPosition(thicknesses);

            // Assert
            Assert.AreEqual(expected, result, 0.01, "Parallel bar Z position should be negative sum of lower components");
        }

        [TestMethod]
        public void CalculateCommonPlateZPosition_ReturnsNegativeValue()
        {
            // Arrange
            var thicknesses = new ThicknessData
            {
                DiePltThk = 40.0,
                LowerPadThk = 20.0,
                LowerShoeThk = 70.0,
                ParallelBarThk = 15.0
            };
            var expected = -145.0; // -(40 + 20 + 70 + 15)

            // Act
            var result = _service.CalculateCommonPlateZPosition(thicknesses);

            // Assert
            Assert.AreEqual(expected, result, 0.01, "Common plate Z position should include parallel bar thickness");
        }

        #endregion

        #region Sub-Calculation Tests

        [TestMethod]
        public void CalculateDiePlt_LowPadThk_ReturnsCorrectSum()
        {
            // Arrange
            var thicknesses = new ThicknessData
            {
                DiePltThk = 40.0,
                LowerPadThk = 20.0
            };
            var expected = 60.0;

            // Act
            var result = _service.CalculateDiePlt_LowPadThk(thicknesses);

            // Assert
            Assert.AreEqual(expected, result, 0.01, "Die plate + lower pad thickness should sum correctly");
        }

        [TestMethod]
        public void CalculateLowerDieSetThickness_ReturnsCorrectSum()
        {
            // Arrange
            var thicknesses = new ThicknessData
            {
                LowerShoeThk = 70.0,
                LowerPadThk = 20.0,
                DiePltThk = 40.0,
                ParallelBarThk = 15.0,
                CommonPltThk = 40.0
            };
            var expected = 185.0;

            // Act
            var result = _service.CalculateLowerDieSetThickness(thicknesses);

            // Assert
            Assert.AreEqual(expected, result, 0.01, "Lower die set thickness should sum all lower components");
        }

        #endregion

        #region Utility Method Tests

        [TestMethod]
        public void SnapToNearestBand_Value45WithStandardBands_Returns50()
        {
            // Arrange
            var bands = new List<double> { 50.0, 60.0, 70.0, 80.0 };
            var value = 45.0;

            // Act
            var result = _service.SnapToNearestBand(value, bands);

            // Assert
            Assert.AreEqual(50.0, result, "Value 45 should snap to 50 band");
        }

        [TestMethod]
        public void SnapToNearestBand_Value55WithStandardBands_Returns60()
        {
            // Arrange
            var bands = new List<double> { 50.0, 60.0, 70.0, 80.0 };
            var value = 55.0; // Between 50 and 60, should snap to 60 (50 + 10)

            // Act
            var result = _service.SnapToNearestBand(value, bands);

            // Assert
            Assert.AreEqual(60.0, result, "Value 55 should snap to 60 (50 + 10)");
        }

        [TestMethod]
        public void SnapToNearestBand_ValueEqualsband_ReturnsSameBand()
        {
            // Arrange
            var bands = new List<double> { 50.0, 60.0, 70.0, 80.0 };
            var value = 60.0;

            // Act
            var result = _service.SnapToNearestBand(value, bands);

            // Assert
            Assert.AreEqual(60.0, result, "Value exactly matching band should return same band");
        }

        [TestMethod]
        public void SnapToNearestBand_ValueAboveAllBands_ReturnsOriginalValue()
        {
            // Arrange
            var bands = new List<double> { 50.0, 60.0, 70.0, 80.0 };
            var value = 95.0; // Above all bands

            // Act
            var result = _service.SnapToNearestBand(value, bands);

            // Assert
            Assert.AreEqual(95.0, result, "Value above all bands should return original value");
        }

        [TestMethod]
        public void SnapToNearestBand_EmptyBands_ReturnsOriginalValue()
        {
            // Arrange
            var bands = new List<double>();
            var value = 55.0;

            // Act
            var result = _service.SnapToNearestBand(value, bands);

            // Assert
            Assert.AreEqual(55.0, result, "Empty bands should return original value");
        }

        [TestMethod]
        public void SnapToNearestBand_NullBands_ReturnsOriginalValue()
        {
            // Arrange
            List<double> bands = null;
            var value = 55.0;

            // Act
            var result = _service.SnapToNearestBand(value, bands);

            // Assert
            Assert.AreEqual(55.0, result, "Null bands should return original value");
        }

        #endregion

        #region Validation Tests

        [TestMethod]
        public void ValidateThicknesses_AllPositiveValues_ReturnsTrue()
        {
            // Arrange
            var thicknesses = ThicknessData.CreateTestData(); // All positive values

            // Act
            var result = _service.ValidateThicknesses(thicknesses);

            // Assert
            Assert.IsTrue(result, "All positive thickness values should be valid");
        }

        [TestMethod]
        public void ValidateThicknesses_ZeroValues_ReturnsTrue()
        {
            // Arrange
            var thicknesses = new ThicknessData(); // All zero values

            // Act
            var result = _service.ValidateThicknesses(thicknesses);

            // Assert
            Assert.IsTrue(result, "Zero thickness values should be valid");
        }

        [TestMethod]
        public void ValidateThicknesses_NegativeValue_ReturnsFalse()
        {
            // Arrange
            var thicknesses = ThicknessData.CreateTestData();
            thicknesses.UpperShoeThk = -10.0; // Invalid negative value

            // Act
            var result = _service.ValidateThicknesses(thicknesses);

            // Assert
            Assert.IsFalse(result, "Negative thickness values should be invalid");
        }

        [TestMethod]
        public void ValidateThicknesses_NullData_ReturnsFalse()
        {
            // Arrange
            ThicknessData thicknesses = null;

            // Act
            var result = _service.ValidateThicknesses(thicknesses);

            // Assert
            Assert.IsFalse(result, "Null thickness data should be invalid");
        }

        #endregion

        #region Integration Tests

        [TestMethod]
        public void CalculationConsistency_DieHeightEqualsAllComponents()
        {
            // Arrange
            var thicknesses = ThicknessData.CreateTestData();

            // Act
            var dieHeight = _service.CalculateDieHeight(thicknesses);
            var upperShoePosition = _service.CalculateUpperShoeZPosition(thicknesses);
            var lowerDieSet = _service.CalculateLowerDieSetThickness(thicknesses);

            // Manual calculation for verification
            var expectedDieHeight = upperShoePosition + thicknesses.DiePltThk +
                                  thicknesses.LowerPadThk + thicknesses.LowerShoeThk +
                                  thicknesses.ParallelBarThk + thicknesses.CommonPltThk;

            // Assert
            Assert.AreEqual(expectedDieHeight, dieHeight, 0.01,
                "Die height should equal sum of all component calculations");
        }

        [TestMethod]
        public void RealisticManufacturingScenario_CompleteCalculation()
        {
            // Arrange - Realistic manufacturing values
            var thicknesses = new ThicknessData
            {
                UpperShoeThk = 75.0,
                UpperPadThk = 25.0,
                PunHolderThk = 32.0,
                BottomPltThk = 20.0,
                StripperPltThk = 18.0,
                MatThk = 1.5,
                DiePltThk = 45.0,
                LowerPadThk = 22.0,
                LowerShoeThk = 75.0,
                ParallelBarThk = 12.0,
                CommonPltThk = 35.0
            };
            var liftHeight = 120.0;

            // Act
            var dieHeight = _service.CalculateDieHeight(thicknesses);
            var punchLength = _service.CalculatePunchLength(thicknesses);
            var penetration = _service.CalculatePenetration(thicknesses);
            var feedHeight = _service.CalculateFeedHeight(thicknesses, liftHeight);

            // Assert - Verify all calculations are reasonable
            Assert.IsTrue(dieHeight > 300 && dieHeight < 400, "Die height should be in reasonable range");
            Assert.IsTrue(punchLength >= 50 && punchLength <= 90, "Punch length should be in standard range");
            Assert.IsTrue(penetration >= 0 && penetration <= 20, "Penetration should be reasonable");
            Assert.IsTrue(feedHeight > liftHeight, "Feed height should be greater than lift height");
            Assert.IsTrue(_service.ValidateThicknesses(thicknesses), "All thicknesses should be valid");
        }

        #endregion
    }
}