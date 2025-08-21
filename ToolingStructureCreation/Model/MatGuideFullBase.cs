using NXOpen;
using NXOpen.Assemblies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ToolingStructureCreation.Model
{
    public abstract class MatGuideFullBase
    {
        public double Length { get;}
        public double Width { get;}
        public double Thickness { get;}
        public string FileName { get; set; }

        public const string TEMPLATE_MATGUIDEFULLFRONT_NAME = "3DA_Template_MATGUIDEFULL_FRONT-V00.prt";
        public const string TEMPLATE_MATGUIDEFULLREAR_NAME = "3DA_Template_MATGUIDEFULL_REAR-V00.prt";
        public const string MATGUIDEFULLFRONT_PRESENTATION_NAME = "MaterialGuideFullFront";
        public const string MATGUIDEFULLREAR_PRESENTATION_NAME = "MaterialGuideFullRear";

        public const string MATERIAL_GUIDE_FULL_REAR = "MATERIAL_GUIDE_FULL_REAR";
        public const string MATERIAL_GUIDE_FULL_FRONT = "MATERIAL_GUIDE_FULL_FRONT";

        public MatGuideFullBase(double length, double width, double thickness, string fileName = null)
        {
            Length = length;
            Width = width;
            Thickness = thickness;
            FileName = fileName;
        }

        public abstract void Create(string folderPath, ProjectInfo projectInfo, string drawingCode, string itemName);

        public static void Insert(Part workAssy, string compName, Point3d basePoint, string folderPath)
        {
            ComponentAssembly compAssy = workAssy.ComponentAssembly;
            PartLoadStatus status = null;
            int layer = 200;
            string referenceSetName = NXDrawing.MODEL;
            Matrix3x3 orientation = new Matrix3x3();
            orientation.Xx = 1.0;
            orientation.Xy = 0.0;
            orientation.Xz = 0.0;
            orientation.Yx = 0.0;
            orientation.Yy = 1.0;
            orientation.Yz = 0.0;

            string partToAdd = $"{folderPath}{compName}{NXDrawing.EXTENSION}";

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
