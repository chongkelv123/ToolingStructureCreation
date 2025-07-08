using NXOpen;
using NXOpen.Assemblies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Model
{
    public class Plate
    {
        private string plateName;
        private double plateLength;
        private double plateWidth;
        private double plateThickness;        

        public const string TEMPLATE_PLATE_NAME = "3DA_Template_PLATE-V00.prt";                
        public const string PLATE = "Plate";
        NXDrawing drawing;

        public Plate(string name, double length, double width, double thickness, NXDrawing drawing)
        {
            this.plateName = name;
            this.plateLength = length;
            this.plateWidth = width;
            this.plateThickness = thickness;
            this.drawing = drawing;
        }

        public string GetPlateName()
        {
            return plateName;
        }

        public double GetPlateLength()
        {
            return plateLength;
        }

        public double GetPlateWidth()
        {
            return plateWidth;
        }

        public double GetPlateThickness()
        {
            return plateThickness;
        }

        public void CreateNewPlate(string folderPath, string stationNumber)
        {
            Session session = Session.GetSession();
            FileNew fileNew = session.Parts.FileNew();
            fileNew.TemplateFileName = TEMPLATE_PLATE_NAME;
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = NXDrawing.MODEL_TEMPLATE;
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = PLATE;
            fileNew.SetCanCreateAltrep(false);
            fileNew.NewFileName = $"{folderPath}{stationNumber}-{plateName}{NXDrawing.EXTENSION}";
            fileNew.MakeDisplayedPart = true;
            fileNew.DisplayPartOption = NXOpen.DisplayPartOption.AllowAdditional;
            NXObject plateObject;
            plateObject = fileNew.Commit();

            Part workPart = session.Parts.Work;
            Part displayPart = session.Parts.Display;

            fileNew.Destroy();

            session.ApplicationSwitchImmediate(NXDrawing.UG_APP_MODELING);

            NXOpen.Expression expressionPlateWidth = ((NXOpen.Expression)workPart.Expressions.FindObject("PlateWidth"));
            NXOpen.Expression expressionPlateLength = ((NXOpen.Expression)workPart.Expressions.FindObject("PlateLength"));
            NXOpen.Expression expressionPlateThk = ((NXOpen.Expression)workPart.Expressions.FindObject("PlateThk"));
            if (expressionPlateWidth == null)
            {
                drawing.ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'PlateWidth' not found.");
                return;
            }
            else if (expressionPlateLength == null)
            {
                drawing.ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'PlateLength' not found.");
                return;
            }
            else if (expressionPlateThk == null)
            {
                drawing.ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'PlateThk' not found.");
                return;
            }
            workPart.Expressions.EditExpression(expressionPlateWidth, GetPlateWidth().ToString());
            workPart.Expressions.EditExpression(expressionPlateLength, GetPlateLength().ToString());
            workPart.Expressions.EditExpression(expressionPlateThk, GetPlateThickness().ToString());

            NXOpen.Session.UndoMarkId undoMark = session.SetUndoMark(Session.MarkVisibility.Invisible, "Create New Plate");
            session.UpdateManager.DoUpdate(undoMark);

            /*
             * Change Color
             */
            NXOpen.BodyCollection bodyCollection = workPart.Bodies;
            foreach (NXOpen.Body body in bodyCollection)
            {
                if (plateName.Equals(NXDrawing.UPPER_PAD))
                {
                    body.Color = (int)PlateColor.UPPERPAD;
                }
                else if (plateName.Equals(NXDrawing.PUNCH_HOLDER))
                {
                    body.Color = (int)PlateColor.PUNCHHOLDER;
                }
                else if (plateName.Equals(NXDrawing.BOTTOMING_PLATE))
                {
                    body.Color = (int)PlateColor.BOTTOMINGPLATE;
                }
                else if (plateName.Equals(NXDrawing.STRIPPER_PLATE))
                {
                    body.Color = (int)PlateColor.STRIPPERPLATE;
                }
                else if (plateName.Equals(NXDrawing.DIE_PLATE))
                {
                    body.Color = (int)PlateColor.DIEPLATE;
                }
                else if (plateName.Equals(NXDrawing.LOWER_PAD))
                {
                    body.Color = (int)PlateColor.LOWERPAD;
                }
                else
                {
                    body.Color = (int)PlateColor.COMMONPLATE;
                }
            }

            BasePart.SaveComponents saveComponentParts = BasePart.SaveComponents.True;
            BasePart.CloseAfterSave close = BasePart.CloseAfterSave.True;
            workPart.Save(saveComponentParts, close);
        }

        static public void InsertPlate(Part workAssy, string compName, double cumThk, string folderPath)
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
