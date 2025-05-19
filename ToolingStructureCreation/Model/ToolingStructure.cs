using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Model
{
    public class ToolingStructure
    {
        public List<ToolingComponent> Components { get; set; } = new List<ToolingComponent>();
        public ToolingTemplate Template { get; set; }
        public BaseComponentInfo BaseInfo { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;

        // Methods for structure manipulation
        public void AddComponent(ToolingComponent component)
        {
            Components.Add(component);
        }

        public List<ToolingComponent> GetComponentsByType(ToolingComponentType type)
        {
            return Components.Where(c => c.Type == type).ToList();
        }

        // Statistics and information
        public int ComponentCount => Components.Count;
        public bool HasFixtures => Components.Any(c => c.Type == ToolingComponentType.Fixture);
        public bool HasClamps => Components.Any(c => c.Type == ToolingComponentType.Clamp);
        public int ClampCount => Components.Count(c => c.Type == ToolingComponentType.Clamp);
    
    }
}
