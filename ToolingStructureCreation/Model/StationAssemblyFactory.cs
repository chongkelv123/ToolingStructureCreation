using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Services;
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

        List<string> uprShoeComponentCollection;
        List<string> lowShoeComponentCollection;
        List<string> shoeComponentCollection;
        List<string> pBarComponentCollection;
        List<string> comPltComponentCollection;
        List<string> subToolAsmCollection;
        public StationAssemblyFactory(Dictionary<string, double> plateThicknesses, List<Sketch> stationSketchLists, List<Sketch> shoeSketchLists, string folderPath, Controller.Control control)
        {
            PlateThicknesses = plateThicknesses ?? new Dictionary<string, double>();
            StationSketchLists = stationSketchLists ?? new List<Sketch>();
            ShoeSketchLists = shoeSketchLists ?? new List<Sketch>();
            this.folderPath = folderPath;
            this.control = control;
            myForm = control.GetForm;
            shoeComponentCollection = new List<string>();

            uprShoeComponentCollection = new List<string>();
            lowShoeComponentCollection = new List<string>();
            pBarComponentCollection = new List<string>();
            comPltComponentCollection = new List<string>();
            subToolAsmCollection = new List<string>();
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
                int stnNo = i + 1;
                string stnNumber = STN + stnNo;
                List<string> pltComponentNames = new List<string>();
                Dictionary<string, double> pltLists = new Dictionary<string, double>();
                PlateCodeGeneratorService pltCodeGenerator;
                foreach (var plt in PlateThicknesses)
                {
                    if (plt.Key.Equals(NXDrawing.MAT_THK, StringComparison.OrdinalIgnoreCase))
                    {
                        // Skip material thickness, as it is not a plate
                        pltLists.Add(plt.Key, plt.Value);
                        continue;
                    }
                    var type = CodeGeneratorService.GetToolingType(plt.Key);
                    pltCodeGenerator = new PlateCodeGeneratorService(control, myForm.GetProjectInfo(), type, stnNo);
                    string fileName = pltCodeGenerator.AskFileName();
                    pltLists.Add(fileName, plt.Value);
                    Plate plate = new Plate(fileName, stnSketch.Length, stnSketch.Width, plt.Value);
                    plate.CreateNewPlate(folderPath);
                }

                string itemName = $"{stnNumber}-Assembly";
                AsmCodeGeneratorServicecs asmCodeGenerator = new AsmCodeGeneratorServicecs(control, myForm.GetProjectInfo(), itemName);
                string subAsmFileName = asmCodeGenerator.AskFileName();
                subToolAsmCollection.Add(subAsmFileName);
                CreateStationAssembly(pltLists, subAsmFileName, folderPath);
            }

            Sketch shoeSketch = null;

            for (int i = 0; i < ShoeSketchLists.Count; i++)
            {
                shoeSketch = ShoeSketchLists[i];
                ProjectInfo projectInfo = myForm.GetProjectInfo();
                ShoeCodeGeneratorService uprShoeGenerator = new ShoeCodeGeneratorService(control, projectInfo, Shoe.UPPER_SHOE);
                string uprShoeFileNameWithoutExtension = uprShoeGenerator.AskFileName();
                uprShoeComponentCollection.Add(uprShoeFileNameWithoutExtension);
                Shoe upperShoe = new Shoe(uprShoeFileNameWithoutExtension, shoeSketch.Length, shoeSketch.Width, myForm.UpperShoeThk);
                upperShoe.CreateNewShoe(folderPath);

                ShoeCodeGeneratorService lowShoeGenerator = new ShoeCodeGeneratorService(control, projectInfo, Shoe.LOWER_SHOE);
                string lowShoeFileNameWithoutExtension = lowShoeGenerator.AskFileName();
                lowShoeComponentCollection.Add(lowShoeFileNameWithoutExtension);
                Shoe lowerShoe = new Shoe(lowShoeFileNameWithoutExtension, shoeSketch.Length, shoeSketch.Width, myForm.LowerShoeThk);
                lowerShoe.CreateNewShoe(folderPath);
            }

            if (shoeSketch != null)
            {
                string pBarItemName = ParallelBar.PARALLEL_BAR;
                ShoeCodeGeneratorService pBarCodeGenerator = new ShoeCodeGeneratorService(control, myForm.GetProjectInfo(), pBarItemName);
                string fileName = pBarCodeGenerator.AskFileName();
                pBarComponentCollection.Add(fileName);
                ParallelBar parallelBar = new ParallelBar(fileName, PARALLEL_BAR_WIDTH, shoeSketch.Width - 85.0, myForm.ParallelBarThk);
                parallelBar.CreateNewParallelBar(folderPath);
            }

            string compPltItemName = CommonPlate.LOWER_COMMON_PLATE;
            ShoeCodeGeneratorService compCodeGenerator = new ShoeCodeGeneratorService(control, myForm.GetProjectInfo(), compPltItemName);
            string compPlatefileName = compCodeGenerator.AskFileName();
            comPltComponentCollection.Add(compPlatefileName);
            Machine machine = myForm.GetMachine;
            var commonPltInfo = machine.GetCommonPlate(myForm.GetMachineName);
            CommonPlate commonPlate = new CommonPlate(commonPltInfo.GetLength(), commonPltInfo.GetWidth(), myForm.CommonPltThk, compPlatefileName);
            commonPlate.CreateNewCommonPlate(folderPath);
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

            string itemName = "MainToolAssembly";
            AsmCodeGeneratorServicecs asmCodeGenerator = new AsmCodeGeneratorServicecs(control, myForm.GetProjectInfo(), itemName);
            string asmFileName = asmCodeGenerator.AskFileName();
            fileNew.NewFileName = $"{folderPath}{asmFileName}{NXDrawing.EXTENSION}";
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
                string compName = subToolAsmCollection[i];                
                Point3d startLocation = stnSketch.StartLocation;
                double stnAsmZPosition = myForm.GetDiePlt_LowPadThk() * -1;

                ToolingAssembly.InsertStationAssembly(
                    workAssy,
                    compName, 
                    new Point3d(startLocation.X, Y_POSITION, stnAsmZPosition), 
                    folderPath);
            }

            // Insert Shoe, Parallel Bar, and Common Plate
            for (int i = 0; i < ShoeSketchLists.Count; i++)
            {
                Sketch shoeSketch = ShoeSketchLists[i];
                double uprShoeZPosition = myForm.GetUpperShoeZPosition();
                double lowShoeZPosition = myForm.GetDiePlt_LowPadThk() * -1;
                foreach (var compName in uprShoeComponentCollection)
                {
                    Shoe.Insert(
                        workAssy,
                        compName,
                        new Point3d(shoeSketch.StartLocation.X, Y_POSITION, uprShoeZPosition),
                        folderPath);
                }
                foreach (var compName in lowShoeComponentCollection)
                {
                    Shoe.Insert(
                        workAssy,
                        compName,
                        new Point3d(shoeSketch.StartLocation.X, Y_POSITION, lowShoeZPosition),
                        folderPath);
                }

                foreach (var compName in pBarComponentCollection)
                {
                    double firstParallelBarXPosition = shoeSketch.StartLocation.X + (PARALLEL_BAR_WIDTH / 2.0);
                    double lastParallelBarXPosition = shoeSketch.StartLocation.X + shoeSketch.Length - PARALLEL_BAR_WIDTH / 2.0;
                    double distBetweenFirstLastPBars = (lastParallelBarXPosition - firstParallelBarXPosition);
                    const double DIST_BETWEEN_PBAR = 330.0;
                    int numberOfParallelBars = (int)Math.Ceiling(distBetweenFirstLastPBars / DIST_BETWEEN_PBAR);
                    Shoe.Insert(workAssy, compName, new Point3d(firstParallelBarXPosition, Y_POSITION, myForm.GetParallelBarZPosition()), folderPath);
                    Shoe.Insert(workAssy, compName, new Point3d(lastParallelBarXPosition, Y_POSITION, myForm.GetParallelBarZPosition()), folderPath);

                    for (int j = 0; j < numberOfParallelBars - 2; j++)
                    {
                        double dist = distBetweenFirstLastPBars / (numberOfParallelBars - 1);
                        double xPosition = firstParallelBarXPosition + (j + 1) * dist;
                        Shoe.Insert(workAssy, compName, new Point3d(xPosition, Y_POSITION, myForm.GetParallelBarZPosition()), folderPath);
                    }
                }
                
                foreach(var compName in comPltComponentCollection)
                {
                    Shoe.Insert(
                        workAssy,
                        compName, 
                        new Point3d(shoeSketch.MidPoint.X, Y_POSITION, myForm.GetCommonPlateZPosition()), 
                        folderPath);
                }
                
            }

            BasePart.SaveComponents saveComponentParts = BasePart.SaveComponents.True;
            BasePart.CloseAfterSave save = BasePart.CloseAfterSave.False;
            workAssy.Save(saveComponentParts, save);

            workAssy.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Isometric, NXOpen.View.ScaleAdjustment.Fit);
        }

        public void CreateStationAssembly(Dictionary<string, double> plateList, string fileName, string folderPath)
        {
            Session session = Session.GetSession();
            FileNew fileNew = session.Parts.FileNew();
            fileNew.TemplateFileName = ToolingAssembly.TEMPLATE_STP_NAME;
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = ToolingAssembly.ASSEMBLY_TEMPLATE;
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = ToolingAssembly.ASSEMBLY;
            fileNew.SetCanCreateAltrep(false);            
            fileNew.NewFileName = $"{folderPath}{fileName}{NXDrawing.EXTENSION}";
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
                string fn = component.Key;
                Plate.InsertPlate(workPart, fn, cumThk, folderPath);
            }
            BasePart.SaveComponents saveComponentParts = BasePart.SaveComponents.True;
            BasePart.CloseAfterSave save = BasePart.CloseAfterSave.True;
            workPart.Save(saveComponentParts, save);
        }
    }
}
