using NXOpen;
using NXOpen.Assemblies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Model
{
    public class ToolingAssembly
    {
        public Dictionary<string, double> PlateThicknesses { get; set; }
        double plateWidth;
        double plateLength;
        string stationNumber;
        NXDrawing drawing;
        string folderPath;
        static Session session;

        public const string TEMPLATE_STP_NAME = "3DA_Template_STP-V00.prt";
        public const string ASSEMBLY_TEMPLATE = "AssemblyTemplate";
        public const string ASSEMBLY = "Assembly";

        public ToolingAssembly(double plateWidth, double plateLength, string stationNumber, NXDrawing drawing, string folderPath, Dictionary<string, double> plateThicknesses)
        {
            this.plateWidth = plateWidth;
            this.plateLength = plateLength;
            this.stationNumber = stationNumber;
            this.drawing = drawing;
            this.folderPath = folderPath;
            session = drawing.GetSession();

            PlateThicknesses = plateThicknesses ?? new Dictionary<string, double>();
        }

        public double GetTotalThickness()
        {
            double totalThickness = 0.0;
            foreach (var plate in PlateThicknesses)
            {
                if (!plate.Key.Equals(NXDrawing.MAT_THK, StringComparison.OrdinalIgnoreCase))
                {
                    totalThickness += plate.Value;
                }
            }
            return totalThickness;
        }

        public double GetPlateWidth()
        {
            return plateWidth;
        }

        public double GetPlateLength()
        {
            return plateLength;
        }

        public string GetStationNumber()
        {
            return stationNumber;
        }

        /*public void CreateStationFactory()
        {
            if (PlateThicknesses == null)
            {
                throw new InvalidOperationException("PlateThicknesses dictionary is not initialized. Please provide a valid dictionary of plate thicknesses.");
            }
            foreach (var plt in PlateThicknesses)
            {
                if (plt.Key.Equals(NXDrawing.MAT_THK, StringComparison.OrdinalIgnoreCase))
                {
                    // Skip material thickness, as it is not a plate
                    continue;
                }
                Plate plate = new Plate(plt.Key, GetPlateLength(), GetPlateWidth(), plt.Value);
                plate.CreateNewPlate(folderPath, stationNumber);
            }

            CreateStationAssembly(PlateThicknesses, GetStationNumber(), folderPath);
        }*/

        /*public static void CreateStationAssembly(Dictionary<string, double> plateList, string stationNumber, string folderPath)
        {
            Session session = Session.GetSession();
            FileNew fileNew = session.Parts.FileNew();
            fileNew.TemplateFileName = TEMPLATE_STP_NAME;
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = ASSEMBLY_TEMPLATE;
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = ASSEMBLY;
            fileNew.SetCanCreateAltrep(false);
            fileNew.NewFileName = $"{folderPath}{stationNumber}-Assembly{NXDrawing.EXTENSION}";
            fileNew.MakeDisplayedPart = true;
            fileNew.DisplayPartOption = NXOpen.DisplayPartOption.AllowAdditional;
            NXObject plateObject;
            plateObject = fileNew.Commit();

            Part workPart = session.Parts.Work;
            Part displayPart = session.Parts.Display;

            fileNew.Destroy();

            session.ApplicationSwitchImmediate(NXDrawing.UG_APP_MODELING);

            workPart.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Isometric, NXOpen.View.ScaleAdjustment.Fit);

            double cumThk = 0.0;
            foreach (var component in plateList)
            {
                cumThk += component.Value;
                if (component.Key.Equals(NXDrawing.MAT_THK, StringComparison.OrdinalIgnoreCase))
                {
                    continue; // Skip the material thickness entry
                }                
                string fileName = component.Key;
                Plate.InsertPlate(workPart, fileName, cumThk, folderPath);
            }
            BasePart.SaveComponents saveComponentParts = BasePart.SaveComponents.True;
            BasePart.CloseAfterSave save = BasePart.CloseAfterSave.True;
            workPart.Save(saveComponentParts, save);
        }*/
        /*static public void CreateToolAssembly(string folderPath)
        {
            FileNew fileNew = session.Parts.FileNew();
            fileNew.TemplateFileName = TEMPLATE_STP_NAME;
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = ASSEMBLY_TEMPLATE;
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = ASSEMBLY;
            fileNew.SetCanCreateAltrep(false);
            fileNew.NewFileName = $"{folderPath}ToolingAssembly{NXDrawing.EXTENSION}";
            fileNew.MakeDisplayedPart = true;
            fileNew.DisplayPartOption = NXOpen.DisplayPartOption.AllowAdditional;
            NXObject plateObject;
            plateObject = fileNew.Commit();

            Part workAssy = session.Parts.Work;
            Part displayPart = session.Parts.Display;

            fileNew.Destroy();

            session.ApplicationSwitchImmediate(NXDrawing.UG_APP_MODELING);

            workAssy.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Isometric, NXOpen.View.ScaleAdjustment.Fit);

            InsertStationAssembly(workAssy, "Stn1-Assembly", new Point3d(0.0, 0.0, 0.0), folderPath);
            InsertStationAssembly(workAssy, "Stn2-Assembly", new Point3d(422.0, 0, 0), folderPath);
            InsertStationAssembly(workAssy, "Stn3-Assembly", new Point3d(924.0, 0, 0), folderPath);
            InsertStationAssembly(workAssy, "Stn4-Assembly", new Point3d(1376.0, 0, 0), folderPath);

            Shoe.Insert(workAssy, Shoe.UPPER_SHOE, new Point3d(-17.0, 0.0, 234.55), folderPath);
            Shoe.Insert(workAssy, Shoe.LOWER_SHOE, new Point3d(-17.0, 0.0, 0.0), folderPath);

            Shoe.Insert(workAssy, ParallelBar.PARALLEL_BAR, new Point3d(30.0, 0.0, -70.0), folderPath);
            Shoe.Insert(workAssy, ParallelBar.PARALLEL_BAR, new Point3d(908.0, 0.0, -70.0), folderPath);
            Shoe.Insert(workAssy, ParallelBar.PARALLEL_BAR, new Point3d(1786.0, 0.0, -70.0), folderPath);

            Shoe.Insert(workAssy, CommonPlate.LOWER_COMMON_PLATE, new Point3d(908.0, 0.0, -225.0), folderPath);

            BasePart.SaveComponents saveComponentParts = BasePart.SaveComponents.True;
            BasePart.CloseAfterSave save = BasePart.CloseAfterSave.False;
            workAssy.Save(saveComponentParts, save);

            workAssy.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Isometric, NXOpen.View.ScaleAdjustment.Fit);
        }*/

        static public void InsertStationAssembly(Part workAssy, string assyName, Point3d basePoint, string folderPath)
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

            string partToAdd = $"{folderPath}{assyName}{NXDrawing.EXTENSION}";

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
                new NXOpen.Layer.StateInfo(layer, NXOpen.Layer.State.Selectable)
            };
            workAssy.Layers.ChangeStates(stateArray);
        }
    }
}
