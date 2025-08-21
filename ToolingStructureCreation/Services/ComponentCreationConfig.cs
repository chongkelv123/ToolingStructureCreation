using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using ToolingStructureCreation.Model;

namespace ToolingStructureCreation.Services
{
    /// <summary>
    /// Configuration for component creation to eliminate duplication across
    /// Plate, Shoe, CommonPlate, etc. creation methods
    /// </summary>
    public class ComponentCreationConfig
    {
        public string TemplateFileName { get; set; }
        public string PresentationName { get; set; }
        public string UndoDescription { get; set; }
        public string FolderPath { get; set; }
        public string FileName { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        public double Thickness { get; set; }
        public ProjectInfo ProjectInfo { get; set; }
        public string DrawingCode { get; set; }
        public string ItemName { get; set; }
        public Action<Part, string> ColorAssignmentAction { get; set; }
        public string PartPropertiesType { get; set; }
        public string Material { get; set; } = NXDrawing.S50C;
        public string HardnessOrGrade { get; set; } = NXDrawing.HYPHEN;
        public bool IsMatGuideFull { get; set; } = false;
    }
}
