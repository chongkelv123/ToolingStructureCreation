using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToolingStructureCreation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Model;

namespace ToolingStructureCreation.Services.Tests
{
    [TestClass()]
    public class CodeGeneratorServiceTests
    {
        [TestMethod()]
        public void AskVersionTest_V01()
        {
            // Arrange
            string baseName = "CSLG40011S-43T_3DStripLayout";
            string dirPath = "C:\\CreateFolder\\Testing-Tooling-Structure\\test";
            // Act
            string result = CodeGeneratorService.AskVersion(baseName, dirPath);
            // Assert
            Assert.AreEqual("-V01", result);
        }

        [TestMethod()]
        public void AskVersionTest_V00()
        {
            // Arrange
            string baseName = "CSLG40011S-43T_3DStripLayout";
            string dirPath = "C:\\CreateFolder\\Testing-Tooling-Structure\\test2";
            // Act
            string result = CodeGeneratorService.AskVersion(baseName, dirPath);
            // Assert
            Assert.AreEqual("-V00", result);
        }

        [TestMethod()]
        public void AskVersionTest_V11()
        {
            // Arrange
            string baseName = "_CSLG40011S-43T_3DStripLayout";
            string dirPath = "C:\\CreateFolder\\Testing-Tooling-Structure\\test";
            // Act
            string result = CodeGeneratorService.AskVersion(baseName, dirPath);
            // Assert
            Assert.AreEqual("-V11", result);
        }

        [TestMethod()]
        public void ReplaceSpacesWithUnderscoreTest_MiddleWhiteSpace()
        {
            //Arrange
            string input = "CSLG 40011S-43T 3DStripLayout";
            // Act
            string result = CodeGeneratorService.ReplaceSpacesWithUnderscore(input);
            // Assert
            Assert.AreEqual("CSLG_40011S-43T_3DStripLayout", result);
        }

        [TestMethod()]
        public void ReplaceSpacesWithUnderscoreTest_SideWhiteSpace()
        {
            //Arrange
            string input = " CSLG 40011S-43T 3DStripLayout ";
            // Act
            string result = CodeGeneratorService.ReplaceSpacesWithUnderscore(input);
            // Assert
            Assert.AreEqual("CSLG_40011S-43T_3DStripLayout", result);
        }

        [TestMethod()]
        public void GenerateNextNumberTest_WithoutIncreament()
        {
            // Arrange
            int currentNumber = 5;
            // Act
            int result = CodeGeneratorService.GenerateNextNumber(currentNumber);
            // Assert
            Assert.AreEqual(6, result);
        }

        [TestMethod()]
        public void GenerateNextNumberTest_WithIncreament()
        {
            // Arrange
            int currentNumber = 5;
            int increment = 2;
            // Act
            int result = CodeGeneratorService.GenerateNextNumber(currentNumber, increment);
            // Assert
            Assert.AreEqual(7, result);
        }

        [TestMethod()]
        public void TruncateInputTest_PrefixMoreThan6()
        {
            // Arrange
            string input = "43JR0001000-2401-";
            // Act
            var result = CodeGeneratorService.NormalizeDrawingCodeParts(input);
            // Assert
            Assert.AreEqual("43JR00", result.Item1);
            Assert.AreEqual("2401", result.Item2);
        }

        [TestMethod()]
        public void TruncateInputTest_PrefixLessthen2()
        {
            // Arrange
            string input = "4-2401-";
            // Act
            var result = CodeGeneratorService.NormalizeDrawingCodeParts(input);
            // Assert
            Assert.AreEqual("4xxxxx", result.Item1);
            Assert.AreEqual("2401", result.Item2);
        }

        [TestMethod()]
        public void TruncateInputTest_PrefixLessthen4()
        {
            // Arrange
            string input = "43J-2401-";
            // Act
            var result = CodeGeneratorService.NormalizeDrawingCodeParts(input);
            // Assert
            Assert.AreEqual("43Jxxx", result.Item1);
            Assert.AreEqual("2401", result.Item2);
        }

