using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NXOpen;

namespace ToolingStructureCreation.Services
{
    public class FileManagerService
    {
        public static string GetCurrentDirectory(Part workPart)
        {
            var fullPath = workPart.FullPath;
            var directory = Path.GetDirectoryName(fullPath);

            return directory;
        }
    }
}
