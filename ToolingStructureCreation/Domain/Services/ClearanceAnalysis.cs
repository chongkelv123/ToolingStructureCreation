using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Domain.Services
{
    public class ClearanceAnalysis
    {
        public bool IsAcceptable { get; }
        public string Summary { get; }

        public ClearanceAnalysis(bool isAcceptable, string summary)
        {
            IsAcceptable = isAcceptable;
            Summary = summary ?? string.Empty;
        }

        public override string ToString()
        {
            return $"{(IsAcceptable ? "✓" : "⚠")} {Summary}";
        }
    }
}
