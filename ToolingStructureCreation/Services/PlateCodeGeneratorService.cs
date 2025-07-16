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
    public class PlateCodeGeneratorService : CodeGeneratorService, ICodeGeneratorServices
    {
        ToolingStructureType type;
        formToolStructure myForm;
        string dirPath;
        string codePrefix;
        string itemName;
        int stnNumber;

        public PlateCodeGeneratorService(Control control, ProjectInfo projectInfo, ToolingStructureType type, int stnNumber) : base(control, projectInfo)
        {
            this.type = type;
            myForm = control.GetForm;
            dirPath = myForm.GetPath;
            codePrefix = GetCodePrefix(projectInfo.DwgCodePrefix);
            itemName = type.ToString();
            this.stnNumber = stnNumber;
        }

        public override string AskDrawingCode()
        {
            return GenerateDrawingCode(type, dirPath, codePrefix, stnNumber);
        }

        public override string AskFileName()
        {
            return GenerateFileName(type, dirPath, codePrefix, itemName, stnNumber);
        }

        public override string AskFolderCode()
        {
            return GenerateFolderCode(type, dirPath, codePrefix, itemName, stnNumber);
        }
    }
}
