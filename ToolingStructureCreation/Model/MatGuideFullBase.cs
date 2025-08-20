using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ToolingStructureCreation.Model
{
    public abstract class MatGuideFullBase
    {
        public double Length { get;}
        public double Width { get;}
        public double Thickness { get;}
        public string FileName { get; set; }

        public const string TEMPLATE_MATGUIDEFULLFRONT_NAME = "3DA_Template_MATGUIDEFULL_FRONT-V00.prt";
        public const string TEMPLATE_MATGUIDEFULLREAR_NAME = "3DA_Template_MATGUIDEFULL_REAR-V00.prt";
        public const string MATGUIDEFULLFRONT_PRESENTATION_NAME = "MaterialGuideFullFront";
        public const string MATGUIDEFULLREAR_PRESENTATION_NAME = "MaterialGuideFullRear";

        public MatGuideFullBase(double length, double width, double thickness, string fileName = null)
        {
            Length = length;
            Width = width;
            Thickness = thickness;
            FileName = fileName;
        }

        public abstract void Create(string folderPath, ProjectInfo projectInfo, string drawingCode, string itemName);
    }
}
