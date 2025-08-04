using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Domain.Services
{
    public class SketchValidationResult
    {
        public bool IsValid { get; }
        public string Message { get; }
        public SketchGeometry SketchInfo { get; }

        public SketchValidationResult(bool isValid, string message, SketchGeometry sketchInfo = null)
        {
            IsValid = isValid;
            Message = message ?? string.Empty;
            SketchInfo = sketchInfo;
        }

        public static SketchValidationResult Valid(SketchGeometry sketchInfo)
        {
            return new SketchValidationResult(true, "Valid rectangular sketch", sketchInfo);
        }

        public static SketchValidationResult Invalid (string message)
        {
            return new SketchValidationResult(false, message);
        }

        public override string ToString()
        {
            return IsValid ? $"Valid: {Message}" : $"Invalid: {Message}";
        }
    }
}
