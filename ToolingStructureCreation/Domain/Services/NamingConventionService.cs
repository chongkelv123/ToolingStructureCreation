using NXOpen.Layout2d;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.Enums;
using ToolingStructureCreation.Domain.ValueObjects;

namespace ToolingStructureCreation.Domain.Services
{
    public class NamingConventionService
    {
        private readonly DrawingCode _baseDrawingCode;
        private readonly string _outputDirectory;

        public NamingConventionService(DrawingCode baseDrawingCode, string outputDirectory)
        {
            _baseDrawingCode = baseDrawingCode ?? throw new ArgumentNullException(nameof(baseDrawingCode));
            _outputDirectory = string.IsNullOrWhiteSpace(outputDirectory)
                ? throw new ArgumentException("Output directory cannot be null or empty.")
                : outputDirectory.Trim();
        }
        public ComponentNaming GeneratePlateNaming(PlateType plateType, int stationNumber)
        {
            var typeCode = GetPlateTypeCode(plateType);
            var stationPart = stationNumber.ToString("D2");
            var drawingCode = _baseDrawingCode.WithCode($"{stationPart}{typeCode:D2}");

            var itemName = FormatItemName(plateType.ToString());
            var folderCode = $"{drawingCode}_{plateType}";
            var fileName = GenerateFileName(folderCode);

            return new ComponentNaming(drawingCode, itemName, folderCode, fileName);
        }

        public ComponentNaming GenerateShoeNaming(ShoeType shoeType, int sequenceNumber = 1)
        {
            var typeCode = GetShoeTypeCode(shoeType);
            var drawingCode = _baseDrawingCode.WithCode($"00{typeCode:D2}");

            var itemName = $"{FormatItemName(shoeType.ToString())} SHOE";
            if (sequenceNumber > 1)
                itemName += $"-{sequenceNumber}";

            var folderCode = $"{drawingCode}_{shoeType}_SHOE";
            if (sequenceNumber > 1)
                folderCode += $"-{sequenceNumber}";

            var fileName = GenerateFileName(folderCode);

            return new ComponentNaming(drawingCode , itemName, folderCode, fileName);            
        }

        public ComponentNaming GenerateParallelBarNaming()
        {
            var drawingCode = _baseDrawingCode.WithCode("0001");
            var itemName = "PARALLEL BAR";
            var folderCode = $"{drawingCode}_PARALLEL_BAR";
            var fileName = GenerateFileName(folderCode);

            return new ComponentNaming(drawingCode, itemName, folderCode, fileName);
        }

        public ComponentNaming GenerateCommonPlateNaming(CommonPlateType plateType, int sequenceNumber = 1)
        {
            var typeCode = GetCommonPlateTypeCode(plateType);
            var drawingCode = _baseDrawingCode.WithCode($"00{typeCode:D2}");

            var itemName = "LOWER COMMON PLATE";
            if (plateType != CommonPlateType.Single)
            {
                itemName += $" {plateType.ToString().ToUpperInvariant()}";
                if (sequenceNumber > 1)
                    itemName += $"-{sequenceNumber}";
            }

            var folderCode = $"{drawingCode}_LOWER_COMMON_PLATE";
            if (plateType != CommonPlateType.Single)
                folderCode += $"_{plateType.ToString().ToUpperInvariant()}";
            if (sequenceNumber > 1)
                folderCode += $"-{sequenceNumber}";

            var fileName = GenerateFileName(folderCode);

            return new ComponentNaming(drawingCode, itemName, folderCode, fileName);
        }

        public ComponentNaming GenerateAssemblyNaming(string assemblyType, int stationNumber = 0)
        {
            var drawingCode = stationNumber > 0
                ? _baseDrawingCode.WithCode($"{stationNumber:D2}00")
                : _baseDrawingCode.WithCode("0000");

            var itemName = stationNumber > 0
                ? $"Stn{stationNumber}-Assembly"
                : assemblyType;

            var folderCode = $"{drawingCode}_{itemName.Replace("-", "_")}";
            var fileName = GenerateFileName(folderCode);

            return new ComponentNaming(drawingCode , itemName, folderCode, fileName);
        }        

        private string GenerateFileName(string baseName)
        {
            var version = DetermineNextVersion(baseName);
            return $"{baseName}{version}";
        }

        private string DetermineNextVersion(string baseName)
        {
            if (!Directory.Exists(_outputDirectory))
                return "-V00";

            try
            {
                var existingFiles = Directory.GetFiles(_outputDirectory, "*.prt")
                    .Select(Path.GetFileNameWithoutExtension)
                    .Where(name => name.StartsWith($"{baseName}-V", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (!existingFiles.Any())
                    return "-V00";

                var versions = new List<int>();
                foreach (var file in existingFiles)
                {
                    var versionPart = file.Substring(baseName.Length + 2); // Skip baseName + "-V"
                    if (int.TryParse(versionPart, out var version))
                        versions.Add(version);                    
                }

                var nextVersion = versions.Any() ? versions.Max() + 1 : 0;
                return $"-V{nextVersion:D2}";
            }
            catch
            {
                return "-V00";
            }
        }

        private static string FormatItemName(string name)
        {
            return name.Replace("_", " ").ToUpperInvariant();
        }

        private static int GetPlateTypeCode(PlateType plateType)
        {
            switch(plateType)
            {
                case PlateType.Upper_Pad:
                    return 2;
                case PlateType.Punch_Holder:
                    return 3;
                case PlateType.Bottoming_Plate:
                    return 4;
                case PlateType.Stripper_Plate:
                    return 5;
                case PlateType.Die_Plate:
                    return 6;
                case PlateType.Lower_Pad:
                    return 7;
                default:
                    return 1;
            }
        }
        private static int GetShoeTypeCode(ShoeType shoeType)
        {
            switch(shoeType)
            {
                case ShoeType.Upper:
                    return 1;
                case ShoeType.Lower:
                    return 1;
                default:
                    return 1;
            }
        }

        private static int GetCommonPlateTypeCode(CommonPlateType plateType)
        {
            switch(plateType)
            {
                case CommonPlateType.Single:
                    return 1;
                case CommonPlateType.DoubleLeft:
                    return 1;
                case CommonPlateType.DoubleRight:
                    return 1;
                default:
                    return 1;
            }
        }        
    }
}
