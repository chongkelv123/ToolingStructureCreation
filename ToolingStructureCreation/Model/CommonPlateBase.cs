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
        private double Length;
        private double Width;
        private double Thickness;

        public const string LOWER_COMMON_PLATE = "LOWER_COMMON_PLATE";

        public CommonPlateBase(double length, double width, double thickness, string fileName = null)
        {
            this.fileName = fileName;
            this.Length = length;
            this.Width = width;
            this.Thickness = thickness;
        }
        public double GetLength() => Length;        

        public double GetWidth() => Width;
        public double GetThickness() => Thickness;
        public string GetFileName() => fileName;

        public abstract void CreateNewCommonPlate(string folderPath);        
    }
}
