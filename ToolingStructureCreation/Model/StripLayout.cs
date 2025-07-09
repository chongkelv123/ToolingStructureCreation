using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Model
{
    public class StripLayout
    {
        string fullPath;
        string fileNameWithoutExtension;
        Point3d position;
        public StripLayout(string fullPath, Point3d position)
        {
            this.fullPath = fullPath;
            this.position = position;
            fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(fullPath);
        }

        public string GetFullPath => fullPath;
        public Point3d GetPosition => position;
        public string GetFileNameWithoutExtension => fileNameWithoutExtension;
    }
}
