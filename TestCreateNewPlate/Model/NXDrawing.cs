using NXOpen;
using NXOpen.Annotations;
using NXOpen.Assemblies;
using NXOpen.CAE;
using NXOpen.CAE.Connections;
using NXOpen.Display;
using NXOpen.Features;
using NXOpen.Layout2d;
using NXOpen.UF;
using NXOpenUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestCreateNewPlate.Controller;

namespace TestCreateNewPlate.Model
{
    public enum PlateColor
    {
        UPPERSHOE = 127,
        UPPERPAD = 91,
        PUNCHHOLDER = 59,
        BOTTOMINGPLATE = 124,
        STRIPPERPLATE = 55,
        DIEPLATE = 108,
        LOWERPAD = 92,
        LOWERSHOE = 60,
        COMMONPLATE = 80,
    }
    public class NXDrawing
    {
        Session session;
        Part workPart;
        UI ui;
        UFSession ufs;
        Controller.Control control;

        string folderPath = "C:\\CreateFolder\\Testing-Tooling-Structure\\";

        const string LOWER_PAD = "LOWER_PAD";
        const string DIE_PLATE = "DIE_PLATE";
        const string MAT_THK = "mat_thk";
        const string STRIPPER_PLATE = "STRIPPER_PLATE";
        const string BOTTOMING_PLATE = "BOTTOMING_PLATE";
        const string PUNCH_HOLDER = "PUNCH_HOLDER";
        const string UPPER_PAD = "UPPER_PAD";

        public NXDrawing()
        {
        }

        public NXDrawing(Controller.Control control)
        {
            session = Session.GetSession();
            ufs = UFSession.GetUFSession();
            workPart = session.Parts.Work;
            ui = UI.GetUI();


            this.control = control;
        }

        public void ShowMessageBox(string title, NXMessageBox.DialogType msgboxType, string message)
        {
            ui.NXMessageBox.Show(title, msgboxType, message);
        }

        public void CreateStationAssembly(Dictionary<string, double> plateList, string stationNumber)
        {
            FileNew fileNew = session.Parts.FileNew();
            fileNew.TemplateFileName = "3DA_Template_STP-V00.prt";
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = "AssemblyTemplate";
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = "Assembly";
            fileNew.SetCanCreateAltrep(false);
            //string fileName = "ToolAssembly";
            fileNew.NewFileName = $"{folderPath}{stationNumber}-Assembly.prt";
            fileNew.MakeDisplayedPart = true;
            fileNew.DisplayPartOption = NXOpen.DisplayPartOption.AllowAdditional;
            NXObject plateObject;
            plateObject = fileNew.Commit();

            Part workPart = session.Parts.Work;
            Part displayPart = session.Parts.Display;

            fileNew.Destroy();

            session.ApplicationSwitchImmediate("UG_APP_MODELING");

            double cumThk = 0.0;
            foreach (var component in plateList)
            {
                cumThk += component.Value;
                if (component.Key.Equals(MAT_THK, StringComparison.OrdinalIgnoreCase))
                {
                    continue; // Skip the material thickness entry
                }
                string fileName = stationNumber + "-" + component.Key;
                InsertComponent2(workPart, fileName, cumThk);
            }
            BasePart.SaveComponents saveComponentParts = BasePart.SaveComponents.True;
            BasePart.CloseAfterSave save = BasePart.CloseAfterSave.True;
            workPart.Save(saveComponentParts, save);

            //workPart.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Isometric, NXOpen.View.ScaleAdjustment.Fit);   
        }

        public void InsertComponent2(Part workAssy, string compName, double cumThk)
        {
            ComponentAssembly compAssy = workAssy.ComponentAssembly;
            PartLoadStatus status = null;
            int layer = 100;
            string referenceSetName = "MODEL";
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

            string partToAdd = $"{folderPath}{compName}.prt";

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
                new NXOpen.Layer.StateInfo(100, NXOpen.Layer.State.Selectable)
            };
            workAssy.Layers.ChangeStates(stateArray);
        }

