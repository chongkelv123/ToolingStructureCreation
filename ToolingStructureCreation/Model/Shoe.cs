using NXOpen;
using NXOpen.Assemblies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Services;

namespace ToolingStructureCreation.Model
{
    public class Shoe
    {
        private string fileName;
        private double length;
        private double width;
        private double thickness;

        public const string UPPER_SHOE = "UPPER_SHOE";
        public const string LOWER_SHOE = "LOWER_SHOE";
        public const string TEMPLATE_SHOE_NAME = "3DA_Template_SHOE-V00.prt";
        public const string SHOE_PRESENTATION_NAME = "Shoe";

        public Shoe(string name, double length, double width, double thickness)
        {
            this.fileName = name;
            this.length = length;
            this.width = width;
            this.thickness = thickness;
        }

        public string GetShoeName() => fileName;
        public double GetShoeLength() => length;
        public double GetShoeWidth() => width;
        public double GetShoeHeight() => thickness;

        public void CreateNewShoe(string folderPath, ProjectInfo projectInfo, string drawingCode, string itemName)
        {
            // Create configuration for this shoe
            var config = ComponentCreationConfigs.CreateShoeConfig(
                folderPath, fileName, length, width, thickness, projectInfo, drawingCode, itemName
                );

            // Use the unified service to create the component
            var creationService = new ComponentCreationService();
            creationService.CreateComponent(config);
        }
        static public void Insert(Part workAssy, string compName, Point3d basePoint, string folderPath)
        {
            ComponentAssembly compAssy = workAssy.ComponentAssembly;
            PartLoadStatus status = null;
            int layer = 100;
            string referenceSetName = NXDrawing.MODEL;
            Matrix3x3 orientation = new Matrix3x3();
            orientation.Xx = 1.0;
            orientation.Xy = 0.0;
            orientation.Xz = 0.0;
            orientation.Yx = 0.0;
            orientation.Yy = 1.0;
            orientation.Yz = 0.0;
            orientation.Zx = 0.0;
            orientation.Zy = 0.0;
            orientation.Zz = 1.0;

            string partToAdd = $"{folderPath}{compName}{NXDrawing.EXTENSION}";

            if (
                compName.Contains(Shoe.LOWER_SHOE)
                || compName.Contains(ParallelBar.PARALLEL_BAR)
                || compName.Contains(CommonPlate.LOWER_COMMON_PLATE)
                )
            {
                layer = 200;
            }
            else if (compName.Contains("Striplayout"))
            {
                layer = 210;
            }

            NXOpen.Assemblies.Component component = compAssy.AddComponent(partToAdd, referenceSetName, compName, basePoint, orientation, layer, out status);

            NXOpen.Positioning.ComponentPositioner positioner = workAssy.ComponentAssembly.Positioner;
            NXOpen.Positioning.Network network = positioner.EstablishNetwork();
            NXOpen.Positioning.ComponentNetwork componentNetwork = ((NXOpen.Positioning.ComponentNetwork)network);
            NXOpen.Positioning.Constraint constraint = positioner.CreateConstraint(true);
            NXOpen.Positioning.ComponentConstraint componentConstraint = ((NXOpen.Positioning.ComponentConstraint)constraint);
            componentConstraint.ConstraintType = NXOpen.Positioning.Constraint.Type.Fix;
            NXOpen.Positioning.ConstraintReference constraintReference = componentConstraint.CreateConstraintReference(component, component, false, false, false);
            componentNetwork.Solve();

            NXOpen.Layer.StateInfo[] stateArray = new NXOpen.Layer.StateInfo[]
            {
                new NXOpen.Layer.StateInfo(layer, NXOpen.Layer.State.Selectable)
            };
            workAssy.Layers.ChangeStates(stateArray);
        }
    }
}
