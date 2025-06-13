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

        public void CreateAssembly(Dictionary<string, double> plateList)
        {
            FileNew fileNew = session.Parts.FileNew();
            fileNew.TemplateFileName = "3DA_Template_STP-V00.prt";
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = "AssemblyTemplate";
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = "Assembly";
            fileNew.SetCanCreateAltrep(false);
            string fileName = "ToolAssembly";
            fileNew.NewFileName = $"{folderPath}{fileName}.prt";
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
                InsertComponent2(workPart, component.Key, cumThk);
            }
            BasePart.SaveComponents saveComponentParts = BasePart.SaveComponents.True;
            BasePart.CloseAfterSave save = BasePart.CloseAfterSave.False;
            workPart.Save(saveComponentParts, save);

            workPart.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Isometric, NXOpen.View.ScaleAdjustment.Fit);   
        }
        public void InsertComponent2(Part workAssy, string compName, double cumThk)
        {
            ComponentAssembly compAssy = workAssy.ComponentAssembly;
            PartLoadStatus status = null;
            int layer = 100;
            string referenceSetName = "MODEL";
            string componentName = "DiePlate1";
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

            //string fileName = "DiePlate1";
            string partToAdd = $"{folderPath}{compName}.prt";

            compAssy.AddComponent(partToAdd, referenceSetName, componentName, basePoint, orientation, layer, out status);

            NXOpen.Layer.StateInfo[] stateArray = new NXOpen.Layer.StateInfo[]
            {
                new NXOpen.Layer.StateInfo(100, NXOpen.Layer.State.Selectable)
            };
            workAssy.Layers.ChangeStates(stateArray);
        }

        /*public void InsertComponent(Part workPart)
        {
            NXOpen.Assemblies.AddComponentBuilder addComponentBuilder = workPart.AssemblyManager.CreateAddComponentBuilder();
            addComponentBuilder.SetAllowMultipleAssemblyLocations(false);

            string fileName = "DiePlate1";
            BasePart basePart = session.Parts.OpenBase($"C:\\CreateFolder\\Testing-Tooling-Structure\\{fileName}.prt", out _);
            addComponentBuilder.ReferenceSet = "MODEL";

            addComponentBuilder.Layer = 100;

            BasePart[] partouse = new BasePart[] {(Part)basePart};
            addComponentBuilder.SetPartsToAdd(partouse);         

            var root = workPart.ComponentAssembly.RootComponent;

            NXOpen.Assemblies.Component component = null;
            foreach (var comp in root.GetChildren())
            {
                if(comp.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                {
                    component = comp;                    
                }                
            }

            if(component == null)
            {
                return;
            }

            //NXObject[] moveableObj = new NXObject[] { component };
            NXOpen.Layer.StateInfo[] stateArray = new NXOpen.Layer.StateInfo[]
            {
                new NXOpen.Layer.StateInfo(100, NXOpen.Layer.State.Selectable)
            };
            workPart.Layers.ChangeStates(stateArray);

        }*/

        public void CreateNewPlate(string fileName, double thickness)
        {
            FileNew fileNew = session.Parts.FileNew();
            fileNew.TemplateFileName = "3DA_Template_PLATE-V00.prt";
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = "ModelTemplate";
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = "Plate";
            fileNew.SetCanCreateAltrep(false);
            fileNew.NewFileName = $"{folderPath}{fileName}.prt";
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
            workPart.Expressions.EditExpression(expressionPlateWidth, "300.0");
            workPart.Expressions.EditExpression(expressionPlateLength, "450.0");
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
    }
}