        [TestMethod()]
        public void GetDrawingCodeTest_ValidInput()
        {
            // Arrange
            string input = "43JR00-2401-0401_UPPER_PAD-V01";
            string expected = "0401";
            // Act
            var result = CodeGeneratorService.GetDrawingCode(input);
            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void GetDrawingCodeTest_InValidInput_Not4digit()
        {
            // Arrange
            string input = "43JR00-2401-040A-UPPER_PAD-V01";
            string expected = "Extracted drawing code is invalid. Expected a 4-digit number.";
            //string expected = "0401";
            // Act
            var ex = Assert.ThrowsException<ArgumentException>(() => CodeGeneratorService.GetDrawingCode(input));
            // Assert            
            Assert.AreEqual(ex.Message, expected);
        }
        [TestMethod()]
        public void GetDrawingCodeTest_InValidInput_NotValidFormat()
        {
            // Arrange
            string input = "43JR0024010401UPPER_PAD-V01";
            string expected = "Input string is not in a valid drawing code format.";
            // Act
            var ex = Assert.ThrowsException<ArgumentException>(() => CodeGeneratorService.GetDrawingCode(input));
            // Assert            
            Assert.AreEqual(ex.Message, expected);
        }

        [DataTestMethod]
        [DataRow(ToolingStructureType.SHOE, 1, "0101")]
        [DataRow(ToolingStructureType.ACCESSORIES, 1, "0130")]
        [DataRow(ToolingStructureType.UPPER_PAD_SPACER, 1, "0101")]
        [DataRow(ToolingStructureType.UPPER_PAD, 1, "0102")]
        [DataRow(ToolingStructureType.PUNCH_HOLDER, 2, "0203")]
        [DataRow(ToolingStructureType.BOTTOMING_PLATE, 2, "0204")]
        [DataRow(ToolingStructureType.STRIPPER_PLATE, 2, "0205")]
        [DataRow(ToolingStructureType.DIE_PLATE, 5, "0506")]
        [DataRow(ToolingStructureType.LOWER_PAD, 5, "0507")]
        [DataRow(ToolingStructureType.LOWER_PAD, 10, "1007")]
        [DataRow(ToolingStructureType.LOWER_PAD_SPACER, 11, "1108")]
        [DataRow(ToolingStructureType.INSERT, 1, "0111")]
        [DataRow(ToolingStructureType.INSERT, 2, "0211")]
        [DataRow(ToolingStructureType.INSERT, 12, "1211")]
        [DataRow(ToolingStructureType.ASSEMBLY, 0, "0000")]

        public void GetDrawingCodeFromType_ValidInputs_ReturnsExpectedCode(ToolingStructureType type, int stnNumber, string expected)
        {
            var result = CodeGeneratorService.GetDrawingCodeFromType(type, stnNumber);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void GetDrawingCodeFromType_DefaultStationNumber_ReturnsExpected()
        {
            // Arrange
            ToolingStructureType type = ToolingStructureType.SHOE;
            string expected = "0001";
            // Act
            var result = CodeGeneratorService.GetDrawingCodeFromType(type);
            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void GetDrawingCodeFromType_InvalidEnum_ThrowsException()
        {
            // Arrange
            ToolingStructureType invalidType = (ToolingStructureType)999; // Invalid enum value            
            // Assert is handled by ExpectedException
            CodeGeneratorService.GetDrawingCodeFromType(invalidType);
        }

        [TestMethod()]
        public void AskNextRunningNumberTest_Valid_ShoeNextNumber()
        {
            // Arrange
            string dir = "C:\\CreateFolder\\Testing-Tooling-Structure\\test_runNumber";
            string codePrefix = "40XC00-2401-";
            string expected = "0004";
            // Act
            string result = CodeGeneratorService.AskNextRunningNumber(ToolingStructureType.SHOE, dir, codePrefix);
            // Assert
            Assert.AreEqual(expected, result);
        }
        [TestMethod()]
        public void AskNextRunningNumberTest_NoAccessoriesFile_ReturnsDefaultTypeCode()
        {
            // Arrange
            string dir = "C:\\CreateFolder\\Testing-Tooling-Structure\\test_runNumber";
            string codePrefix = "40XC00-2401-";
            string expected = "0030";
            // Act
            string result = CodeGeneratorService.AskNextRunningNumber(ToolingStructureType.ACCESSORIES, dir, codePrefix);
            // Assert
            Assert.AreEqual(expected, result);
        }
        [TestMethod()]
        public void AskNextRunningNumberTest_InsertStation1_ReturnsNextCode()
        {
            // Arrange
            string dir = "C:\\CreateFolder\\Testing-Tooling-Structure\\test_runNumber";
            string codePrefix = "40XC00-2401-";
            var station = 1;
            string expected = "0116";
            // Act
            string result = CodeGeneratorService.AskNextRunningNumber(ToolingStructureType.INSERT, dir, codePrefix, station);
            // Assert
            Assert.AreEqual(expected, result);
        }
        [TestMethod()]
        public void AskNextRunningNumberTest_InsertStation5_ReturnsNextCode()
        {
            // Arrange
            string dir = "C:\\CreateFolder\\Testing-Tooling-Structure\\test_runNumber";
            string codePrefix = "40XC00-2401-";
            var station = 5;
            string expected = "0537";
            // Act
            string result = CodeGeneratorService.AskNextRunningNumber(ToolingStructureType.INSERT, dir, codePrefix, station);
            // Assert
            Assert.AreEqual(expected, result);
        }
        [TestMethod()]
        public void AskNextRunningNumberTest_PunHolderStation5_ReturnsNextCode()
        {
            // Arrange
            string dir = "C:\\CreateFolder\\Testing-Tooling-Structure\\test_runNumber";
            string codePrefix = "40XC00-2401-";
            var station = 5;
            string expected = "0503";
            // Act
            string result = CodeGeneratorService.AskNextRunningNumber(ToolingStructureType.PUNCH_HOLDER, dir, codePrefix, station);
            // Assert
            Assert.AreEqual(expected, result);
        }
        [TestMethod()]
        public void AskNextRunningNumberTest_DiePlateStation10_ReturnsNextCode()
        {
            // Arrange
            string dir = "C:\\CreateFolder\\Testing-Tooling-Structure\\test_runNumber";
            string codePrefix = "40XC00-2401-";
            var station = 10;
            string expected = "1006";
            // Act
            string result = CodeGeneratorService.AskNextRunningNumber(ToolingStructureType.DIE_PLATE, dir, codePrefix, station);
            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void GenerateDrawingCodeTest_Shoe_ReturnLatestDrawingCode()
        {
            // Arrange
            var type = ToolingStructureType.SHOE;
            var dirPath = "C:\\CreateFolder\\Testing-Tooling-Structure";
            var codePrefix = "40XC00-2401-";
            var expected = "40XC00-2401-0002";

            //Act
            var result = CodeGeneratorService.GenerateDrawingCode(type, dirPath, codePrefix);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void GenerateFileNameTest_UpperShoe_GetValidFilename()
        {
            // Arrange
            var type = ToolingStructureType.SHOE;
            var dirPath = "C:\\CreateFolder\\Testing-Tooling-Structure";
            var codePrefix = "40XC00-2401-";
            var itemName = Shoe.UPPER_SHOE;
            var expected = $"40XC00-2401-0002_{itemName}-V00";
            // Act
            var result = CodeGeneratorService.GenerateFileName(type, dirPath, codePrefix, itemName);
            // Assert
            Assert.AreEqual(expected, result);
        }
        [TestMethod()]
        public void GenerateFileNameTest_PunchHolder3_GetValidFilename()
        {
            // Arrange
            var type = ToolingStructureType.PUNCH_HOLDER;
            var dirPath = "C:\\CreateFolder\\Testing-Tooling-Structure\\test_runNumber";
            var codePrefix = "40XC00-2401-";
            var itemName = type.ToString();
            var stnNumber = 3;
            var expected = $"40XC00-2401-0303_{itemName}-V00";
            // Act
            var result = CodeGeneratorService.GenerateFileName(type, dirPath, codePrefix, itemName, stnNumber);
            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}