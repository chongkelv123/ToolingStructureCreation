using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Domain.ValueObjects;

namespace ToolingStructureCreation.Domain.Services
{
    public class ComponentNaming
    {
        public DrawingCode DrawingCode { get; }
        public string ItemName { get; }
        public string FolderCode { get; }
        public string FileName { get; }

        public ComponentNaming(DrawingCode drawingCode, string itemName, string folderCode, string fileName)
        {
            DrawingCode = drawingCode ?? throw new ArgumentNullException(nameof(drawingCode));
            ItemName = string.IsNullOrWhiteSpace(itemName) ? throw new ArgumentException("Item name cannot be null or empty.") : itemName.Trim();
            FolderCode = string.IsNullOrWhiteSpace(folderCode) ? throw new ArgumentException("Folder code cannot be null or empty.") : folderCode.Trim();
            FileName = string.IsNullOrWhiteSpace(fileName) ? throw new ArgumentException("File name cannot be null or empty.") : fileName.Trim();
        }
        public override string ToString()
        {
            return $"{DrawingCode} - {ItemName} ({FileName})";
        }
    }
}