        public void CreateNewPlate(StationToolingStructure toolingStructure, string fileName, double thickness)
        {
            FileNew fileNew = session.Parts.FileNew();
            fileNew.TemplateFileName = "3DA_Template_PLATE-V00.prt";
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = "ModelTemplate";
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = "Plate";
            fileNew.SetCanCreateAltrep(false);
            fileNew.NewFileName = $"{folderPath}{toolingStructure.GetStationNumber()}-{fileName}.prt";
            fileNew.MakeDisplayedPart = true;
            fileNew.DisplayPartOption = NXOpen.DisplayPartOption.AllowAdditional;
            NXObject plateObject;
            plateObject = fileNew.Commit();

            Part workPart = session.Parts.Work;
            Part displayPart = session.Parts.Display;

            fileNew.Destroy();

            session.ApplicationSwitchImmediate("UG_APP_MODELING");

            NXOpen.Expression expressionPlateWidth = ((NXOpen.Expression)workPart.Expressions.FindObject("PlateWidth"));
            NXOpen.Expression expressionPlateLength = ((NXOpen.Expression)workPart.Expressions.FindObject("PlateLength"));
            NXOpen.Expression expressionPlateThk = ((NXOpen.Expression)workPart.Expressions.FindObject("PlateThk"));
            if (expressionPlateWidth == null)
            {
                ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'PlateWidth' not found.");
                return;
            }
            else if (expressionPlateLength == null)
            {
                ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'PlateLength' not found.");
                return;
            }
            else if (expressionPlateThk == null)
            {
                ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'PlateThk' not found.");
                return;
            }
            workPart.Expressions.EditExpression(expressionPlateWidth, toolingStructure.GetPlateWidth().ToString());
            workPart.Expressions.EditExpression(expressionPlateLength, toolingStructure.GetPlateLength().ToString());
            workPart.Expressions.EditExpression(expressionPlateThk, thickness.ToString());

            NXOpen.Session.UndoMarkId undoMark = session.SetUndoMark(Session.MarkVisibility.Invisible, "Create New Plate");
            session.UpdateManager.DoUpdate(undoMark);

            /*
             * Change Color
             */
            NXOpen.BodyCollection bodyCollection = workPart.Bodies;
            foreach (NXOpen.Body body in bodyCollection)
            {
                if (fileName.Equals(UPPER_PAD))
                {
                    body.Color = (int)PlateColor.UPPERPAD;
                }
                else if (fileName.Equals(PUNCH_HOLDER))
                {
                    body.Color = (int)PlateColor.PUNCHHOLDER;
                }
                else if (fileName.Equals(BOTTOMING_PLATE))
                {
                    body.Color = (int)PlateColor.BOTTOMINGPLATE;
                }
                else if (fileName.Equals(STRIPPER_PLATE))
                {
                    body.Color = (int)PlateColor.STRIPPERPLATE;
                }
                else if (fileName.Equals(DIE_PLATE))
                {
                    body.Color = (int)PlateColor.DIEPLATE;
                }
                else if (fileName.Equals(LOWER_PAD))
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

        public void CreateStationFactory(StationToolingStructure toolStructure)
        {
            var list = toolStructure.GetPlateThicknesses();
            foreach (var plate in list)
            {
                if (plate.Key.Equals("mat_thk", StringComparison.OrdinalIgnoreCase))
                {
                    // Skip material thickness, as it is not a plate
                    continue;
                }
                CreateNewPlate(toolStructure, plate.Key, plate.Value);
            }

            CreateStationAssembly(list, toolStructure.GetStationNumber());
        }

        public void CreateToolAssembly()
        {
            FileNew fileNew = session.Parts.FileNew();
            fileNew.TemplateFileName = "3DA_Template_STP-V00.prt";
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = "AssemblyTemplate";
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = "Assembly";
            fileNew.SetCanCreateAltrep(false);
            //string fileName = "ToolAssembly";
            fileNew.NewFileName = $"{folderPath}ToolingAssembly.prt";
            fileNew.MakeDisplayedPart = true;
            fileNew.DisplayPartOption = NXOpen.DisplayPartOption.AllowAdditional;
            NXObject plateObject;
            plateObject = fileNew.Commit();

            Part workPart = session.Parts.Work;
            Part displayPart = session.Parts.Display;

            fileNew.Destroy();

            session.ApplicationSwitchImmediate("UG_APP_MODELING");

            InsertStationAssembly(workPart, "Stn1-Assembly", 0.0);
            InsertStationAssembly(workPart, "Stn2-Assembly", 422.0);

            BasePart.SaveComponents saveComponentParts = BasePart.SaveComponents.True;
            BasePart.CloseAfterSave save = BasePart.CloseAfterSave.False;
            workPart.Save(saveComponentParts, save);

            workPart.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Isometric, NXOpen.View.ScaleAdjustment.Fit);
        }

        public void InsertStationAssembly(Part workAssy, string assyName, double distance)
        {
            ComponentAssembly compAssy = workAssy.ComponentAssembly;
            PartLoadStatus status = null;
            int layer = 100;
            string referenceSetName = "MODEL";
            Point3d basePoint = new Point3d(distance, 0.0, 0.0);
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

            string partToAdd = $"{folderPath}{assyName}.prt";

            NXOpen.Assemblies.Component component = compAssy.AddComponent(partToAdd, referenceSetName, assyName, basePoint, orientation, layer, out status);

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
                new NXOpen.Layer.StateInfo(100, NXOpen.Layer.State.Selectable)
            };
            workAssy.Layers.ChangeStates(stateArray);
        }
    }
}
        

