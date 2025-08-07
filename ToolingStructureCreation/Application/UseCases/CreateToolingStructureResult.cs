using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Application.UseCases
{
    public class CreateToolingStructureResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ProjectPath { get; set; }
        public Exception Exception { get; set; }

        public List<string> CreatedPlates { get; set; }
        public List<string> CreatedShoes { get; set; }
        public List<string> CreatedParallelBars { get; set; }
        public List<string> CreatedCommonPlates { get; set; }
        public List<string> CreatedAssemblies { get; set; }

        public CreateToolingStructureResult()
        {
            CreatedPlates = new List<string>();
            CreatedShoes = new List<string>();
            CreatedParallelBars = new List<string>();
            CreatedCommonPlates = new List<string>();
            CreatedAssemblies = new List<string>();
        }

        public int TotalComponentsCreated =>
            CreatedPlates.Count + CreatedShoes.Count + CreatedParallelBars.Count +
            CreatedCommonPlates.Count + CreatedAssemblies.Count;
    }
}
