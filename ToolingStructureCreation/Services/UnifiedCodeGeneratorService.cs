using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Model;
using ToolingStructureCreation.View;
using ToolingStructureCreation.Controller;
using NXOpen.Layout2d;

namespace ToolingStructureCreation.Services
{
    public class UnifiedCodeGeneratorService : CodeGeneratorService, ICodeGeneratorServices
    {
        private readonly ToolingStructureType _type;
        private readonly formToolStructure _myForm;
        private readonly string _dirPath;
        private readonly string _codePrefix;
        private readonly string _itemName;
        private readonly int _stationNumber;

        /*/// <summary>
        /// Constructor for assembly and shoe types (no station number needed)
        /// </summary>
        public UnifiedCodeGeneratorService(Control control, ProjectInfo projectInfo, ToolingStructureType type, string itemName)
            : base(control, projectInfo)
        {
            _type = type;
            _myForm = control.GetForm;
            _dirPath = _myForm.GetPath;
            _codePrefix = GetCodePrefix(projectInfo.DwgCodePrefix);
            _itemName = itemName;
            _stationNumber = 0; // Default for non-plate types
        }*/

        /// <summary>
        /// Constructor for plate types (requires station number)
        /// </summary>
        public UnifiedCodeGeneratorService(Control control, ProjectInfo projectInfo, ToolingStructureType type, string itemName, int stationNumber = 0)
            : base(control, projectInfo)
        {
            _type = type;
            _myForm = control.GetForm;
            _dirPath = _myForm.GetPath;
            _codePrefix = GetCodePrefix(projectInfo.DwgCodePrefix);
            _itemName = itemName;
            _stationNumber = stationNumber;
        }

        public override string AskDrawingCode()
        {
            return _stationNumber > 0
                ? GenerateDrawingCode(_type, _dirPath, _codePrefix, _stationNumber)
                : GenerateDrawingCode(_type, _dirPath, _codePrefix);
        }

        public override string AskFileName()
        {
            return _stationNumber > 0
                ? GenerateFileName(_type, _dirPath, _codePrefix, _itemName, _stationNumber)
                : GenerateFileName(_type, _dirPath, _codePrefix, _itemName);
        }

        public override string AskFolderCode()
        {
            return _stationNumber > 0
                ? GenerateFolderCode(_type, _dirPath, _codePrefix, _itemName, _stationNumber)
                : GenerateFolderCode(_type, _dirPath, _codePrefix, _itemName);
        }

        /// <summary>
        /// Factory method to create assembly code generators
        /// </summary>
        public static UnifiedCodeGeneratorService CreateForAssembly(Control control, ProjectInfo projectInfo, string itemName)
        {
            return new UnifiedCodeGeneratorService(control, projectInfo, ToolingStructureType.ASSEMBLY, itemName);
        }

        /// <summary>
        /// Factory method to create shoe code generators
        /// </summary>
        public static UnifiedCodeGeneratorService CreateForShoe(Control control, ProjectInfo projectInfo, string itemName)
        {
            return new UnifiedCodeGeneratorService(control, projectInfo, ToolingStructureType.SHOE, itemName);
        }

        /// <summary>
        /// Factory method to create plate code generators
        /// </summary>
        public static UnifiedCodeGeneratorService CreateForPlate(Control control, ProjectInfo projectInfo, ToolingStructureType plateType, int stationNumber)
        {
            return new UnifiedCodeGeneratorService(control, projectInfo, plateType, plateType.ToString(), stationNumber);
        }

        /// <summary>
        /// Factory method to create material guide code generators
        /// </summary>
        public static UnifiedCodeGeneratorService CreateMaterialGuide(Control control, ProjectInfo projectInfo, ToolingStructureType type, string itemName, int stationNumber)
        {
            return new UnifiedCodeGeneratorService(control, projectInfo, type, itemName, stationNumber);
        }
    }
}
