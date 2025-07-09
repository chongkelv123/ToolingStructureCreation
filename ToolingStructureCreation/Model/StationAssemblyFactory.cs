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
        
        Controller.Control control;
        formToolStructure myForm;

        const double PARALLEL_BAR_WIDTH = 60.0; // Width of the parallel bar
        const double Y_POSITION = 0.0; // Y position is always 0.0 for the station assembly
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
                    Plate plate = new Plate(plt.Key, stnSketch.Length, stnSketch.Width, plt.Value);
                    plate.CreateNewPlate(folderPath, stnNumber);
                }

                ToolingAssembly.CreateStationAssembly(PlateThicknesses, stnNumber, folderPath);
            }

            for (int i = 0; i < ShoeSketchLists.Count; i++)
            {
                Sketch shoeSketch = ShoeSketchLists[i];
                Shoe upperShoe = new Shoe(Shoe.UPPER_SHOE, shoeSketch.Length, shoeSketch.Width, myForm.UpperShoeThk);
                Shoe lowerShoe = new Shoe(Shoe.LOWER_SHOE, shoeSketch.Length, shoeSketch.Width, myForm.LowerShoeThk);
                upperShoe.CreateNewShoe(folderPath);
                lowerShoe.CreateNewShoe(folderPath);

                
                ParallelBar parallelBar = new ParallelBar(PARALLEL_BAR_WIDTH, shoeSketch.Width-85.0, myForm.ParallelBarThk);
                parallelBar.CreateNewParallelBar(folderPath);

                Machine machine = myForm.GetMachine;
                var commonPltInfo = machine.GetCommonPlate(myForm.GetMachineName);
                CommonPlate commonPlate = new CommonPlate(commonPltInfo.GetLength(), commonPltInfo.GetWidth(), myForm.CommonPltThk);
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

            // Insert Strip Layout
            StripLayout stripLayout = control.GetStripLayout;
            Shoe.Insert(workAssy, stripLayout.GetFileNameWithoutExtension, stripLayout.GetPosition, folderPath);

            // Orient the work view to Isometric
            workAssy.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Isometric, NXOpen.View.ScaleAdjustment.Fit);
            
            // Insert Station Assembly
            for (int i = 0; i < StationSketchLists.Count; i++)
            {
                Sketch stnSketch = StationSketchLists[i];
                string stnNumber = STN + (i + 1);
                Point3d startLocation = stnSketch.StartLocation;
                double stnAsmZPosition = myForm.GetDiePlt_LowPadThk() * -1;
                
                ToolingAssembly.InsertStationAssembly(workAssy, $"{stnNumber}-Assembly", new Point3d(startLocation.X, Y_POSITION, stnAsmZPosition), folderPath);
            }
            
            // Insert Shoe, Parallel Bar, and Common Plate
            for (int i = 0; i < ShoeSketchLists.Count; i++)
            {
                Sketch shoeSketch = ShoeSketchLists[i];
                double lowShoeZPosition = myForm.GetDiePlt_LowPadThk() * -1;
                Shoe.Insert(workAssy, Shoe.UPPER_SHOE, new Point3d(shoeSketch.StartLocation.X, Y_POSITION, myForm.GetUpperShoeZPosition()), folderPath);
                Shoe.Insert(workAssy, Shoe.LOWER_SHOE, new Point3d(shoeSketch.StartLocation.X, Y_POSITION, lowShoeZPosition), folderPath);

                double firstParallelBarXPosition = shoeSketch.StartLocation.X + (PARALLEL_BAR_WIDTH / 2.0);
                double lastParallelBarXPosition = shoeSketch.StartLocation.X + shoeSketch.Length - PARALLEL_BAR_WIDTH / 2.0;
                double distBetweenFirstLastPBars = (lastParallelBarXPosition - firstParallelBarXPosition);
                const double DIST_BETWEEN_PBAR = 330.0;
                int numberOfParallelBars = (int)Math.Ceiling(distBetweenFirstLastPBars / DIST_BETWEEN_PBAR);
                Shoe.Insert(workAssy, ParallelBar.PARALLEL_BAR, new Point3d(firstParallelBarXPosition, Y_POSITION, myForm.GetParallelBarZPosition()), folderPath);                
                Shoe.Insert(workAssy, ParallelBar.PARALLEL_BAR, new Point3d(lastParallelBarXPosition, Y_POSITION, myForm.GetParallelBarZPosition()), folderPath);

                for (int j = 0; j < numberOfParallelBars-2; j++)
                {
                    double dist = distBetweenFirstLastPBars / (numberOfParallelBars - 1);
                    double xPosition = firstParallelBarXPosition + (j + 1) * dist;
                    Shoe.Insert(workAssy, ParallelBar.PARALLEL_BAR, new Point3d(xPosition, Y_POSITION, myForm.GetParallelBarZPosition()), folderPath);
                }

                Shoe.Insert(workAssy, CommonPlate.LOWER_COMMON_PLATE, new Point3d(shoeSketch.MidPoint.X, Y_POSITION, myForm.GetCommonPlateZPosition()), folderPath);
            }                        

            BasePart.SaveComponents saveComponentParts = BasePart.SaveComponents.True;
            BasePart.CloseAfterSave save = BasePart.CloseAfterSave.False;
            workAssy.Save(saveComponentParts, save);

            workAssy.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Isometric, NXOpen.View.ScaleAdjustment.Fit);
        }
    }
}
