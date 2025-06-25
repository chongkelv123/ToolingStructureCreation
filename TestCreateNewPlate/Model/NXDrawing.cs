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

        public void CreateStationAssembly(Dictionary<string, double> plateList, string stationNumber, string folderPath)
        {
            FileNew fileNew = session.Parts.FileNew();
            fileNew.TemplateFileName = "3DA_Template_STP-V00.prt";
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = "AssemblyTemplate";
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = "Assembly";
            fileNew.SetCanCreateAltrep(false);            
            fileNew.NewFileName = $"{folderPath}{stationNumber}-Assembly.prt";
            fileNew.MakeDisplayedPart = true;
            fileNew.DisplayPartOption = NXOpen.DisplayPartOption.AllowAdditional;
            NXObject plateObject;
            plateObject = fileNew.Commit();

            Part workPart = session.Parts.Work;
            Part displayPart = session.Parts.Display;

            fileNew.Destroy();

            session.ApplicationSwitchImmediate("UG_APP_MODELING");

            workPart.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Isometric, NXOpen.View.ScaleAdjustment.Fit);

            double cumThk = 0.0;
            foreach (var component in plateList)
            {
                cumThk += component.Value;
                if (component.Key.Equals(MAT_THK, StringComparison.OrdinalIgnoreCase))
                {
                    continue; // Skip the material thickness entry
                }
                string fileName = stationNumber + "-" + component.Key;
                InsertPlate(workPart, fileName, cumThk, folderPath);
            }
            BasePart.SaveComponents saveComponentParts = BasePart.SaveComponents.True;
            BasePart.CloseAfterSave save = BasePart.CloseAfterSave.True;
            workPart.Save(saveComponentParts, save);            
        }

        public void InsertPlate(Part workAssy, string compName, double cumThk, string folderPath)
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

        public void CreateToolAssembly(string folderPath)
        {
            FileNew fileNew = session.Parts.FileNew();
            fileNew.TemplateFileName = "3DA_Template_STP-V00.prt";
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = "AssemblyTemplate";
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = "Assembly";
            fileNew.SetCanCreateAltrep(false);            
            fileNew.NewFileName = $"{folderPath}ToolingAssembly.prt";
            fileNew.MakeDisplayedPart = true;
            fileNew.DisplayPartOption = NXOpen.DisplayPartOption.AllowAdditional;
            NXObject plateObject;
            plateObject = fileNew.Commit();

            Part workPart = session.Parts.Work;
            Part displayPart = session.Parts.Display;

            fileNew.Destroy();

            session.ApplicationSwitchImmediate("UG_APP_MODELING");

            workPart.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Isometric, NXOpen.View.ScaleAdjustment.Fit);

            InsertStationAssembly(workPart, "Stn1-Assembly", 0.0, folderPath);
            InsertStationAssembly(workPart, "Stn2-Assembly", 422.0, folderPath);

            BasePart.SaveComponents saveComponentParts = BasePart.SaveComponents.True;
            BasePart.CloseAfterSave save = BasePart.CloseAfterSave.False;
            workPart.Save(saveComponentParts, save);

            workPart.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Isometric, NXOpen.View.ScaleAdjustment.Fit);
        }

        public void InsertStationAssembly(Part workAssy, string assyName, double distance, string folderPath)
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

        public Session GetSession() 
        { 
            return session;
        }
        public UI GetUI() 
        { 
            return ui; 
        }
        public UFSession GetUFSession() 
        { 
            return ufs; 
        }
        public Part GetWorkPart() 
        { 
            return workPart; 
        }
    }
}
        

