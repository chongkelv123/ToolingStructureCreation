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
    public class ShoeCodeGeneratorService : CodeGeneratorService, ICodeGeneratorServices
    {        
        ToolingStructureType type = ToolingStructureType.SHOE;
        formToolStructure myForm;
        string dirPath;
        string codePrefix;
        string itemName;

        public ShoeCodeGeneratorService(Control control, ProjectInfo projectInfo) : base(control, projectInfo)
        {
            myForm = control.GetForm;
            dirPath = myForm.GetPath;
            codePrefix = GetCodePrefix(projectInfo.DwgCodePrefix);
            itemName = type.ToString();
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
