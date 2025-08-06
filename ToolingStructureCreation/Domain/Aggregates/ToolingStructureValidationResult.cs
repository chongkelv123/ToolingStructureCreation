using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Domain.Aggregates
{
    public class ToolingStructureValidationResult
    {
        public bool IsValid { get; }
        public string Summary { get; }
        public IReadOnlyList<string> Issues { get; }

        public ToolingStructureValidationResult(bool isValid, string summary, List<string> issues)
        {
            IsValid = isValid;
            Summary = summary ?? string.Empty;
            Issues = issues?.AsReadOnly() ?? new List<string>().AsReadOnly();
        }

        public override string ToString()
        {
            return IsValid ? $"✓ {Summary}" : $"⚠ {Summary}";
        }
    }
}
