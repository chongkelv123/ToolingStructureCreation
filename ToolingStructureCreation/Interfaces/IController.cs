using ToolingStructureCreation.Model;

namespace ToolingStructureCreation.Interfaces
{
    public interface IController
    {
        void Initialize();
        void Start(ToolingParameters parameter);
    }
}
