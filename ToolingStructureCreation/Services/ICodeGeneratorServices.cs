using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Model;

namespace ToolingStructureCreation.Services
{
    public interface ICodeGeneratorServices
    {
        string AskFolderCode();
        string AskDrawingCode();
        string AskFileName();
    }
}
