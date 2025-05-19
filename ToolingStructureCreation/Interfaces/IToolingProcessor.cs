using NXOpen;
using System.Collections.Generic;
using ToolingStructureCreation.Model;

namespace ToolingStructureCreation.Interfaces
{
    public interface IToolingProcessor
    {
        BaseComponentInfo ProcessBaseComponent(TaggedObject component, ComponentType type);
        List<ToolingComponent> CreateToolingComponents(
            BaseComponentInfo baseInfo,
            ToolingParameters parameters,
            IToolingComponentFactory componentFactory);
        ToolingStructure AssembleComponents(
            List<ToolingComponent> components,
            ToolingTemplate template);
    }
}
