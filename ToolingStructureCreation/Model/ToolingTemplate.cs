using System;
using System.Collections.Generic;
using System.Linq;
using ToolingStructureCreation.Interfaces;

namespace ToolingStructureCreation.Model
{
    public class ToolingTemplate
    {
        // Basic identification
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Configuration
        public List<string> SupportedComponentTypes { get; set; } = new List<string>();
        public bool IncludesFixture { get; set; }
        public bool IncludesClamps { get; set; }
        public int DefaultClampCount { get; set; }

        // Default parameters
        public Dictionary<string, object> DefaultParameters { get; set; } = new Dictionary<string, object>();

        // Helper method to check if this template supports a specific component type
        public bool SupportsComponentType(ComponentType type)
        {
            return SupportedComponentTypes.Contains(type.ToString());
        }
    }
}
