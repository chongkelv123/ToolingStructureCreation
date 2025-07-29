using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Model
{
    public abstract class CommonPlateBase
    {
        private string fileName;
        private double length;
        private double width;
        private double thickness;

        public const string LOWER_COMMON_PLATE = "LOWER_COMMON_PLATE";

        public CommonPlateBase(double length, double width, double thickness, string fileName = null)
        {
            this.fileName = fileName;
            this.length = length;
            this.width = width;
            this.thickness = thickness;
        }
        public double GetLength() => length;        

        public double GetWidth() => width;
        public double GetThickness() => thickness;
        public string GetFileName() => fileName;

        public abstract void CreateNewCommonPlate(string folderPath, ProjectInfo projectInfo, string drawingCode, string itemName);        
    }
}
