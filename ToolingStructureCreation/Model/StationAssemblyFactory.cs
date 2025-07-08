using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.View;

namespace ToolingStructureCreation.Model
{
    public class StationAssemblyFactory
    {
        public Dictionary<string, double> PlateThicknesses { get; set; }
        public List<Sketch> StationSketchLists;
        public List<Sketch> ShoeSketchLists;
        string folderPath = "";
        const string STN = "Stn";
        NXDrawing drawing;
        Controller.Control control;
        formToolStructure myForm;

        const double PARALLEL_BAR_WIDTH = 60.0; // Width of the parallel bar

        public StationAssemblyFactory(Dictionary<string, double> plateThicknesses, List<Sketch> stationSketchLists, List<Sketch> shoeSketchLists, string folderPath, Controller.Control control)
        {
            PlateThicknesses = plateThicknesses ?? new Dictionary<string, double>();
            StationSketchLists = stationSketchLists ?? new List<Sketch>();
            ShoeSketchLists = shoeSketchLists ?? new List<Sketch>();
            this.folderPath = folderPath;
            this.control = control;
            myForm = control.GetForm;
        }

        public void CreateStnAsmFactory()
        {
            if (PlateThicknesses == null)
            {
                throw new InvalidOperationException("PlateThicknesses dictionary is not initialized. Please provide a valid dictionary of plate thicknesses.");
            }

            for (int i = 0; i < StationSketchLists.Count; i++)
            {
                Sketch stnSketch = StationSketchLists[i];
                string stnNumber = STN + (i + 1);
                //System.Diagnostics.Debugger.Launch();
                foreach (var plt in PlateThicknesses)
                {
                    if (plt.Key.Equals(NXDrawing.MAT_THK, StringComparison.OrdinalIgnoreCase))
                    {
                        // Skip material thickness, as it is not a plate
                        continue;
                    }
                    Plate plate = new Plate(plt.Key, stnSketch.Length, stnSketch.Width, plt.Value, drawing);
                    plate.CreateNewPlate(folderPath, stnNumber);
                }

                ToolingAssembly.CreateStationAssembly(PlateThicknesses, stnNumber, folderPath);
            }

            for (int i = 0; i < ShoeSketchLists.Count; i++)
            {
                Sketch shoeSketch = ShoeSketchLists[i];
                Shoe upperShoe = new Shoe(Shoe.UPPER_SHOE, shoeSketch.Length, shoeSketch.Width, myForm.UpperShoeThk, drawing);
                Shoe lowerShoe = new Shoe(Shoe.LOWER_SHOE, shoeSketch.Length, shoeSketch.Width, myForm.LowerShoeThk, drawing);
                upperShoe.CreateNewShoe(folderPath);
                lowerShoe.CreateNewShoe(folderPath);

                
                ParallelBar parallelBar = new ParallelBar(PARALLEL_BAR_WIDTH, shoeSketch.Width-85.0, myForm.ParallelBarThk, drawing);
                parallelBar.CreateNewParallelBar(folderPath);

                CommonPlate commonPlate = new CommonPlate(2300, 980, myForm.CommonPltThk, drawing);
                commonPlate.CreateNewCommonPlate(folderPath);
            }
            
        }

        public void CreateToolAsmFactory()
        {
            Session session = Session.GetSession();
            FileNew fileNew = session.Parts.FileNew();
            fileNew.TemplateFileName = ToolingAssembly.TEMPLATE_STP_NAME;
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = ToolingAssembly.ASSEMBLY_TEMPLATE;
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = ToolingAssembly.ASSEMBLY;
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
            
            for (int i = 0; i < StationSketchLists.Count; i++)
            {
                Sketch stnSketch = StationSketchLists[i];
                string stnNumber = STN + (i + 1);
                Point3d startLocation = stnSketch.StartLocation;
                ToolingAssembly.InsertStationAssembly(workAssy, $"{stnNumber}-Assembly", new Point3d(startLocation.X, 0.0, 0.0), folderPath);
            }
            
            for (int i = 0; i < ShoeSketchLists.Count; i++)
            {
                Sketch shoeSketch = ShoeSketchLists[i];
                Shoe.Insert(workAssy, Shoe.UPPER_SHOE, new Point3d(shoeSketch.StartLocation.X, 0.0, myForm.GetUpperShoeZPosition()), folderPath);
                Shoe.Insert(workAssy, Shoe.LOWER_SHOE, new Point3d(shoeSketch.StartLocation.X, 0.0, 0.0), folderPath);

                double firstParallelBarXPosition = shoeSketch.StartLocation.X + (PARALLEL_BAR_WIDTH / 2.0);
                double lastParallelBarXPosition = shoeSketch.StartLocation.X + shoeSketch.Length - PARALLEL_BAR_WIDTH / 2.0;
                Shoe.Insert(workAssy, ParallelBar.PARALLEL_BAR, new Point3d(firstParallelBarXPosition, 0.0, -70.0), folderPath);
                Shoe.Insert(workAssy, ParallelBar.PARALLEL_BAR, new Point3d(shoeSketch.MidPoint.X, 0.0, -70.0), folderPath);
                Shoe.Insert(workAssy, ParallelBar.PARALLEL_BAR, new Point3d(lastParallelBarXPosition, 0.0, -70.0), folderPath);

                Shoe.Insert(workAssy, CommonPlate.LOWER_COMMON_PLATE, new Point3d(shoeSketch.MidPoint.X, 0.0, myForm.GetCommonPlateZPosition()), folderPath);
            }                        

            BasePart.SaveComponents saveComponentParts = BasePart.SaveComponents.True;
            BasePart.CloseAfterSave save = BasePart.CloseAfterSave.False;
            workAssy.Save(saveComponentParts, save);

            workAssy.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Isometric, NXOpen.View.ScaleAdjustment.Fit);
        }
    }
}
