using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Controller;
using ToolingStructureCreation.Model;
using ToolingStructureCreation.View;

namespace ToolingStructureCreation.Services
{
    public class AsmCodeGeneratorServicecs : CodeGeneratorService, ICodeGeneratorServices
    {
        ToolingStructureType type = ToolingStructureType.ASSEMBLY;
        formToolStructure myForm;
        string dirPath;
        string codePrefix;
        string itemName;

        public AsmCodeGeneratorServicecs(Control control, ProjectInfo projectInfo, string itemName) : base(control, projectInfo)
        {
            myForm = control.GetForm;
            dirPath = myForm.GetPath;
            codePrefix = GetCodePrefix(projectInfo.DwgCodePrefix);
            this.itemName = itemName;
        }
        public override string AskDrawingCode()
        {
            return GenerateDrawingCode(type, dirPath, codePrefix);
        }

        public override string AskFileName()
        {
            return GenerateFileName(type, dirPath, codePrefix, itemName);
        }

        public override string AskFolderCode()
        {
            return GenerateFolderCode(type, dirPath, codePrefix, itemName);
        }
    }
}
