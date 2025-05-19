using NXOpen;
using ToolingStructureCreation.Model;

namespace ToolingStructureCreation.Interfaces
{
    /// <summary>
    /// Interface for selecting sketches.
    /// </summary>
    public interface INXSelectionService
    {
        TaggedObject SelectComponent(ComponentType type);
        bool IsBaseComponentSelected { get; }
        TaggedObject GetSelectedComponent();
    }

    public enum ComponentType
    {
        PlateSketch,
        ShoeSketch        
    }
}
