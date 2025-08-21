using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Layout2d;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Services;

namespace ToolingStructureCreation.Model
{
    public class Plate
    {
        private string fileName;
        private double length;
        private double width;
        private double thickness;

        public const string TEMPLATE_PLATE_NAME = "3DA_Template_PLATE-V00.prt";
        public const string PLATE_PRESENTATION_NAME = "Plate";
        public const string HRC = "52~54";
        public const string MATERIAL = "GOA";

        public Plate(string fileName, double length, double width, double thickness)
        {
            this.fileName = fileName;
            this.length = length;
            this.width = width;
            this.thickness = thickness;
        }

        public string GetPlateName() => fileName;
        public double GetPlateLength() => length;
        public double GetPlateWidth() => width;
        public double GetPlateThickness() => thickness;

        public void CreateNewPlate(string folderPath, ProjectInfo projectInfo, string drawingCode, string itemName)
        {
            // Create configuration for this plate            
            var config = ComponentCreationConfigs.CreatePlateConfig(
                folderPath,
                fileName,
                length,
                width,
                thickness,
                projectInfo,
                drawingCode,
                itemName);

            // Use the unified service to create the component
            var creationService = new ComponentCreationService();
            creationService.CreateComponent(config);
        }

        static public void Insert(Part workAssy, string compName, double cumThk, string folderPath)
        {
            ComponentAssembly compAssy = workAssy.ComponentAssembly;
            PartLoadStatus status = null;
            int layer = 100;
            string referenceSetName = NXDrawing.MODEL;
            Point3d basePoint = new Point3d(0.0, 0.0, cumThk);
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

            if (compName.Contains(NXDrawing.DIE_PLATE) || compName.Contains(NXDrawing.LOWER_PAD))
            {
                layer = 200;
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
