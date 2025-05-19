using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Model;

namespace ToolingStructureCreation.Interfaces
{
    public interface IToolingComponentFactory
    {
        ToolingComponent CreateComponent(
            ToolingComponentType componentType,
            ComponentParameters parameters);

        // Specialized creation methods
        ToolingComponent CreatePlateComponent(BaseComponentInfo baseInfo, ComponentParameters parameters);
        ToolingComponent CreateShoeComponent(BaseComponentInfo baseInfo, ComponentParameters parameters);
        ToolingComponent CreateFixtureComponent(BaseComponentInfo baseInfo, ComponentParameters parameters);
        ToolingComponent CreateClampComponent(BaseComponentInfo baseInfo, ComponentParameters parameters, int index);
    
    }
}
